using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace CodeMashCrawler {
    class Program {
        private static List<Session> Sessions;
        static void Main(string[] args) {
            Sessions = new List<Session>();
            GetSessions().Wait();
        }

        private static async Task GetSessions() {
            try {
                var url = "http://www.codemash.org/session-list/";
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var sessionContainers = doc.DocumentNode.Descendants("li")
                    .Where(x => x.GetAttributeValue("class", "").Equals("sz-session sz-session--full")).ToList();
                if (sessionContainers == null)
                    return;
                foreach (var sessionContainer in sessionContainers) {
                    var session = new Session {
                        Name = sessionContainer.Descendants("h3").FirstOrDefault().InnerText,
                        Date = sessionContainer.Descendants("div").FirstOrDefault(x => x.GetAttributeValue("class", "").Equals("sz-session__time"))?.InnerText
                    };
                    Sessions.Add(session);
                }
                Console.WriteLine("Writing sessions to file");
                var groupedSessions = Sessions.GroupBy(x => x.Date);
                File.WriteAllText(@".\Session_Lists\sessions.json", JsonConvert.SerializeObject(groupedSessions));
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}