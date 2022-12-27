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
        public static bool IsCommand(this string text, out ChatCommand? command)
        {
            command = null;

            Regex regex = new("\\!(.*?)\\s|\\!(.*?)$");

            bool isMatch = regex.Match(text).Success;

            if (!isMatch)
                return false;

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
