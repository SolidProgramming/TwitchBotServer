﻿using Shares;
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
            return Settings.LoadSettings<OBSSettingModel>(FileType.OBSSettings);
        }

        public void SaveOBSSettings(OBSSettingModel obsSettings)
        {
            Settings.SaveSettings(obsSettings, FileType.OBSSettings);
        }
    }
}
