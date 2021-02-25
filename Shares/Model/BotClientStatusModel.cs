using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public enum BotClientStatusModel
    {        
        Stopped,
        Started,
        AwaitingDisconnect,
        AwaitingConnection
    }
}
