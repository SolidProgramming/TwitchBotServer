using System;
using System.Collections.Generic;
using System.Text;
using Bot_Manager.Enum;

namespace Bot_Manager.Model
{
    public struct BotClientRequestDataModel
    {
        public BotClientRequestActionType RequestActionType { get; set; }
        public dynamic Data { get; set; }
    }
}
