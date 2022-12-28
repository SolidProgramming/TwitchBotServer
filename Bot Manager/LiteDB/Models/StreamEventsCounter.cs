using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Manager.LiteDB.Models
{
    public class StreamEventsCounter
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public int Deaths { get; set; }
        public int Cheats { get; set; }
        public DateTime StreamDate { get; set; } 
        public DateTime LastUpdate { get; set; }
    }
}
