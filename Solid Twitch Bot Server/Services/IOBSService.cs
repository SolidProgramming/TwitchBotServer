using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shares.Model;

namespace Solid_Twitch_Bot_Server.Services
{
    public interface IOBSService
    {
        void SaveOBSSettings(OBSSettingModel obsSettings);
        OBSSettingModel ReadOBSSettings();
    }
}
