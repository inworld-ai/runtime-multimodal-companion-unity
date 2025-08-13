using System;
using System.Collections;
using Inworld;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class AppManager : MonoBehaviour
{
    static AppManager m_instance;
    public static AppManager Instance => m_instance;

    [SerializeField]
    CameraRecordingController m_cameraRecordingController;

    [SerializeField]
    TextDisplayController m_textDisplayController;

    [SerializeField]
    UIPanelController m_uiPanelController;

    AudioSource m_ttsAudio;
    MicrophoneController m_microphoneController;

    byte[] m_imageBytes;
    byte[] m_audioBytes;

    [SerializeField]
    string m_serverUrl = "http://localhost:3000";
    
    string m_sttUrl = "";
    string m_ttsUrl = "";
    string m_imageToTextUrl = "";

    public event Action<string> OnSTTReceived;
    public event Action<string> OnLLMReceived;

    // Current Character
    [SerializeField]
    CharacterInfo currentCharacter = null;

    void Awake()
    {
        CreateSingleton();
        RequestPermissions();
        SetupUrl();

        m_microphoneController = GetComponent<MicrophoneController>();
        m_ttsAudio = GetComponent<AudioSource>();
    }

    void CreateSingleton()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            m_instance = this;
        }
    }

    void SetupUrl()
    {
        m_sttUrl = $"{m_serverUrl}/stt";
        m_ttsUrl = $"{m_serverUrl}/tts";
        m_imageToTextUrl = $"{m_serverUrl}/chat";
    }

    private void Start()
    {
        m_cameraRecordingController.StartCamera();
        m_textDisplayController.StartTextUpdate();

        if(currentCharacter == null)
        {
            CharacterController[] allCharacter = FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
            if (allCharacter != null && allCharacter.Length > 0)
            {
                currentCharacter = allCharacter[0].Info; // Default to the first character
            }
        }
    }

    private void OnDestroy()
    {
        m_cameraRecordingController.StopCamera();
        m_textDisplayController.StopTextUpdate();

    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void RequestPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    public void OnRecordingBtnPressed()
    {
        // Image Capture
        m_imageBytes = m_cameraRecordingController.CapturePhotoAsByteArray();
        // Start recording audio
        m_microphoneController.StartRecording();
    }

    public void OnRecordingBtnReleased()
    {
        // Stop recording audio
        m_audioBytes = m_microphoneController.StopRecording();

        // Send audio for Speech-to-Text
        if(m_audioBytes != null && m_audioBytes.Length > 0)
        {
            StartCoroutine(SpeechToTextRequest(m_audioBytes));
        }
    }

    public void SetDebugImage(byte[] imagedata)
    {
        m_imageBytes = imagedata;
    }

    public void SendDebugAudio(byte[] audiodata)
    {
        StartCoroutine(SpeechToTextRequest(audiodata));
    }

    IEnumerator SpeechToTextRequest(byte[] audioBytes)
    {
        Debug.Log("Sending audio for STT...");

        // Prepare form with the audio file
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", audioBytes, "audio.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(m_sttUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("STT Response: " + www.downloadHandler.text);
                string json = www.downloadHandler.text;

                // Parse JSON into C# object
                STTResponse response = JsonUtility.FromJson<STTResponse>(json);

                string transcriptText = response.transcription;
                Debug.Log("Transcription: " + transcriptText);

                OnSTTReceived?.Invoke(transcriptText);
                SendImage(transcriptText);
            }
            else
            {
                Debug.LogError("STT Upload failed: " + www.error);
            }

            www.uploadHandler.Dispose();
            www.downloadHandler.Dispose();
        }
    }

    void SendImage(string sttResult)
    {
        Debug.Log($"In sending image");
        if (m_imageBytes != null && m_imageBytes.Length > 0 && sttResult.Trim().Length > 0)
        {
            Debug.Log($"SendImage... check passed");
            string systemPrompt = $"You are a character with the following characteristics({currentCharacter.characterPersonality}) and are replying to the user’s question regarding this image.Generate a response in text, keep it a short sentence and try to make it funny and aligned with the character’s personality. Never use any emoji.";
            string prompt = sttResult;
            StartCoroutine(ImageToTextRequest(m_imageBytes, systemPrompt, prompt));
        }
    }

    IEnumerator ImageToTextRequest(byte[] imageBytes, string systemPrompt , string prompt)
    {
        // Create a form and add prompt and image file
        WWWForm form = new WWWForm();
        form.AddField("prompt", prompt);
        form.AddField("systemPrompt", systemPrompt);
        form.AddBinaryData("image", imageBytes, "image.jpg", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post(m_imageToTextUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                ImageToTextResponse res = JsonUtility.FromJson<ImageToTextResponse>(www.downloadHandler.text);
                if (res != null && !string.IsNullOrEmpty(res.response))
                {
                    Debug.Log("Parsed Response: " + res.response);
                    OnLLMReceived?.Invoke(res.response);  // Notify listeners with the response
                    SendTTSRequest(res.response);  // Send text to TTS
                }
                else
                {
                    Debug.LogWarning("Failed to parse 'response' field.");
                }
            }
            www.uploadHandler.Dispose();
            www.downloadHandler.Dispose();
        }
        ResetData();
    }

    public void SendTTSRequest(string llmResult)
    {
        Debug.Log($"In Send TTS Request: {llmResult}");
        if (!string.IsNullOrEmpty(llmResult))
        {
            Debug.Log($"llmResult is not empty, sending TTS request: {llmResult}");
            StartCoroutine(SendTTSRequestCoroutine(llmResult));
        }
    }

    IEnumerator SendTTSRequestCoroutine(string content)
    {
        // Prepare JSON body
        TTSRequestData requestData = new TTSRequestData
        {
            text = content,
            voiceId = currentCharacter.voiceId
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(m_ttsUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("TTS request failed: " + request.error);
                yield break;
            }

            Debug.Log("TTS request succeeded. Converting to AudioClip...");

            byte[] wavData = request.downloadHandler.data;
            AudioClip clip = WavUtility.ToAudioClip(wavData);

            if (clip != null)
            {
                m_ttsAudio.clip = clip;
                m_ttsAudio.Play();
            }
            else
            {
                Debug.LogError("Failed to convert wav data to AudioClip.");
            }
        }
    }

    void ResetData()
    {
        m_imageBytes = null;
        m_audioBytes = null;
    }

    public void SetCharacter(CharacterInfo newCharacter)
    {
        if (newCharacter.characterName == currentCharacter.characterName)
        {
            return;
        }
        currentCharacter = newCharacter;
        m_uiPanelController.UpdateDisplay(newCharacter);
    }
}
