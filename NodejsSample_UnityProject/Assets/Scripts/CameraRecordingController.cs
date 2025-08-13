using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraRecordingController : MonoBehaviour
{
    RawImage m_display;
    WebCamTexture m_webCamTexture;
    Texture m_source;
    RenderTexture m_renderTexture;
    bool m_isInited = false;

    void Awake()
    {
        m_display = GetComponent<RawImage>();
    }

    void Update()
    {
        if (m_source == null)
        {
            return;
        }

        Graphics.Blit(m_source, m_renderTexture);
    }

    public void StartCamera()
    {
        StartCoroutine(InitCameraTexture());
    }

    void UseExternalTexture(Texture source)
    {
        m_source = source;
        if (m_renderTexture == null)
        {
            m_renderTexture = new RenderTexture(640, 360, 0, RenderTextureFormat.ARGB32);
        }
    }

    IEnumerator InitCameraTexture()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            int rearCamIndex = 0; 
            m_webCamTexture = new WebCamTexture(devices[rearCamIndex].name, 1280, 720, 30);

            if (m_webCamTexture != null)
            {
                m_display.texture = m_webCamTexture;
                m_webCamTexture.Play();

                while (!m_webCamTexture.didUpdateThisFrame)
                {
                    yield return null;
                }

                m_isInited = true;

                UseExternalTexture(m_webCamTexture);
                Debug.Log("Camera started: " + devices[rearCamIndex].name);
                Debug.Log("Actual resolution: " + m_webCamTexture.width + "x" + m_webCamTexture.height);
            }
        }
        else
        {
            Debug.LogWarning("No camera found on device.");
        }
    }

    public void StopCamera()
    {
        if (m_webCamTexture != null && m_webCamTexture.isPlaying)
        {
            m_webCamTexture.Stop();
            m_isInited = false;
        }
    }

    public byte[] CapturePhotoAsByteArray()
    {
        if (!m_isInited)
        {
            return null;
        }

        Texture2D photo = CapturePhoto(m_webCamTexture);
        byte[] imageBytes = photo.EncodeToJPG();
        return imageBytes;
    }

    public string CapturePhotoAsBase64()
    {
        if (!m_isInited)
        {
            return "";
        }

        return Convert.ToBase64String(CapturePhotoAsByteArray());
    }

    Texture2D CapturePhoto(WebCamTexture webcamTexture)
    {
        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        photo.SetPixels(webcamTexture.GetPixels());
        photo.Apply();
        return photo;
    }
}
