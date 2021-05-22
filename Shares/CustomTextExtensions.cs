using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;
using StreamElementsNET.Models.Follower;
using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Shares
{
    public static class CustomTextExtensions
    {
        private static readonly Dictionary<string, Dictionary<dynamic, string>> CustomTextParameter = new()
        {
            {
                "{username}",
                new()
                {
                    { typeof(Subscriber), "DisplayName" },
                    { typeof(ChatMessage), "DisplayName" },
                    { typeof(Follower), "DisplayName"}
                }
            }
        };

        public static string ToCustomTextWithParameter(this string customText, dynamic obj)
        {
            string paramToChange = "";
            string changedParam = "";

            foreach (KeyValuePair<string, Dictionary<dynamic, string>> parameter in CustomTextParameter)
            {
                paramToChange = parameter.Key;
                if (customText.Contains(paramToChange))
                {
                    string propName = parameter.Value[obj.GetType()].ToString();
                    changedParam = obj.GetType().GetProperty(propName).GetValue(obj, null);
                }
            }

            return customText.Replace(paramToChange, changedParam);
        }
    }
}
