using System;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;

namespace OBSWebsocketController
{
    public class OBSWebsocketControllerClient
    {
        private OBSWebsocket OBSWebsocket = new OBSWebsocket();

        public OBSWebsocketControllerClient()
        {
            OBSWebsocket.Connect("ws://127.0.0.1:4444", "bnY50sZcCf1b1sWwriHn");
            OBSWebsocket.Connected += OBSWebsocket_Connected;
            OBSWebsocket.Disconnected += OBSWebsocket_Disconnected;
        }

        public void SwitchToScene(string sceneName)
        {
            OBSWebsocket.SetCurrentScene(sceneName);
        }

        private void OBSWebsocket_Disconnected(object sender, EventArgs e)
        {

        }

        private void OBSWebsocket_Connected(object sender, EventArgs e)
        {

        }

    }
}
