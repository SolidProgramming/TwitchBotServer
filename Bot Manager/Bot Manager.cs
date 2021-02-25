using System;
using System.Collections.Generic;
using System.Text;
using Bot_Manager.Model;
using Newtonsoft.Json;
using Logger;
using Shares.Model;
using Shares;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Client;
using System.Linq;
using TwitchLib.Client.Events;
using System.Threading.Tasks;
using OBSWebsocketController;

namespace Bot_Manager
{
    public static class BotManager
    {
        private static List<TwitchClientExt> Bots = new List<TwitchClientExt>();
        private static OBSWebsocketControllerClient OBSController = new OBSWebsocketControllerClient();

        public static TwitchClientExt CreateBot(BotSettingModel botSetting)
        {
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = botSetting.MessagesAllowedInPeriod,
                ThrottlingPeriod = TimeSpan.FromSeconds(botSetting.ThrottlingPeriod)
            };

            var credentials = new ConnectionCredentials(botSetting.TwitchUsername, botSetting.TwitchOAuth);
            //var customClient = new WebSocketClient(clientOptions);

            if (string.IsNullOrEmpty(botSetting.Id))
            {
                botSetting.Id = Guid.NewGuid().ToString();
            }

            var botClient = new TwitchClientExt()
            {
                BotSetting = botSetting
            };

            botClient.Initialize(credentials, botSetting.Channel);

            Bots.Add(botClient);

            Setting.SaveBotsCredentials(Bots);

            return botClient;
        }
        public static TwitchClientExt CreateBot(TwitchClientExt bot)
        {
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = bot.BotSetting.MessagesAllowedInPeriod,
                ThrottlingPeriod = TimeSpan.FromSeconds(bot.BotSetting.ThrottlingPeriod)
            };

            var credentials = new ConnectionCredentials(bot.BotSetting.TwitchUsername, bot.BotSetting.TwitchOAuth);
            //var customClient = new WebSocketClient(clientOptions);

            bot.Initialize(credentials, bot.BotSetting.Channel);

            Bots.Add(bot);

            Setting.SaveBotsCredentials(Bots);

            return bot;
        }
        public static TwitchClientExt GetBot(string id)
        {
            return Bots.SingleOrDefault(_ => _.BotSetting.Id == id);
        }
        public static List<TwitchClientExt> GetBots()
        {
            if (Bots.Count() == 0)
            {
                return ReadBotSettings();
            }

            return Bots;
        }
        public static void SaveBotsSettings(List<TwitchClientExt> bots)
        {
            Setting.SaveBotsCredentials(bots);
        }
        public static List<TwitchClientExt> ReadBotSettings()
        {
            var tempSettings = Setting.ReadBotClientCredentials();

            Bots.Clear();

            if (tempSettings != null)
            {
                foreach (BotSettingModel botSetting in tempSettings)
                {
                    CreateBot(botSetting);
                }
            }

            return Bots;
        }
        public static async Task StartBot(string botId)
        {
            //TODO: without task.delay
            var bot = Bots.SingleOrDefault(_ => _.BotSetting.Id == botId);
            bot.OnJoinedChannel += TwitchClient_OnJoinedChannel;
            bot.OnConnected += TwitchClient_OnConnected;
            bot.OnConnectionError += TwitchClient_OnConnectionError;
            bot.OnMessageReceived += TwitchClient_OnMessageReceived;
            bot.Status = BotClientStatusModel.AwaitingConnection;
            await Task.Delay(1);            
            bot.Connect();
            bot.Status = BotClientStatusModel.Started;
        }
        public static async Task StopBot(string botId)
        {
            //TODO: without task.delay
            var bot = Bots.Single(_ => _.BotSetting.Id == botId);

            if (!bot.IsConnected || bot.JoinedChannels.Count == 0)
            {
                bot.Status = BotClientStatusModel.Stopped;
                return;
            }

            bot.SendMessage(bot.BotSetting.Channel, bot.BotSetting.ChannelLeaveMessage);
            bot.Status = BotClientStatusModel.AwaitingDisconnect;
            await Task.Delay(1);
            bot.LeaveChannel(bot.BotSetting.Channel);
            bot.Disconnect();
            bot.OnJoinedChannel -= TwitchClient_OnJoinedChannel;
            bot.OnConnected -= TwitchClient_OnConnected;
            bot.OnConnectionError -= TwitchClient_OnConnectionError;
            bot.OnMessageReceived -= TwitchClient_OnMessageReceived;
            bot.Status = BotClientStatusModel.Stopped;
        }
        public static async Task DeleteBot(string botId)
        {
            var bot = Bots.Single(_ => _.BotSetting.Id == botId);

            if (bot.Status != BotClientStatusModel.Stopped)
            {
                await StopBot(botId);
            }

            Bots.Remove(bot);
            Setting.SaveBotsCredentials(Bots);
        }
        private static void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var twitchClient = (TwitchClientExt)sender;
            HandleTwitchMessage(e.ChatMessage.Message, ref twitchClient, e);
        }
        private static void TwitchClient_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine(e.Error);
        }
        private static void TwitchClient_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine(e.BotUsername);
        }
        private static void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            var bot = (TwitchClientExt)sender;
            bot.SendMessage(e.Channel, bot.BotSetting.ChannelJoinMessage);           
        }
        private static void HandleTwitchMessage(string message, ref TwitchClientExt twitchClient, OnMessageReceivedArgs e)
        {
            var chatCommand = new ChatCommandModel();
            var isCommand = IsCommand(message, out chatCommand);

            if (isCommand && chatCommand != ChatCommandModel.none)
            {
                HandleCommand(chatCommand, ref twitchClient, e);
            }
        }
        private static bool IsCommand(string message, out ChatCommandModel chatCommand)
        {
            var command = message.ToChatCommand();

            if (command != ChatCommandModel.none)
            {
                chatCommand = command;
                return true;
            }

            chatCommand = ChatCommandModel.none;
            return false;
        }
        private static void HandleCommand(ChatCommandModel chatCommand, ref TwitchClientExt twitchClient, OnMessageReceivedArgs e)
        {
            switch (chatCommand)
            {
                case ChatCommandModel.none:
                    break;
                case ChatCommandModel.wheel:
                    TriggerSceneSwitch("WOF");
                    break;
                default:
                    break;
            }
        }
        public static void TriggerSceneSwitch(string sceneName)
        {
            OBSController.SwitchToScene(sceneName);
        }
        
    }
}
