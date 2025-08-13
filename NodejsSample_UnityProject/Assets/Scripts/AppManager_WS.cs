using System;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class AppManager_WS : MonoBehaviour
{
    static AppManager_WS m_instance;
    public static AppManager_WS Instance => m_instance;

    WebSocketController m_webSocketController;

    [SerializeField]
    CameraRecordingController m_cameraRecordingController;

    [SerializeField]
    TextDisplayController m_textDisplayController;

    [SerializeField]
    UIPanelController m_uiPanelController;

    [SerializeField]
    Button m_recordingButton;

    [SerializeField]
    GameObject m_interactionBlocker;

    byte[] m_imageBytes;

    [SerializeField]
    string m_httpURL = "https://nodeinworldruntime-production.up.railway.app"; //"http://localhost:4000";

    [SerializeField]
    string m_wsURL = "wss://nodeinworldruntime-production.up.railway.app"; //"ws://localhost:4000";

    [SerializeField]
    string m_APIKey;
    public string GetAPIKey() => m_APIKey;

    [SerializeField]
    string m_APISecret;
    public string GetAPISecret() => m_APISecret;

    public event Action<string> OnTextUpdated;

    // Current Character
    [SerializeField]
    CharacterInfo currentCharacter = null;

    bool m_isRecordingPressing = false;
    string m_messageFrom = "";
    string m_lastInteractionId = "";
    StringBuilder m_displayContent = new StringBuilder();

    void Awake()
    {
        CreateSingleton();
        RequestPermissions();
        m_webSocketController = GetComponent<WebSocketController>();
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

    public void SetupWebsocket()
    {
        if(m_webSocketController != null)
        {
            StartCoroutine(m_webSocketController.InitializeConnection(m_httpURL, m_wsURL));
        }
    }

    private void Start()
    {
        SetupWebsocket();
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

        m_webSocketController.OnTextReceived += OnTextReceived;
        m_webSocketController.OnAudioPlayingChanged += OnAudioPlayChanges;
    }

    private void OnDestroy()
    {
        m_cameraRecordingController.StopCamera();
        m_textDisplayController.StopTextUpdate();

        m_webSocketController.OnTextReceived -= OnTextReceived;
        m_webSocketController.OnAudioPlayingChanged -= OnAudioPlayChanges;
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
        m_webSocketController.StartRecording();
        OnTextUpdated?.Invoke("");
        m_isRecordingPressing = true;
    }

    public void OnRecordingBtnReleased()
    {
        // Stop recording audio
        m_webSocketController.StopRecording();
        m_isRecordingPressing = false;

        if(m_displayContent.ToString().Length > 0)
        {
            SendImage(m_displayContent.ToString());
        }
    }

    void SendImage(string sttResult)
    {
        //Debug.Log($"In sending image");
        if (m_imageBytes != null && m_imageBytes.Length > 0 && sttResult.Trim().Length > 0)
        {
            //Debug.Log($"SendImage... check passed");
            string prompt = $"You are a character with the following characteristics ({currentCharacter.characterPersonality}) and are replying to the user's question ({sttResult}) regarding this image. Generate a response in text, keep it a short sentence and try to make it funny and aligned with the character's personality.Never use any emoji. Do not refer to the user as darling or dear, simply answer the question and do not address the user directly.";
            m_webSocketController.SendImageWithPrompt(prompt, m_imageBytes, currentCharacter.voiceId);

            m_imageBytes = null; // Clear the image bytes after sending
        }
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

    void OnTextReceived(string text, string name, string interactionid)
    {
        //Debug.Log($"Received text from WebSocket: {text}");
        if (m_messageFrom != name)
        {
            m_displayContent.Clear();
            m_messageFrom = name;
        }
        
        if(m_messageFrom != "User" && m_lastInteractionId != interactionid)
        {
            m_displayContent.Clear();
        }

        m_lastInteractionId = interactionid;

        m_displayContent.Append(text);

        if(name == "User" && !m_isRecordingPressing)
        {
            // result from STT
            SendImage(m_displayContent.ToString());
        }
        OnTextUpdated?.Invoke(m_displayContent.ToString());
    }

    void OnAudioPlayChanges(bool isPlaying)
    {
        m_recordingButton.interactable = !isPlaying;
        m_interactionBlocker.SetActive(isPlaying);
    }
}
