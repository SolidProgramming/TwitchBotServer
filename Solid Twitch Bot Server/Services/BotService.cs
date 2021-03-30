using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot_Manager;
using Bot_Manager.Model;

namespace Solid_Twitch_Bot_Server.Services
{
    public class BotService : IBotService
    {
        public TwitchClientExt CreateBot(TwitchClientExt botClient)
        {
            return BotManager.CreateBot(botClient);
        }
        public TwitchClientExt CreateBot(BotSettingModel botSetting)
        {
            return BotManager.CreateBot(botSetting);
        }
        public TwitchClientExt GetBot(string botId)
        {
            return BotManager.GetBot(botId);
        }
        public List<TwitchClientExt> GetBots()
        {
            return BotManager.GetBots(); 
        }
        public List<TwitchClientExt> ReadBotsSettings()
        {
            return BotManager.ReadBotSettings();
        }
        public void SaveBotsSettings()
        {
            BotManager.SaveBotsSettings(GetBots());
        }
        public void SetBotSettings(string botId, BotSettingModel botSetting)
        {
            BotManager.SetBotSettings(botId, botSetting);
        }
        public async Task StartBot(string botId)
        {
            await BotManager.StartBot(botId);
        }
        public async Task StopBot(string botId)
        {
            await BotManager.StopBot(botId);
        }
        public async Task DeleteBot(string botId)
        {
            await BotManager.DeleteBot(botId);
        }
        public BotSettingModel GetBotSettings(string botId)
        {
            return BotManager.GetBotSettings(botId);
        }
    }
}
