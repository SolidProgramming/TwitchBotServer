using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Interfaces;

namespace Shares.Model
{
    public class TwitchClientExt : TwitchClient
    {        
        public BotClientStatusModel Status { get; set; } = BotClientStatusModel.Stopped;
        public bool FirstStartDone { get; set; } = false;
        public BotSettingModel BotSetting { get; set; }
    }
}
