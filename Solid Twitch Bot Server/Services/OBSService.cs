using Shares;
using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solid_Twitch_Bot_Server.Services
{
    public class OBSService : IOBSService
    {
        public OBSSettingModel ReadOBSSettings()
        {
            return Setting.ReadOBSSettings();
        }

        public void SaveOBSSettings(OBSSettingModel obsSettings)
        {
            Setting.SaveOBSSettings(obsSettings);
        }
    }
}
