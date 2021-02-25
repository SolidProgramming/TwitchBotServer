using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solid_Twitch_Bot_Server.Services
{
    interface ISceneSwitcherService
    {
        void TriggerSceneSwitch(string sceneName);
    }
}
