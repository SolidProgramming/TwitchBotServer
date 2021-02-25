using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot_Manager;
using Bot_Manager.Model;

namespace Solid_Twitch_Bot_Server.Services
{
    public class SceneSwitcherService : ISceneSwitcherService
    {
        public void TriggerSceneSwitch(string sceneName)
        {
            BotManager.TriggerSceneSwitch(sceneName);
        }
    }
}
