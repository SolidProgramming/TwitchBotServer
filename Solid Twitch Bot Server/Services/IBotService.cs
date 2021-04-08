using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solid_Twitch_Bot_Server.Services
{
    public interface IBotService
    {
        List<TwitchBotModel> GetBots();
        TwitchBotModel GetBot(string id);
        void SaveBotsSettings();
        void SetBotSettings(string id, BotSettingModel botSetting);
        List<TwitchBotModel> ReadBotsSettings();
        TwitchBotModel CreateBot(BotSettingModel botSetting);
        Task StartBot(string botId);
        Task StopBot(string botId);
        Task DeleteBot(string botId);
        BotSettingModel GetBotSettings(string botId);
    }
}
