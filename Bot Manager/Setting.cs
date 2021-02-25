using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Shares.Model;

namespace Bot_Manager
{   

    public static class Setting
    {
        static readonly string savePathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "Twitch Bot Server");
        static readonly string credentialFilePath = Path.Combine(savePathDirectory, "Settings");

        public static void SaveBotsCredentials(List<TwitchClientExt> bots)
        {            
            if (!Directory.Exists(savePathDirectory))
            {
                Directory.CreateDirectory(savePathDirectory);
            }

            var botsSettings = bots.Select(_ => _.BotSetting).ToList();

            var serializer = new XmlSerializer(typeof(List<BotSettingModel>));

            var xmlserializer = new XmlSerializer(typeof(List<BotSettingModel>));
            var stringWriter = new StringWriter();

            using (var writer = XmlWriter.Create(stringWriter))
            {                
                xmlserializer.Serialize(writer, botsSettings);

                var data = Encryption.Encrypt(stringWriter.ToString());

                File.WriteAllText(credentialFilePath, data);
            }
        }

        public static List<BotSettingModel> ReadBotClientCredentials()
        {
            if (!File.Exists(credentialFilePath)) return null;

            string xmlData = Encryption.Decrypt(File.ReadAllText(credentialFilePath));

            XmlSerializer serializer = new XmlSerializer(typeof(List<BotSettingModel>));
            StringReader rdr = new StringReader(xmlData);

            return (List<BotSettingModel>)serializer.Deserialize(rdr);
        }
    }
}
