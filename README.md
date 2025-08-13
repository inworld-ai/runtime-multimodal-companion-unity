# node-runtime-project-unity

This Unity project demonstrates integration with the [Inworld Node Runtime Server](https://github.com/inworld-ai/node-runtime-project). It connects to a locally hosted Node.js server to enable real-time communication features.

## Instructions

### 1. Start the Node Runtime Server

Follow the setup guide from the original Node.js server repo:  
[node-runtime-project](https://github.com/inworld-ai/node-runtime-project)

Make sure the server is running locally on your machine (default port is `3000`).

### 2. Open the Unity Project

- Open the Unity project in this repository.
- Navigate to `Assets/Scenes/` and open the `DemoScene.unity`.

### 3. Update Server URL

- In the `DemoScene`, select the `AppManager` GameObject.
- Locate the `AppManager` component in the Inspector.
- Set the **ServerUrl** field to your local machine's IPv4 address:
http://<YOUR_IPV4_IP_ADDRESS>:3000


> You can find your local IPv4 address by running `ipconfig` on Windows or `ifconfig` on macOS/Linux.

### 4. Build & Run on Android

- Open `File > Build Settings`.
- Switch the platform to **Android**.
- Add `DemoScene` to the build scenes.
- Click **Build and Run** to deploy the APK to your Android device.

>  Ensure that your Android device is on the same local network as your server host.