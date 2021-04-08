using System;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using Logger;
using Shares;
using Shares.Model;
using Shares.Enum;


namespace OBSWebsocketController
{
    public class OBSWebsocketControllerClient
    {
        private OBSWebsocket OBSWebsocket = new OBSWebsocket();
        OBSSettingModel obsSettings;

        public OBSWebsocketControllerClient()
        {
            OBSWebsocket.Connected += OBSWebsocket_Connected;
            OBSWebsocket.Disconnected += OBSWebsocket_Disconnected;
            OBSWebsocket.OBSExit += OBSWebsocket_OBSExit;
            OBSWebsocket.SceneChanged += OBSWebsocket_SceneChanged;
            OBSWebsocket.Heartbeat += OBSWebsocket_Heartbeat;

            obsSettings = Settings.LoadSettings<OBSSettingModel>(FileType.OBSSettings);

            if (obsSettings == null) return;

            TryConnect();
        }
        private void TryConnect()
        {
            OBSWebsocket.Connect($"ws://127.0.0.1:{obsSettings.WebSocketPort}", obsSettings.WebSocketPassword);

            if (!OBSWebsocket.IsConnected)
            {
                Log.Fatal($"Could not connect not OBS Websocket ws://127.0.0.1:{obsSettings.WebSocketPort}", GetType().Name);
            }
        }
        private void OBSWebsocket_Heartbeat(OBSWebsocket sender, Heartbeat heatbeat)
        {
            Console.WriteLine(heatbeat.Stats);
        }
        private void OBSWebsocket_SceneChanged(OBSWebsocket sender, string newSceneName)
        {
            Log.Info($"OBS Websocket switched scene to {newSceneName}", GetType().Name);
        }
        private void OBSWebsocket_OBSExit(object sender, EventArgs e)
        {
            Log.Info("OBS Exited", GetType().Name);
        }
        public void SwitchToScene(string sceneName)
        {
            OBSWebsocket.SetCurrentScene(sceneName);
        }
        private void OBSWebsocket_Disconnected(object sender, EventArgs e)
        {
            Log.Info($"OBS Websocket disconnected from ws://127.0.0.1:{obsSettings.WebSocketPort}", GetType().Name);
        }
        private void OBSWebsocket_Connected(object sender, EventArgs e)
        {
            Log.Info($"Successfully connected to OBS Websocket: ws://127.0.0.1:{obsSettings.WebSocketPort}", GetType().Name);
        }
    }
}
