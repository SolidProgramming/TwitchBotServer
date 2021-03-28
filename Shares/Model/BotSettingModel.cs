using Shares.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class BotSettingModel
    {
        public string Id { get; set; }
        public string Channel { get; set; }
        public string TwitchUsername { get; set; }
        public string TwitchOAuth { get; set; }
        public int MessagesAllowedInPeriod { get; set; }
        public int ThrottlingPeriod { get; set; }
        public string SubMessage { get; set; }
        public string ChatDMResponse { get; set; }
        public string ChannelJoinMessage { get; set; }
        public string ChannelLeaveMessage { get; set; }
        public List<SceneModel> Scenes { get; set; } = new List<SceneModel>();
        public List<ChatCommandModel> ChatCommands { get; set; } = new List<ChatCommandModel>();
        public ChatLinkAccessibility ChatLinkAccessibility { get; set; } = ChatLinkAccessibility.Public;
    }
}
