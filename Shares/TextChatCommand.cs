using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Model;
using Shares.Enum;

namespace Shares
{
    public static class TextChatCommand
    {
        private static Dictionary<string, ChatCommand> dictChatCommands = new()
        {
            {
                "so",
                ChatCommand.ShoutOut
            },
            {
                "startstream",
                ChatCommand.StartStream
            },
            {
                "cheat",
                ChatCommand.Cheat
            },
            {
                "rip",
                ChatCommand.Death
            },
            {
                "death",
                ChatCommand.Death
            },
            {
                "gestorben",
                ChatCommand.Death
            }

        };

        public static List<string> GetTextChatCommands()
        {
            return dictChatCommands.Select(_ => _.Key).ToList();
        }

        public static ChatCommand GetChatCommandByName(string commandName)
        {
            return dictChatCommands.SingleOrDefault(_ => _.Key == commandName).Value;
        }

        public static List<string> GetCommandNames()
        {
            return System.Enum.GetNames(typeof(ChatCommand)).Where(_ => !_.Contains("None")).ToList();
        }

        public static ChatCommand ToChatCommand(this string textcommand)
        {
            return dictChatCommands.SingleOrDefault(_ => _.Key == textcommand).Value;
        }
    }
}
