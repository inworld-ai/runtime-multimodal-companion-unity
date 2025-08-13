# node-runtime-project-unity

This Unity project demonstrates integration with the [Inworld Node Runtime Server](https://github.com/inworld-ai/node-runtime-project). It connects to a server to enable real-time communication features.

## Instructions

### 1. Open the Unity Project

- Open the Unity project in this repository.
- Navigate to `Assets/Scenes/` and open the `DemoScene_WebSocket.unity`.

### 2. Update API Key and API Secret

- In the `DemoScene_WebSocket`, select the `AppManager` GameObject.
- Locate the `AppManager_WS` component in the Inspector.
- Set the **API Key** and **API Secret** field

### 3. Build & Run on Android

- Open `File > Build Settings`.
- Switch the platform to **Android**.
- Add `DemoScene_WebSocket` to the build scenes.
- Click **Build and Run** to deploy the APK to your Android device.
