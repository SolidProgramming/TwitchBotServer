using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Shares.Enum;
using Shares;

namespace Bot_Manager
{
    public static class Extensions
    {
        public static bool IsCommand(this string text, out ChatCommand? command)
        {
            command = null;
            string commandName = Regex.Match(text, "\\!(.*?)\\s").Groups[1].Value;

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
