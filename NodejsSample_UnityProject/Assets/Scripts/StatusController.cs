using System.Collections;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class StatusController : MonoBehaviour
{
    [SerializeField]
    WebSocketController m_webSocketController;

    [SerializeField]
    GameObject m_errorPanel;

    [SerializeField]
    GameObject m_restartPanel;

    [SerializeField]
    TMP_Text m_errorPopup;

    int m_reconnectionAttempts = 5;
    bool m_isConnected = false;

    IEnumerator errorPrintIEnumerator;
    IEnumerator reconnectionAttemptCoroutine;

    void Awake()
    {
        m_webSocketController.OnErrorReceived += UpdateStatus;
        m_webSocketController.OnDisconnected += OnWebsocketDisconnected;
        m_webSocketController.OnConnected += OnWebsocketReconnected;
    }

    private void OnDestroy()
    {
        m_webSocketController.OnErrorReceived -= UpdateStatus;
        m_webSocketController.OnDisconnected -= OnWebsocketDisconnected;
        m_webSocketController.OnConnected -= OnWebsocketReconnected;
    }

    void UpdateStatus(string status)
    {
        if (errorPrintIEnumerator != null)
        {
            StopCoroutine(errorPrintIEnumerator);
        }
        errorPrintIEnumerator = ErrorPrintCoroutine(status);
        StartCoroutine(errorPrintIEnumerator);

    }

    IEnumerator ErrorPrintCoroutine(string status)
    {
        if (status.StartsWith("WebSocket Error: "))
        {
            status = status.Substring("WebSocket Error: ".Length);
        }

        m_errorPopup.text = status;
        yield return new WaitForSeconds(3f);
        m_errorPopup.text = "";
        errorPrintIEnumerator = null;
    }

    void OnWebsocketDisconnected()
    {
        m_isConnected = false;
        m_errorPanel.SetActive(true);

        if (reconnectionAttemptCoroutine != null)
            StopCoroutine(reconnectionAttemptCoroutine);
        reconnectionAttemptCoroutine = ReconnectionAttempt();
        StartCoroutine(reconnectionAttemptCoroutine);
    }

    void OnWebsocketReconnected()
    {
        m_isConnected = true;
        m_restartPanel.SetActive(false);
        m_errorPanel.SetActive(false);
    }

    IEnumerator ReconnectionAttempt()
    {
        int count = 0;
        while (count < m_reconnectionAttempts && !m_isConnected)
        {
            AppManager_WS.Instance.SetupWebsocket();
            count++;

            yield return new WaitForSeconds(5f);
        }

        if (!m_isConnected)
        {
            m_errorPanel.SetActive(false);
            m_restartPanel.SetActive(true);
        }

        reconnectionAttemptCoroutine = null;
    }
}

