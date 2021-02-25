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

        public TwitchClientExt GetBot(string id)
        {
            return BotManager.GetBot(id);
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

        public async Task StartBot(string botId)
        {
            await BotManager.StartBot(botId);
        }
        public async Task StopBot(string botId)
        {
            await BotManager.StopBot(botId);
        }
        public void DeleteBot(string botId)
        {
            BotManager.DeleteBot(botId);
        }
    }
}
