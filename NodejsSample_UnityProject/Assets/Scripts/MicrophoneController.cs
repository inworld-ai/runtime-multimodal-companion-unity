using System.IO;
using UnityEngine;

public class MicrophoneController: MonoBehaviour
{
    public string microphoneDevice;
    public int sampleRate = 16000;
    public int maxRecordSeconds = 10;

    private AudioClip recordedClip;
    private bool isRecording = false;

    void Start()
    {
        if (Microphone.devices.Length > 0)
            microphoneDevice = Microphone.devices[0];
        else
            Debug.LogError("No microphone device found!");
    }

    public void StartRecording()
    {
        if (isRecording || microphoneDevice == null) return;

        Debug.Log("Recording started...");
        recordedClip = Microphone.Start(microphoneDevice, false, maxRecordSeconds, sampleRate);
        isRecording = true;
    }

    public byte[] StopRecording()
    {
        if (!isRecording) return null;

        Debug.Log("Recording stopped.");
        Microphone.End(microphoneDevice);
        isRecording = false;

        byte[] wavData = AudioClipToWavBytes(recordedClip);
        Debug.Log($"WAV Byte Length: {wavData.Length}");
        return wavData;
        // Now wavData can be uploaded, saved, or used however you need
    }

    //byte[] AudioClipToWavBytes(AudioClip clip)
    public byte[] AudioClipToWavBytes(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] wav = ConvertToWav(samples, clip.channels, clip.frequency);
        return wav;
    }

    byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        MemoryStream stream = new MemoryStream();
        int sampleCount = samples.Length;
        int byteRate = sampleRate * channels * 2;

        // Write WAV header
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(36 + sampleCount * 2);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));

            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            writer.Write(16); // PCM
            writer.Write((short)1); // format = PCM
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)(channels * 2));
            writer.Write((short)16); // bits per sample

            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            writer.Write(sampleCount * 2);

            // Write sample data
            foreach (var sample in samples)
            {
                short intSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                writer.Write(intSample);
            }
        }

        return stream.ToArray();
    }
}
