using System;
using System.Collections.Generic;
using Shares.Model;
using Shares;
using Shares.Enum;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using System.Linq;
using TwitchLib.Client.Events;
using System.Threading.Tasks;
using OBSWebsocketController;
using System.Text.RegularExpressions;
using TwitchLib.Api.V5.Models.Users;
using PhilipsHueController;
using StreamElementsNET.Models.Cheer;

namespace Bot_Manager
{
    public static class BotManager
    {
        private static List<TwitchBotModel> Bots = new();
        private static OBSWebsocketControllerClient OBSController;

        public static TwitchBotModel CreateBot(BotSettingModel botSetting)
        {
            var credentials = new ConnectionCredentials(botSetting.TwitchUsername, botSetting.TwitchOAuth);

            var bot = new TwitchBotModel()
            {
                TwitchClient = new(),
                TwitchAPI = new(),
                StreamElementsClient = new(botSetting.StreamelementsJWT),
                Settings = botSetting
            };

            bot.Id = Guid.NewGuid().ToString();

            bot.TwitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
            bot.TwitchClient.OnConnected += TwitchClient_OnConnected;
            bot.TwitchClient.OnConnectionError += TwitchClient_OnConnectionError;
            bot.TwitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;

            bot.StreamElementsClient.OnConnected += StreamElementsClient_OnConnected;
            bot.StreamElementsClient.OnAuthenticated += StreamElementsClient_OnAuthenticated;
            bot.StreamElementsClient.OnAuthenticationFailure += StreamElementsClient_OnAuthenticationFailure;
            bot.StreamElementsClient.OnReceivedRawMessage += StreamElementsClient_OnReceivedRawMessage;
            bot.StreamElementsClient.OnSent += StreamElementsClient_OnSent;
            bot.StreamElementsClient.OnFollower += (sender, e) => StreamElementsClient_OnFollower(sender, e, bot.Id);
            bot.StreamElementsClient.OnSubscriber += (sender, e) => StreamElementsClient_OnSubscriber(sender, e, bot.Id);
            bot.StreamElementsClient.OnTip += (sender, e) => StreamElementsClient_OnTip(sender, e, bot.Id);
            bot.StreamElementsClient.OnCheer += (sender, e) => StreamElementsClient_OnCheer(sender, e, bot.Id);

            bot.TwitchAPI.V5.Settings.AccessToken = bot.Settings.TwitchOAuth;
            bot.TwitchAPI.V5.Settings.ClientId = bot.Settings.TwitchClientId;

            bot.TwitchAPI.Settings.AccessToken = bot.Settings.TwitchOAuth;
            bot.TwitchAPI.Settings.ClientId = bot.Settings.TwitchClientId;

            bot.TwitchClient.Initialize(credentials, botSetting.Channel);           

            Bots.Add(bot);

            SettingsHandler.SaveSettings(Bots.Select(_ => _.Settings).ToList(), FileType.BotSettings);

            return bot;
        }
        public static TwitchBotModel CreateBot(TwitchBotModel bot)
        {
            var credentials = new ConnectionCredentials(bot.Settings.TwitchUsername, bot.Settings.TwitchOAuth);

            bot = new TwitchBotModel
            {
                TwitchClient = new(),
                TwitchAPI = new(),
                StreamElementsClient = new(bot.Settings.StreamelementsJWT)
            };

            bot.TwitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
            bot.TwitchClient.OnConnected += TwitchClient_OnConnected;
            bot.TwitchClient.OnConnectionError += TwitchClient_OnConnectionError;
            bot.TwitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;

            bot.StreamElementsClient.OnConnected += StreamElementsClient_OnConnected;
            bot.StreamElementsClient.OnFollower += (sender, e) => StreamElementsClient_OnFollower(sender, e, bot.Id);
            bot.StreamElementsClient.OnAuthenticated += StreamElementsClient_OnAuthenticated;
            bot.StreamElementsClient.OnAuthenticationFailure += StreamElementsClient_OnAuthenticationFailure;
            bot.StreamElementsClient.OnReceivedRawMessage += StreamElementsClient_OnReceivedRawMessage;
            bot.StreamElementsClient.OnSent += StreamElementsClient_OnSent;
            bot.StreamElementsClient.OnSubscriber += (sender, e) => StreamElementsClient_OnSubscriber(sender, e, bot.Id);
            bot.StreamElementsClient.OnTip += (sender, e) => StreamElementsClient_OnTip(sender, e, bot.Id);
            bot.StreamElementsClient.OnCheer += (sender, e) => StreamElementsClient_OnCheer(sender, e, bot.Id);

            bot.TwitchAPI.V5.Settings.AccessToken = bot.Settings.TwitchOAuth;
            bot.TwitchAPI.V5.Settings.ClientId = bot.Settings.TwitchClientId;

            bot.TwitchAPI.Settings.AccessToken = bot.Settings.TwitchOAuth;
            bot.TwitchAPI.Settings.ClientId = bot.Settings.TwitchClientId;

            bot.TwitchClient.Initialize(credentials, bot.Settings.Channel);

            Bots.Add(bot);

            SettingsHandler.SaveSettings(Bots.Select(_ => _.Settings).ToList(), FileType.BotSettings);

            return bot;
        }
        public static TwitchBotModel GetBot(string id)
        {
            return Bots.SingleOrDefault(_ => _.Id == id);
        }
        public static BotSettingModel GetBotSettings(string botId)
        {
            return Bots.SingleOrDefault(_ => _.Id == botId).Settings;
        }
        public static List<TwitchBotModel> GetBots()
        {
            if (Bots.Count == 0)
            {
                return ReadBotSettings();
            }

            return Bots;
        }
        public static void SaveBotsSettings(List<TwitchBotModel> bots)
        {
            SettingsHandler.SaveSettings(bots.Select(_ => _.Settings).ToList(), Shares.Enum.FileType.BotSettings);
        }
        public static void SetBotSettings(string id, BotSettingModel botSetting)
        {
            var bot = Bots.SingleOrDefault(_ => _.Id == id);

            bot.Settings = botSetting;
        }
        public static List<TwitchBotModel> ReadBotSettings()
        {
            List<BotSettingModel> tempSettings = SettingsHandler.LoadSettings<List<BotSettingModel>>(Shares.Enum.FileType.BotSettings);

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
            var bot = Bots.SingleOrDefault(_ => _.Id == botId);

            bot.Status = BotClientStatusModel.AwaitingConnection;

            await Task.Delay(1);

            bot.TwitchClient.Connect();

            if (!string.IsNullOrEmpty(bot.Settings.StreamelementsJWT))
            {
                bot.StreamElementsClient.Connect();
            }

            bot.Status = BotClientStatusModel.Started;
        }
        public static async Task StopBot(string botId)
        {
            //TODO: without task.delay
            var bot = Bots.Single(_ => _.Id == botId);

            if (!bot.TwitchClient.IsConnected || bot.TwitchClient.JoinedChannels.Count == 0)
            {
                bot.Status = BotClientStatusModel.Stopped;
                return;
            }

            bot.TwitchClient.SendMessage(bot.Settings.Channel, bot.Settings.ChannelLeaveMessage);
            bot.Status = BotClientStatusModel.AwaitingDisconnect;
            await Task.Delay(1);
            bot.TwitchClient.LeaveChannel(bot.Settings.Channel);
            bot.TwitchClient.Disconnect();
            bot.Status = BotClientStatusModel.Stopped;
        }
        public static async Task DeleteBot(string botId)
        {
            var bot = Bots.Single(_ => _.Id == botId);

            if (bot.Status != BotClientStatusModel.Stopped)
            {
                await StopBot(botId);
            }

            Bots.Remove(bot);
            SettingsHandler.SaveSettings(Bots.Select(_ => _.Settings).ToList(), FileType.BotSettings);
        }
        private static void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            HandleTwitchMessageAsync((TwitchClient)sender, e);
        }
        private static void TwitchClient_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine(e.Error.Message);
        }
        private static void TwitchClient_OnConnected(object sender, OnConnectedArgs e)
        {

        }
        private static void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            var bot = Bots.SingleOrDefault(_ => _.TwitchClient == (TwitchClient)sender);
            bot.TwitchClient.SendMessage(e.Channel, bot.Settings.ChannelJoinMessage);
        }
        private static void StreamElementsClient_OnFollower(object sender, StreamElementsNET.Models.Follower.Follower e, string botId)
        {
            var bot = Bots.Single(_ => _.Id == botId);
            HandleNewFollower(bot, e);
        }
        private static void StreamElementsClient_OnSubscriber(object sender, StreamElementsNET.Models.Subscriber.Subscriber e, string botId)
        {
            var bot = Bots.Single(_ => _.Id == botId);
            HandleNewSubscriber(bot, e);
        }
        private static void StreamElementsClient_OnTip(object sender, StreamElementsNET.Models.Tip.Tip e, string botId)
        {
            var bot = Bots.Single(_ => _.Id == botId);
            HandleNewTip(bot, e);
        }
        private static void StreamElementsClient_OnCheer(object sender, StreamElementsNET.Models.Cheer.Cheer e, string botId)
        {
            var bot = Bots.Single(_ => _.Id == botId);
            HandleNewCheer(bot, e);
        }
        private static void StreamElementsClient_OnAuthenticationFailure(object sender, EventArgs e)
        {
            Console.WriteLine($"Failed to login! Invalid JWT token!");
        }
        private static void StreamElementsClient_OnAuthenticated(object sender, StreamElementsNET.Models.Internal.Authenticated e)
        {
            Console.WriteLine($"AUTHENTICATED: {e.ChannelId}");
        }
        private static void StreamElementsClient_OnConnected(object sender, EventArgs e)
        {

        }
        private static void StreamElementsClient_OnReceivedRawMessage(object sender, string e)
        {
            Console.WriteLine($"RECEIVED: {e}");
        }
        private static void StreamElementsClient_OnSent(object sender, string e)
        {
            Console.WriteLine($"SENT: {e}");
        }
        private static void HandleTwitchMessageAsync(TwitchClient twitchClient, OnMessageReceivedArgs e)
        {
            //TODO: refactor
            string chatterUsername = e.ChatMessage.DisplayName;
            string channelName = e.ChatMessage.Channel;
            string userId = e.ChatMessage.UserId;
            string chatMessageText = e.ChatMessage.Message;

            var bot = Bots.SingleOrDefault(_ => _.TwitchClient == twitchClient);

            if (!bot.Chatters.Contains(chatterUsername) && (!e.ChatMessage.IsMe && !e.ChatMessage.IsBroadcaster))
            {
                bot.Chatters.Add(chatterUsername);
                GreetChatter(bot, e.ChatMessage);
            }
            HandleLinkPosting(ref bot, e.ChatMessage);
            HandleCommand(ref bot, e.ChatMessage);

        }
        private static void HandleNewSubscriber(TwitchBotModel bot, StreamElementsNET.Models.Subscriber.Subscriber e)
        {
            if (string.IsNullOrWhiteSpace(bot.Settings.SubMessage)) return;

            string channel = bot.Settings.Channel;

            bot.TwitchClient.SendMessage(channel, bot.Settings.SubMessage.ToCustomTextWithParameter(e));
        }
        private static void HandleNewFollower(TwitchBotModel bot, StreamElementsNET.Models.Follower.Follower e)
        {
            if (string.IsNullOrWhiteSpace(bot.Settings.FollowMessage)) return;

            string channel = bot.Settings.Channel;

            bot.TwitchClient.SendMessage(channel, bot.Settings.FollowMessage.ToCustomTextWithParameter(e));
        }
        private static void HandleNewTip(TwitchBotModel bot, StreamElementsNET.Models.Tip.Tip e)
        {
            if (string.IsNullOrWhiteSpace(bot.Settings.DonationMessage)) return;

            string channel = bot.Settings.Channel;

            bot.TwitchClient.SendMessage(channel, bot.Settings.DonationMessage.ToCustomTextWithParameter(e));
        }
        private static void HandleNewCheer(TwitchBotModel bot, Cheer e)
        {
            if (string.IsNullOrWhiteSpace(bot.Settings.CheerMessage)) return;

            string channel = bot.Settings.Channel;

            bot.TwitchClient.SendMessage(channel, bot.Settings.CheerMessage.ToCustomTextWithParameter(e));
        }
        private static void HandleCommand(ref TwitchBotModel bot, ChatMessage chatMessage)
        {
            string chatMessageText = chatMessage.Message;

            if (chatMessageText.IsCommand(out Shares.Enum.ChatCommand? chatCommand))
            {
                switch (chatCommand)
                {
                    case Shares.Enum.ChatCommand.ShoutOut:
                        HandleShoutout(bot, chatMessage);
                        break;
                    default:
                        break;
                }
            }
        }
        private static async void HandleShoutout(TwitchBotModel bot, ChatMessage chatMessage)
        {
            if (string.IsNullOrEmpty(bot.Settings.ShoutOutText)) return;

            string channelName = bot.Settings.Channel;
            string userId = chatMessage.UserId;
            string username = chatMessage.Username;

            string customUsername = chatMessage.Message[(chatMessage.Message.IndexOf(' ') + 1)..].Replace("@", string.Empty);

            CustomUserModel customUserModel = new()
            {
                Username = customUsername
            };

            if (await TwitchUserExistsAsync(bot, customUserModel.Username))
            {
                bot.TwitchClient.SendMessage(channelName, bot.Settings.ShoutOutText.ToCustomTextWithParameter(customUserModel));
            }
            else
            {
                bot.TwitchClient.SendReply(channelName, userId, "Dieser Benutzer existiert nicht.");
            }
        }
        private static void HandleLinkPosting(ref TwitchBotModel bot, ChatMessage chatMessage)
        {
            string chatterUsername = chatMessage.DisplayName;
            string channelName = chatMessage.Channel;
            string chatMessageText = chatMessage.Message;

            if (IsValidUrl(chatMessageText))
            {
                if (bot.Settings.ChatLinkAccessibility == ChatLinkAccessibility.Private)
                {
                    if (!chatMessage.IsVip && !chatMessage.IsModerator && !chatMessage.IsMe && !chatMessage.IsBroadcaster && !chatMessage.IsStaff)
                    {
                        switch (bot.Settings.ChatLinkAction)
                        {
                            case ChatLinkAction.BanUser:
                                TimeoutUser(ref bot.TwitchClient, channelName, chatterUsername, (int)TimeSpan.FromMinutes(15).TotalSeconds);
                                break;
                            case ChatLinkAction.DeleteMessage:
                                TimeoutUser(ref bot.TwitchClient, channelName, chatterUsername, 1);
                                break;
                            case ChatLinkAction.Warning:
                                WarnUserForLinkPosting(ref bot.TwitchClient, channelName, chatterUsername);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        private static bool IsValidUrl(string url)
        {
            string pattern = @"(www.|[a-zA-Z].)[a-zA-Z0-9\-\.]+\.(AAA|AARP|ABARTH|ABB|ABBOTT|ABBVIE|ABC|ABLE|ABOGADO|ABUDHABI|AC|ACADEMY|ACCENTURE|ACCOUNTANT|ACCOUNTANTS|ACO|ACTIVE|ACTOR|AD|ADAC|ADS|ADULT|AE|AEG|AERO|AETNA|AF|AFAMILYCOMPANY|AFL|AFRICA|AG|AGAKHAN|AGENCY|AI|AIG|AIGO|AIRBUS|AIRFORCE|AIRTEL|AKDN|AL|ALFAROMEO|ALIBABA|ALIPAY|ALLFINANZ|ALLSTATE|ALLY|ALSACE|ALSTOM|AM|AMERICANEXPRESS|AMERICANFAMILY|AMEX|AMFAM|AMICA|AMSTERDAM|ANALYTICS|ANDROID|ANQUAN|ANZ|AO|AOL|APARTMENTS|APP|APPLE|AQ|AQUARELLE|AR|ARAB|ARAMCO|ARCHI|ARMY|ARPA|ART|ARTE|AS|ASDA|ASIA|ASSOCIATES|AT|ATHLETA|ATTORNEY|AU|AUCTION|AUDI|AUDIBLE|AUDIO|AUSPOST|AUTHOR|AUTO|AUTOS|AVIANCA|AW|AWS|AX|AXA|AZ|AZURE|BA|BABY|BAIDU|BANAMEX|BANANAREPUBLIC|BAND|BANK|BAR|BARCELONA|BARCLAYCARD|BARCLAYS|BAREFOOT|BARGAINS|BASEBALL|BASKETBALL|BAUHAUS|BAYERN|BB|BBC|BBT|BBVA|BCG|BCN|BD|BE|BEATS|BEAUTY|BEER|BENTLEY|BERLIN|BEST|BESTBUY|BET|BF|BG|BH|BHARTI|BI|BIBLE|BID|BIKE|BING|BINGO|BIO|BIZ|BJ|BLACK|BLACKFRIDAY|BLANCO|BLOCKBUSTER|BLOG|BLOOMBERG|BLUE|BM|BMS|BMW|BN|BNL|BNPPARIBAS|BO|BOATS|BOEHRINGER|BOFA|BOM|BOND|BOO|BOOK|BOOKING|BOSCH|BOSTIK|BOSTON|BOT|BOUTIQUE|BOX|BR|BRADESCO|BRIDGESTONE|BROADWAY|BROKER|BROTHER|BRUSSELS|BS|BT|BUDAPEST|BUGATTI|BUILD|BUILDERS|BUSINESS|BUY|BUZZ|BV|BW|BY|BZ|BZH|CA|CAB|CAFE|CAL|CALL|CALVINKLEIN|CAM|CAMERA|CAMP|CANCERRESEARCH|CANON|CAPETOWN|CAPITAL|CAPITALONE|CAR|CARAVAN|CARDS|CARE|CAREER|CAREERS|CARS|CARTIER|CASA|CASE|CASEIH|CASH|CASINO|CAT|CATERING|CATHOLIC|CBA|CBN|CBRE|CBS|CC|CD|CEB|CENTER|CEO|CERN|CF|CFA|CFD|CG|CH|CHANEL|CHANNEL|CHARITY|CHASE|CHAT|CHEAP|CHINTAI|CHRISTMAS|CHROME|CHRYSLER|CHURCH|CI|CIPRIANI|CIRCLE|CISCO|CITADEL|CITI|CITIC|CITY|CITYEATS|CK|CL|CLAIMS|CLEANING|CLICK|CLINIC|CLINIQUE|CLOTHING|CLOUD|CLUB|CLUBMED|CM|CN|CO|COACH|CODES|COFFEE|COLLEGE|COLOGNE|COM|COMCAST|COMMBANK|COMMUNITY|COMPANY|COMPARE|COMPUTER|COMSEC|CONDOS|CONSTRUCTION|CONSULTING|CONTACT|CONTRACTORS|COOKING|COOKINGCHANNEL|COOL|COOP|CORSICA|COUNTRY|COUPON|COUPONS|COURSES|CR|CREDIT|CREDITCARD|CREDITUNION|CRICKET|CROWN|CRS|CRUISE|CRUISES|CSC|CU|CUISINELLA|CV|CW|CX|CY|CYMRU|CYOU|CZ|DABUR|DAD|DANCE|DATA|DATE|DATING|DATSUN|DAY|DCLK|DDS|DE|DEAL|DEALER|DEALS|DEGREE|DELIVERY|DELL|DELOITTE|DELTA|DEMOCRAT|DENTAL|DENTIST|DESI|DESIGN|DEV|DHL|DIAMONDS|DIET|DIGITAL|DIRECT|DIRECTORY|DISCOUNT|DISCOVER|DISH|DIY|DJ|DK|DM|DNP|DO|DOCS|DOCTOR|DODGE|DOG|DOHA|DOMAINS|DOT|DOWNLOAD|DRIVE|DTV|DUBAI|DUCK|DUNLOP|DUNS|DUPONT|DURBAN|DVAG|DVR|DZ|EARTH|EAT|EC|ECO|EDEKA|EDU|EDUCATION|EE|EG|EMAIL|EMERCK|ENERGY|ENGINEER|ENGINEERING|ENTERPRISES|EPOST|EPSON|EQUIPMENT|ER|ERICSSON|ERNI|ES|ESQ|ESTATE|ESURANCE|ET|ETISALAT|EU|EUROVISION|EUS|EVENTS|EVERBANK|EXCHANGE|EXPERT|EXPOSED|EXPRESS|EXTRASPACE|FAGE|FAIL|FAIRWINDS|FAITH|FAMILY|FAN|FANS|FARM|FARMERS|FASHION|FAST|FEDEX|FEEDBACK|FERRARI|FERRERO|FI|FIAT|FIDELITY|FIDO|FILM|FINAL|FINANCE|FINANCIAL|FIRE|FIRESTONE|FIRMDALE|FISH|FISHING|FIT|FITNESS|FJ|FK|FLICKR|FLIGHTS|FLIR|FLORIST|FLOWERS|FLY|FM|FO|FOO|FOOD|FOODNETWORK|FOOTBALL|FORD|FOREX|FORSALE|FORUM|FOUNDATION|FOX|FR|FREE|FRESENIUS|FRL|FROGANS|FRONTDOOR|FRONTIER|FTR|FUJITSU|FUJIXEROX|FUN|FUND|FURNITURE|FUTBOL|FYI|GA|GAL|GALLERY|GALLO|GALLUP|GAME|GAMES|GAP|GARDEN|GB|GBIZ|GD|GDN|GE|GEA|GENT|GENTING|GEORGE|GF|GG|GGEE|GH|GI|GIFT|GIFTS|GIVES|GIVING|GL|GLADE|GLASS|GLE|GLOBAL|GLOBO|GM|GMAIL|GMBH|GMO|GMX|GN|GODADDY|GOLD|GOLDPOINT|GOLF|GOO|GOODYEAR|GOOG|GOOGLE|GOP|GOT|GOV|GP|GQ|GR|GRAINGER|GRAPHICS|GRATIS|GREEN|GRIPE|GROCERY|GROUP|GS|GT|GU|GUARDIAN|GUCCI|GUGE|GUIDE|GUITARS|GURU|GW|GY|HAIR|HAMBURG|HANGOUT|HAUS|HBO|HDFC|HDFCBANK|HEALTH|HEALTHCARE|HELP|HELSINKI|HERE|HERMES|HGTV|HIPHOP|HISAMITSU|HITACHI|HIV|HK|HKT|HM|HN|HOCKEY|HOLDINGS|HOLIDAY|HOMEDEPOT|HOMEGOODS|HOMES|HOMESENSE|HONDA|HONEYWELL|HORSE|HOSPITAL|HOST|HOSTING|HOT|HOTELES|HOTELS|HOTMAIL|HOUSE|HOW|HR|HSBC|HT|HU|HUGHES|HYATT|HYUNDAI|IBM|ICBC|ICE|ICU|ID|IE|IEEE|IFM|IKANO|IL|IM|IMAMAT|IMDB|IMMO|IMMOBILIEN|IN|INC|INDUSTRIES|INFINITI|INFO|ING|INK|INSTITUTE|INSURANCE|INSURE|INT|INTEL|INTERNATIONAL|INTUIT|INVESTMENTS|IO|IPIRANGA|IQ|IR|IRISH|IS|ISELECT|ISMAILI|IST|ISTANBUL|IT|ITAU|ITV|IVECO|JAGUAR|JAVA|JCB|JCP|JE|JEEP|JETZT|JEWELRY|JIO|JLL|JM|JMP|JNJ|JO|JOBS|JOBURG|JOT|JOY|JP|JPMORGAN|JPRS|JUEGOS|JUNIPER|KAUFEN|KDDI|KE|KERRYHOTELS|KERRYLOGISTICS|KERRYPROPERTIES|KFH|KG|KH|KI|KIA|KIM|KINDER|KINDLE|KITCHEN|KIWI|KM|KN|KOELN|KOMATSU|KOSHER|KP|KPMG|KPN|KR|KRD|KRED|KUOKGROUP|KW|KY|KYOTO|KZ|LA|LACAIXA|LADBROKES|LAMBORGHINI|LAMER|LANCASTER|LANCIA|LANCOME|LAND|LANDROVER|LANXESS|LASALLE|LAT|LATINO|LATROBE|LAW|LAWYER|LB|LC|LDS|LEASE|LECLERC|LEFRAK|LEGAL|LEGO|LEXUS|LGBT|LI|LIAISON|LIDL|LIFE|LIFEINSURANCE|LIFESTYLE|LIGHTING|LIKE|LILLY|LIMITED|LIMO|LINCOLN|LINDE|LINK|LIPSY|LIVE|LIVING|LIXIL|LK|LLC|LOAN|LOANS|LOCKER|LOCUS|LOFT|LOL|LONDON|LOTTE|LOTTO|LOVE|LPL|LPLFINANCIAL|LR|LS|LT|LTD|LTDA|LU|LUNDBECK|LUPIN|LUXE|LUXURY|LV|LY|MA|MACYS|MADRID|MAIF|MAISON|MAKEUP|MAN|MANAGEMENT|MANGO|MAP|MARKET|MARKETING|MARKETS|MARRIOTT|MARSHALLS|MASERATI|MATTEL|MBA|MC|MCKINSEY|MD|ME|MED|MEDIA|MEET|MELBOURNE|MEME|MEMORIAL|MEN|MENU|MERCKMSD|METLIFE|MG|MH|MIAMI|MICROSOFT|MIL|MINI|MINT|MIT|MITSUBISHI|MK|ML|MLB|MLS|MM|MMA|MN|MO|MOBI|MOBILE|MOBILY|MODA|MOE|MOI|MOM|MONASH|MONEY|MONSTER|MOPAR|MORMON|MORTGAGE|MOSCOW|MOTO|MOTORCYCLES|MOV|MOVIE|MOVISTAR|MP|MQ|MR|MS|MSD|MT|MTN|MTR|MU|MUSEUM|MUTUAL|MV|MW|MX|MY|MZ|NA|NAB|NADEX|NAGOYA|NAME|NATIONWIDE|NATURA|NAVY|NBA|NC|NE|NEC|NET|NETBANK|NETFLIX|NETWORK|NEUSTAR|NEW|NEWHOLLAND|NEWS|NEXT|NEXTDIRECT|NEXUS|NF|NFL|NG|NGO|NHK|NI|NICO|NIKE|NIKON|NINJA|NISSAN|NISSAY|NL|NO|NOKIA|NORTHWESTERNMUTUAL|NORTON|NOW|NOWRUZ|NOWTV|NP|NR|NRA|NRW|NTT|NU|NYC|NZ|OBI|OBSERVER|OFF|OFFICE|OKINAWA|OLAYAN|OLAYANGROUP|OLDNAVY|OLLO|OM|OMEGA|ONE|ONG|ONL|ONLINE|ONYOURSIDE|OOO|OPEN|ORACLE|ORANGE|ORG|ORGANIC|ORIGINS|OSAKA|OTSUKA|OTT|OVH|PA|PAGE|PANASONIC|PARIS|PARS|PARTNERS|PARTS|PARTY|PASSAGENS|PAY|PCCW|PE|PET|PF|PFIZER|PG|PH|PHARMACY|PHD|PHILIPS|PHONE|PHOTO|PHOTOGRAPHY|PHOTOS|PHYSIO|PIAGET|PICS|PICTET|PICTURES|PID|PIN|PING|PINK|PIONEER|PIZZA|PK|PL|PLACE|PLAY|PLAYSTATION|PLUMBING|PLUS|PM|PN|PNC|POHL|POKER|POLITIE|PORN|POST|PR|PRAMERICA|PRAXI|PRESS|PRIME|PRO|PROD|PRODUCTIONS|PROF|PROGRESSIVE|PROMO|PROPERTIES|PROPERTY|PROTECTION|PRU|PRUDENTIAL|PS|PT|PUB|PW|PWC|PY|QA|QPON|QUEBEC|QUEST|QVC|RACING|RADIO|RAID|RE|READ|REALESTATE|REALTOR|REALTY|RECIPES|RED|REDSTONE|REDUMBRELLA|REHAB|REISE|REISEN|REIT|RELIANCE|REN|RENT|RENTALS|REPAIR|REPORT|REPUBLICAN|REST|RESTAURANT|REVIEW|REVIEWS|REXROTH|RICH|RICHARDLI|RICOH|RIGHTATHOME|RIL|RIO|RIP|RMIT|RO|ROCHER|ROCKS|RODEO|ROGERS|ROOM|RS|RSVP|RU|RUGBY|RUHR|RUN|RW|RWE|RYUKYU|SA|SAARLAND|SAFE|SAFETY|SAKURA|SALE|SALON|SAMSCLUB|SAMSUNG|SANDVIK|SANDVIKCOROMANT|SANOFI|SAP|SARL|SAS|SAVE|SAXO|SB|SBI|SBS|SC|SCA|SCB|SCHAEFFLER|SCHMIDT|SCHOLARSHIPS|SCHOOL|SCHULE|SCHWARZ|SCIENCE|SCJOHNSON|SCOR|SCOT|SD|SE|SEARCH|SEAT|SECURE|SECURITY|SEEK|SELECT|SENER|SERVICES|SES|SEVEN|SEW|SEX|SEXY|SFR|SG|SH|SHANGRILA|SHARP|SHAW|SHELL|SHIA|SHIKSHA|SHOES|SHOP|SHOPPING|SHOUJI|SHOW|SHOWTIME|SHRIRAM|SI|SILK|SINA|SINGLES|SITE|SJ|SK|SKI|SKIN|SKY|SKYPE|SL|SLING|SM|SMART|SMILE|SN|SNCF|SO|SOCCER|SOCIAL|SOFTBANK|SOFTWARE|SOHU|SOLAR|SOLUTIONS|SONG|SONY|SOY|SPACE|SPORT|SPOT|SPREADBETTING|SR|SRL|SRT|ST|STADA|STAPLES|STAR|STARHUB|STATEBANK|STATEFARM|STC|STCGROUP|STOCKHOLM|STORAGE|STORE|STREAM|STUDIO|STUDY|STYLE|SU|SUCKS|SUPPLIES|SUPPLY|SUPPORT|SURF|SURGERY|SUZUKI|SV|SWATCH|SWIFTCOVER|SWISS|SX|SY|SYDNEY|SYMANTEC|SYSTEMS|SZ|TAB|TAIPEI|TALK|TAOBAO|TARGET|TATAMOTORS|TATAR|TATTOO|TAX|TAXI|TC|TCI|TD|TDK|TEAM|TECH|TECHNOLOGY|TEL|TELEFONICA|TEMASEK|TENNIS|TEVA|TF|TG|TH|THD|THEATER|THEATRE|TIAA|TICKETS|TIENDA|TIFFANY|TIPS|TIRES|TIROL|TJ|TJMAXX|TJX|TK|TKMAXX|TL|TM|TMALL|TN|TO|TODAY|TOKYO|TOOLS|TOP|TORAY|TOSHIBA|TOTAL|TOURS|TOWN|TOYOTA|TOYS|TR|TRADE|TRADING|TRAINING|TRAVEL|TRAVELCHANNEL|TRAVELERS|TRAVELERSINSURANCE|TRUST|TRV|TT|TUBE|TUI|TUNES|TUSHU|TV|TVS|TW|TZ|UA|UBANK|UBS|UCONNECT|UG|UK|UNICOM|UNIVERSITY|UNO|UOL|UPS|US|UY|UZ|VA|VACATIONS|VANA|VANGUARD|VC|VE|VEGAS|VENTURES|VERISIGN|VERSICHERUNG|VET|VG|VI|VIAJES|VIDEO|VIG|VIKING|VILLAS|VIN|VIP|VIRGIN|VISA|VISION|VISTAPRINT|VIVA|VIVO|VLAANDEREN|VN|VODKA|VOLKSWAGEN|VOLVO|VOTE|VOTING|VOTO|VOYAGE|VU|VUELOS|WALES|WALMART|WALTER|WANG|WANGGOU|WARMAN|WATCH|WATCHES|WEATHER|WEATHERCHANNEL|WEBCAM|WEBER|WEBSITE|WED|WEDDING|WEIBO|WEIR|WF|WHOSWHO|WIEN|WIKI|WILLIAMHILL|WIN|WINDOWS|WINE|WINNERS|WME|WOLTERSKLUWER|WOODSIDE|WORK|WORKS|WORLD|WOW|WS|WTC|WTF|XBOX|XEROX|XFINITY|XIHUAN|XIN|XN--11B4C3D|XN--1CK2E1B|XN--1QQW23A|XN--2SCRJ9C|XN--30RR7Y|XN--3BST00M|XN--3DS443G|XN--3E0B707E|XN--3HCRJ9C|XN--3OQ18VL8PN36A|XN--3PXU8K|XN--42C2D9A|XN--45BR5CYL|XN--45BRJ9C|XN--45Q11C|XN--4GBRIM|XN--54B7FTA0CC|XN--55QW42G|XN--55QX5D|XN--5SU34J936BGSG|XN--5TZM5G|XN--6FRZ82G|XN--6QQ986B3XL|XN--80ADXHKS|XN--80AO21A|XN--80AQECDR1A|XN--80ASEHDB|XN--80ASWG|XN--8Y0A063A|XN--90A3AC|XN--90AE|XN--90AIS|XN--9DBQ2A|XN--9ET52U|XN--9KRT00A|XN--B4W605FERD|XN--BCK1B9A5DRE4C|XN--C1AVG|XN--C2BR7G|XN--CCK2B3B|XN--CG4BKI|XN--CLCHC0EA0B2G2A9GCD|XN--CZR694B|XN--CZRS0T|XN--CZRU2D|XN--D1ACJ3B|XN--D1ALF|XN--E1A4C|XN--ECKVDTC9D|XN--EFVY88H|XN--ESTV75G|XN--FCT429K|XN--FHBEI|XN--FIQ228C5HS|XN--FIQ64B|XN--FIQS8S|XN--FIQZ9S|XN--FJQ720A|XN--FLW351E|XN--FPCRJ9C3D|XN--FZC2C9E2C|XN--FZYS8D69UVGM|XN--G2XX48C|XN--GCKR3F0F|XN--GECRJ9C|XN--GK3AT1E|XN--H2BREG3EVE|XN--H2BRJ9C|XN--H2BRJ9C8C|XN--HXT814E|XN--I1B6B1A6A2E|XN--IMR513N|XN--IO0A7I|XN--J1AEF|XN--J1AMH|XN--J6W193G|XN--JLQ61U9W7B|XN--JVR189M|XN--KCRX77D1X4A|XN--KPRW13D|XN--KPRY57D|XN--KPU716F|XN--KPUT3I|XN--L1ACC|XN--LGBBAT1AD8J|XN--MGB9AWBF|XN--MGBA3A3EJT|XN--MGBA3A4F16A|XN--MGBA7C0BBN0A|XN--MGBAAKC7DVF|XN--MGBAAM7A8H|XN--MGBAB2BD|XN--MGBAI9AZGQP6J|XN--MGBAYH7GPA|XN--MGBB9FBPOB|XN--MGBBH1A|XN--MGBBH1A71E|XN--MGBC0A9AZCG|XN--MGBCA7DZDO|XN--MGBERP4A5D4AR|XN--MGBGU82A|XN--MGBI4ECEXP|XN--MGBPL2FH|XN--MGBT3DHD|XN--MGBTX2B|XN--MGBX4CD0AB|XN--MIX891F|XN--MK1BU44C|XN--MXTQ1M|XN--NGBC5AZD|XN--NGBE9E0A|XN--NGBRX|XN--NODE|XN--NQV7F|XN--NQV7FS00EMA|XN--NYQY26A|XN--O3CW4H|XN--OGBPF8FL|XN--OTU796D|XN--P1ACF|XN--P1AI|XN--PBT977C|XN--PGBS0DH|XN--PSSY2U|XN--Q9JYB4C|XN--QCKA1PMC|XN--QXAM|XN--RHQV96G|XN--ROVU88B|XN--RVC1E0AM3E|XN--S9BRJ9C|XN--SES554G|XN--T60B56A|XN--TCKWE|XN--TIQ49XQYJ|XN--UNUP4Y|XN--VERMGENSBERATER-CTB|XN--VERMGENSBERATUNG-PWB|XN--VHQUV|XN--VUQ861B|XN--W4R85EL8FHU5DNRA|XN--W4RS40L|XN--WGBH1C|XN--WGBL6A|XN--XHQ521B|XN--XKC2AL3HYE2A|XN--XKC2DL3A5EE0H|XN--Y9A3AQ|XN--YFRO4I67O|XN--YGBI2AMMX|XN--ZFR164B|XXX|XYZ|YACHTS|YAHOO|YAMAXUN|YANDEX|YE|YODOBASHI|YOGA|YOKOHAMA|YOU|YOUTUBE|YT|YUN|ZA|ZAPPOS|ZARA|ZERO|ZIP|ZIPPO|ZM|ZONE|ZUERICH|ZW)(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\;\?\'\\\+&amp;%\$#\=~_\-]+))*$";
            //string pattern = @" ^ (http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new (pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);
        }
        public static void TriggerSceneSwitch(string sceneName)
        {
            OBSController.SwitchToScene(sceneName);
        }
        private static void TimeoutUser(ref TwitchClient twitchClient, string channelName, string username, int duration, string reason = "")
        {
            twitchClient.SendMessage(channelName, $".timeout {username} {duration} {reason}");
        }

        //TODO: .timeout for link delete with bool = deletelink
        private static void WarnUserForLinkPosting(ref TwitchClient twitchClient, string channelName, string chatterUsername)
        {
            twitchClient.SendMessage(channelName, $"Bitte keine Links posten");
        }
        private static async Task<bool> TwitchUserExistsAsync(TwitchBotModel twitchBot, string username)
        {
            Users user = await twitchBot.TwitchAPI.V5.Users.GetUserByNameAsync(username);

            if (user.Total > 0)
                return true;

            return false;
        }
        private static void GreetChatter(TwitchBotModel twitchBot, ChatMessage chat)
        {
            if (twitchBot.Settings.GreetMessage.Length == 0) return;
            twitchBot.TwitchClient.SendMessage(chat.Channel, twitchBot.Settings.GreetMessage.ToCustomTextWithParameter(chat));
        }
        public static async Task TestPhilipsHueController()
        {
            PhilipsHueControllerClient hueClient = new()
            {
                AppName = "",
                DeviceName = ""
            };

            await hueClient.RegisterClient("127.0.0.1", PhilipsHueBridgeLocatorType.LocalNetworkScanBridgeLocator);

        }

    }
}
