using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Model;

namespace Shares
{
    public static class ChatCommand
    {
        public static Dictionary <string, ChatCommandModel> dictChatCommands = new Dictionary<string, ChatCommandModel>()
        {
            { "!wheel", ChatCommandModel.wheel }
        };

        public static ChatCommandModel ToChatCommand(this string textcommand)
        {
            return dictChatCommands.SingleOrDefault(_ => _.Key == textcommand).Value;
        }
    }
}
