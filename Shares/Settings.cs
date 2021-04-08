using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Shares.Model;
using Shares.Enum;
using Logger;

namespace Shares
{
    public static class Settings
    {
        private static readonly string savePathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "Twitch Bot Server");
        private static readonly string BotSettingsFilePath = Path.Combine(savePathDirectory, "BotSettings");
        private static readonly string OBSSettingsFilePath = Path.Combine(savePathDirectory, "OBSSettings");
        private static readonly string ChatCommandsFilePath = Path.Combine(savePathDirectory, "ChatCommands");

        private static Dictionary<FileType, string> FilesPath = new()
        {
            { FileType.BotSettings, BotSettingsFilePath },
            { FileType.OBSSettings, OBSSettingsFilePath },
            { FileType.ChatCommands, ChatCommandsFilePath }
        };
        public static T LoadSettings<T>(FileType fileType)
        {
            CheckDirectoryExists();

            string filePath = FilesPath[fileType];

            if (!File.Exists(filePath)) return default;

            try
            {
                string xmlData = Encryption.Decrypt(File.ReadAllText(filePath));

                var serializer = new XmlSerializer(typeof(T));
                var rdr = new StringReader(xmlData);

                return (T)Convert.ChangeType(serializer.Deserialize(rdr), typeof(T), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                Log.Fatal($"Could not load settings for {fileType}", "Settings");
                return default;
            }
        }
        public static void SaveSettings(dynamic settings, FileType fileType)
        {
            CheckDirectoryExists();

            string filePath = FilesPath[fileType];

            try
            {
                XmlSerializer xmlserializer = new(settings.GetType());
                StringWriter stringWriter = new();

                using var writer = XmlWriter.Create(stringWriter);

                xmlserializer.Serialize(writer, settings);

                string data = Encryption.Encrypt(stringWriter.ToString());

                File.WriteAllText(filePath, data);
            }
            catch (Exception)
            {
                Log.Fatal($"Could not save settings for {fileType}", "Settings");
            }
        }
        private static void CheckDirectoryExists()
        {
            if (!Directory.Exists(savePathDirectory)) Directory.CreateDirectory(savePathDirectory);
        }


    }
}
