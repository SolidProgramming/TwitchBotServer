using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace Shares.Model
{
    public class TwitchClientExt : TwitchClient
    {                
        public BotClientStatusModel Status { get; set; } = BotClientStatusModel.Stopped;
        public BotSettingModel BotSetting { get; set; }
    }
}
