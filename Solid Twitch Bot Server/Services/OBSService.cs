using Shares;
using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares.Enum;

namespace Solid_Twitch_Bot_Server.Services
{
    public class OBSService : IOBSService
    {
        public OBSSettingModel ReadOBSSettings()
        {
            return Setting.LoadSettings<OBSSettingModel>(FileType.OBSSettings);
        }

        public void SaveOBSSettings(OBSSettingModel obsSettings)
        {
            Setting.SaveSettings(obsSettings, FileType.OBSSettings);
        }
    }
}
