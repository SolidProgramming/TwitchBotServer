using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solid_Twitch_Bot_Server.Services
{
    public interface IBotService
    {
        List<TwitchClientExt> GetBots();
        TwitchClientExt GetBot(string id);
        void SaveBotsSettings();
        List<TwitchClientExt> ReadBotsSettings();
        TwitchClientExt CreateBot(TwitchClientExt botClient);
        TwitchClientExt CreateBot(BotSettingModel botSetting);
        Task StartBot(string botId);
        Task StopBot(string botId);
        void DeleteBot(string botId);
    }
}
