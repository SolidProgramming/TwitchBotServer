using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;

namespace Shares.Model
{
    public class TwitchBotModel
    {
        public TwitchClient TwitchClient;
        public TwitchAPI TwitchAPI;
        public BotSettingModel Settings;
        public List<string> Chatters = new();
        public string Id { get; set; }
        public BotClientStatusModel Status { get; set; } = BotClientStatusModel.Stopped;
    }
}
