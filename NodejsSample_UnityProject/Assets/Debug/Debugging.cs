using UnityEngine;

public class Debugging : MonoBehaviour
{

    public Texture2D DebugTexture;
    public AudioClip DebugAudioClip;

    [SerializeField]
    MicrophoneController microphoneController;

    [SerializeField]
    CameraRecordingController cameraRecordingController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRecordingPressed() 
    {
        if(AppManager.Instance != null)
            AppManager.Instance.SetDebugImage(GetDebugImageData());

    }

    public void OnRecordingReleased()
    {
        if (AppManager.Instance != null)
            AppManager.Instance.SendDebugAudio(GetDebugAudioData());
    }

    byte[] GetDebugAudioData()
    {
        Debug.Log("Using debug contents...");
        byte[] wavData = microphoneController.AudioClipToWavBytes(DebugAudioClip);
        return wavData;
    }

    byte[] GetDebugImageData()
    {
        Debug.Log("Using debug image contents...");
        return DebugTexture.EncodeToJPG();
    }
}
