using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class STTResponse
{
    public bool success;
    public string transcription;
    public string originalFilename;
}

[Serializable]
public class MultimodalResponse
{
    public bool success;
    public string response;
    public string originalFilename;
    public string prompt;
}

[Serializable]
public class CharacterInfo 
{
    public string characterName;
    public string voiceId;
    public string characterPersonality;
    public Texture2D characterImage;
}

[Serializable]
public class TTSRequestData
{
    public string text;
    public string voiceId;
}

[Serializable]
public class ImageToTextResponse
{
    public string response;
}

[Serializable]
public class Agent
{
    public string name;
    public string description;
    public string motivation;
    public string knowledge;
}

[Serializable]
public class ClientMessage
{
    public string type;
}

[Serializable]
public class ClientAudioMessage : ClientMessage
{
    public List<List<float>> audio;
}

[Serializable]
public class ClientTextMessage : ClientMessage
{
    public string text;
}

[Serializable]
public class ClientLLMInput : ClientMessage
{
    public string text;
    public string image;
    public string voiceId;
}

[Serializable]
public class ServerMessage
{
    public string type;
    public TextData text;
    public AudioData audio;
    public string error;
    public RoutingData routing;
    public PacketData packetId;
    public string date;
}

[Serializable]
public class TextData
{
    public string text;
    public bool final;
}

[Serializable]
public class AudioData
{
    public string chunk;
}

[Serializable]
public class RoutingData
{
    public SourceData source;
}

[Serializable]
public class SourceData
{
    public bool isAgent;
    public string name;
}

[Serializable]
public class PacketData
{
    public string utteranceId;
    public string interactionId;
}

[Serializable]
public class SessionResponse
{
    public string sessionKey;
}