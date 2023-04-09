using Bot_Manager.LiteDB.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_Manager.LiteDB
{
    public static class LiteDBHelper
    {
        internal static int GetDeathCount(string channel)
        {
            using LiteDatabase db = new(@"twitchbot.db");

            ILiteCollection<StreamEventsCounter> col = db.GetCollection<StreamEventsCounter>("streamevents");

            StreamEventsCounter result = col.FindOne(x => x.Channel == channel && x.StreamDate == DateTime.Today);

            bool needInsert = false;

            if (result is null)
            {
                result = new StreamEventsCounter
                {
                    Channel = channel,
                    StreamDate = DateTime.Today
                };
                needInsert = true;
            }

            result.LastUpdate = DateTime.Now;

            if (needInsert)
            {
                col.Insert(result);
            }
            else
            {
                col.Update(result);
            }

            return result.Deaths;
        }

        internal static void UpdateDeathCount(string channel, int newDeathCount)
        {
            using LiteDatabase db = new(@"twitchbot.db");

            ILiteCollection<StreamEventsCounter> col = db.GetCollection<StreamEventsCounter>("streamevents");

            StreamEventsCounter result = col.FindOne(x => x.Channel == channel && x.StreamDate == DateTime.Today);

            bool needInsert = false;

            if (result is null)
            {
                result = new StreamEventsCounter
                {
                    Channel = channel,
                    StreamDate = DateTime.Today
                };
                needInsert = true;
            }

            result.LastUpdate = DateTime.Now;
            result.Deaths = newDeathCount;
          

            if (needInsert)
            {
                col.Insert(result);
            }
            else
            {
                col.Update(result);
            }
        }
    }
}
