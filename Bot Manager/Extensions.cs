using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shares.Enum;
using Shares;
using WebSocketSharp;

namespace Bot_Manager
{
    public static class Extensions
    {
        public static bool IsCommand(this string text, out ChatCommand? command, out int value)
        {
            value = 0;
            command = null;

            Regex regex = new("(?:!(.*?)\\s|\\!(.*?)$)(?'value'\\s?\\d{0,3})");

            Match match = regex.Match(text);

            bool isMatch = match.Success;

            if (!isMatch)
                return false;

            string valueGrp = match.Groups["value"].Value;

            if (!string.IsNullOrEmpty(valueGrp))
            {
                int.TryParse(valueGrp, out value);
            }

            GroupCollection matches = regex.Match(text).Groups;

            string commandName = "";

            if (!string.IsNullOrEmpty(matches[1].Value))
            {
                commandName = matches[1].Value;
            }

            if (!string.IsNullOrEmpty(matches[2].Value))
            {
                commandName = matches[2].Value;
            }

            if (string.IsNullOrEmpty(commandName))
            {
                return false;
            }

            command = TextChatCommand.GetChatCommandByName(commandName);

            if (command is null)
            {
                return false;
            }

            return true;
        }
    }
}
