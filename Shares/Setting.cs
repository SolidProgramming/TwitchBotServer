using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Shares.Model;

namespace Shares
{   

    public static class Setting
    {
        static readonly string savePathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "Twitch Bot Server");
        static readonly string BotSettingsFilePath = Path.Combine(savePathDirectory, "BotSettings");
        static readonly string OBSSettingsFilePath = Path.Combine(savePathDirectory, "OBSSettings");

        public static void SaveBotsSettings(List<TwitchClientExt> bots)
        {
            CheckDirectoryExists();

            var botsSettings = bots.Select(_ => _.BotSetting).ToList();

            var xmlserializer = new XmlSerializer(typeof(List<BotSettingModel>));
            var stringWriter = new StringWriter();

            using (var writer = XmlWriter.Create(stringWriter))
            {                
                xmlserializer.Serialize(writer, botsSettings);

                var data = Encryption.Encrypt(stringWriter.ToString());

                File.WriteAllText(BotSettingsFilePath, data);
            }
        }

        public static List<BotSettingModel> ReadBotSettings()
        {
            CheckDirectoryExists();

            if (!File.Exists(BotSettingsFilePath)) return null;

            string xmlData = Encryption.Decrypt(File.ReadAllText(BotSettingsFilePath));

            XmlSerializer serializer = new(typeof(List<BotSettingModel>));
            StringReader rdr = new(xmlData);

            return (List<BotSettingModel>)serializer.Deserialize(rdr);
        }

        public static void SaveOBSSettings(OBSSettingModel obsSettings)
        {
            CheckDirectoryExists();

            var xmlserializer = new XmlSerializer(typeof(OBSSettingModel));
            var stringWriter = new StringWriter();

            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, obsSettings);

                var data = Encryption.Encrypt(stringWriter.ToString());

                File.WriteAllText(OBSSettingsFilePath, data);
            }

        }
        public static OBSSettingModel ReadOBSSettings()
        {
            CheckDirectoryExists();

            if (!File.Exists(OBSSettingsFilePath)) return null;

            string xmlData = Encryption.Decrypt(File.ReadAllText(OBSSettingsFilePath));

            XmlSerializer serializer = new(typeof(OBSSettingModel));
            StringReader rdr = new(xmlData);

            return (OBSSettingModel)serializer.Deserialize(rdr);
        }
        private static void CheckDirectoryExists()
        {
            if (!Directory.Exists(savePathDirectory))
            {
                Directory.CreateDirectory(savePathDirectory);
            }
        }
    }
}
