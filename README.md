<h1 align="center">Inworld Runtime App Template - Multimodal Companion (Unity Client)</h1>

<p align="center"> 
    <a href="https://docs.inworld.ai/docs/introduction"><strong>Build with Inworld</strong></a> · 
    <a href="https://docs.inworld.ai/docs/node/templates/multimodal-companion"><strong>Template Docs</strong></a> · 
    <a href="https://github.com/inworld-ai/runtime-multimodal-companion-node"><strong>Node.js Server Repo</strong></a>
</p> 

<p align="center">
The Real-time Multimodal Companion Template demonstrates how to build an AI companion that combines speech-to-text, image understanding, and text-to-speech using WebSocket communication. This Unity project works with a <a href="https://github.com/inworld-ai/runtime-multimodal-companion-node"><strong>Node.js server</strong></a> to deliver a complete, real-time interactive experience.
</p>

## How to use

### 1. Open the Unity Project

- Clone or download this repository.
- Open the Unity project.
- Navigate to `Assets/Scenes/` and open the `DemoScene_WebSocket.unity`.

### 2. Update API Key and API Secret

- In the `DemoScene_WebSocket`, select the `AppManager` GameObject.
- Locate the `AppManager_WS` component in the Inspector.
- Enter your **API Key** and **API Secret**

### 3. Build & Run on Android

- Open `File > Build Settings`.
- Switch the platform to **Android**.
- Add `DemoScene_WebSocket` to the build scenes.
- Click **Build and Run** to deploy the APK to your Android device.
