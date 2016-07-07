using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;

namespace dx.misv.passwordbot.app
{
    public static class Utility
    {
        private static readonly Dictionary<string, long> numberTable =
            new Dictionary<string, long>
            {
                {"zero", 0},
                {"one", 1},
                {"two", 2},
                {"three", 3},
                {"four", 4},
                {"five", 5},
                {"six", 6},
                {"seven", 7},
                {"eight", 8},
                {"nine", 9},
                {"ten", 10},
                {"eleven", 11},
                {"twelve", 12},
                {"thirteen", 13},
                {"fourteen", 14},
                {"fifteen", 15},
                {"sixteen", 16},
                {"seventeen", 17},
                {"eighteen", 18},
                {"nineteen", 19},
                {"twenty", 20},
                {"thirty", 30},
                {"forty", 40},
                {"fifty", 50},
                {"sixty", 60},
                {"seventy", 70},
                {"eighty", 80},
                {"ninety", 90},
                {"hundred", 100},
                {"thousand", 1000},
                {"million", 1000000},
                {"billion", 1000000000},
                {"trillion", 1000000000000},
                {"quadrillion", 1000000000000000},
                {"quintillion", 1000000000000000000}
            };

        public static long ToLong(string numberString)
        {
            var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                .Select(m => m.Value.ToLowerInvariant())
                .Where(v => numberTable.ContainsKey(v))
                .Select(v => numberTable[v]);
            long acc = 0, total = 0L;
            foreach (var n in numbers)
            {
                if (n >= 1000)
                {
                    total += acc*n;
                    acc = 0;
                }
                else if (n >= 100)
                {
                    acc *= n;
                }
                else acc += n;
            }
            return (total + acc)*(numberString.StartsWith("minus",
                StringComparison.InvariantCultureIgnoreCase)
                ? -1
                : 1);
        }

        public static async Task<IEnumerable<string>> PasswordAPIRequest(string strength, string count)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["PasswordBotApiUrl"]);
            var request =
                new RestRequest(string.Format(ConfigurationManager.AppSettings["PasswordBotApiPath"], strength, count),
                    Method.GET);
            var restResponse = await client.ExecuteTaskAsync<List<string>>(request);
            return restResponse.Data;

        }

        public static IEnumerable<string> PasswordStrengths()
        {
            return new[] {"simple", "strong", "complex"};
        }

        public static string ToMarkdownList(this IEnumerable<string> items, bool unordered = true)
        {
            var itemsArray = items.ToArray();
            var sb = new StringBuilder();
            sb.AppendLine();
            var bullet = "*";
            for (var i = 0; i < itemsArray.Length; i++)
            {
                if (!unordered)
                {
                    bullet = (i + 1).ToString();
                }
                sb.AppendFormat("{0} {1}\r\n", bullet, itemsArray[i]);
            }
            return sb.ToString();
        }
    }
}