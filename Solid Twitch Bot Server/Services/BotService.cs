using Shares.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot_Manager;

namespace Solid_Twitch_Bot_Server.Services
{
    public class BotService
    {
        //public TwitchClientExt CreateBot(TwitchClientExt botClient)
        //{
        //    return BotManager.CreateBot(botClient);
        //}
        public TwitchBotModel CreateBot(BotSettingModel botSetting)
        {
            return BotManager.CreateBot(botSetting);
        }
        public TwitchBotModel GetBot(string botId)
        {
            return BotManager.GetBot(botId);
        }
        public List<TwitchBotModel> GetBots()
        {
            return BotManager.GetBots(); 
        }
        public List<TwitchBotModel> ReadBotsSettings()
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
        public void StartAutostartOnLiveCheck(TwitchBotModel bot)
        {
            BotManager.StartAutostartOnLiveCheck(bot);
        }
        public void StopAutostartOnLiveCheck(TwitchBotModel bot)
        {
            BotManager.StopAutostartOnLiveCheck(bot);
        }
    }
}
