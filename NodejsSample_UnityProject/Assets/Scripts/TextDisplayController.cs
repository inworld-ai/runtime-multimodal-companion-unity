using TMPro;
using UnityEngine;

public class TextDisplayController : MonoBehaviour
{
    [SerializeField]
    TMP_Text m_textComponent;

    public void StartTextUpdate()
    {
        if (AppManager.Instance != null)
        {
            // Add Listener to STT
            AppManager.Instance.OnSTTReceived += OnTextReceived;

            // Add Listener to LLM
            AppManager.Instance.OnLLMReceived += OnTextReceived;
        }

        if(AppManager_WS.Instance != null)
        {
            AppManager_WS.Instance.OnTextUpdated += OnTextReceived;
        }
    }

    public void StopTextUpdate()
    {
        if (AppManager.Instance != null)
        {
            // Remove Listener from STT
            AppManager.Instance.OnSTTReceived -= OnTextReceived;

            // Remove Listener from LLM
            AppManager.Instance.OnLLMReceived -= OnTextReceived;
        }

        if (AppManager_WS.Instance != null)
        {
            AppManager_WS.Instance.OnTextUpdated -= OnTextReceived;
        }
    }

    void OnTextReceived(string str)
    {
        if (m_textComponent != null)
        {
            m_textComponent.text = str;
        }
    }
}
