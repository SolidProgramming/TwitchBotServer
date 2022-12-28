using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Manager.LiteDB.Models
{
    public class TwitchBotPOCOModel
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public string TwitchUsername { get; set; }
        public string TwitchOAuth { get; set; }
        public string TwitchClientId { get; set; }
        public string StreamelementsJWT { get; set; }
        public int MessagesAllowedInPeriod { get; set; }
        public int ThrottlingPeriod { get; set; }
        public string SubMessage { get; set; }
        public string FollowMessage { get; set; }
        public string ChatDMResponse { get; set; }
        public string ChannelJoinMessage { get; set; }
        public string ChannelLeaveMessage { get; set; }
        public string GreetMessage { get; set; }
        public string ShoutOutText { get; set; }
        public string DonationMessage { get; set; }
        public string CheerMessage { get; set; }
        public string RaidHostMessage { get; set; }
        public bool UseAutostart { get; set; }
    }
}
