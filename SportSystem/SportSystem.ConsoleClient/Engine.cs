using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SportSystem.ConsoleClient.DatabaseUpdating;
using SportSystem.Data;
using SportSystem.Models;

namespace SportSystem.ConsoleClient
{
    public class Engine
    {
        private static Engine _instance;
        private static readonly object SyncRoot = new object();
        private SportSystemData _db;

        private Engine()
        {
            _db = new SportSystemData();
        }

        public static Engine Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Engine();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Start()
        {
            var data = GetWebData("http://vitalbet.net/sportxml");

            CheckDirectory("WebData");

            string currentLog = $"{DateTime.Now:yyyy-MM-dd_hh;mm;ss}.xml";
            string path = Path.Combine(
                Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location),
                    $@"WebData\{currentLog}");

            CreateDataFile(path, data);

            UpdateDatabase(data);
        }

        private void UpdateDatabase(string data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            var root = doc.DocumentElement;

            var sports = root.GetElementsByTagName("Sport");

            Console.WriteLine("Started updating database...");

            var updateManager = new UpdateManager();
            updateManager.UpdateData(sports, typeof(Sport));

            Console.WriteLine("Database updated successfully.");
        }

        private void SinglethreadImport(string data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            var root = doc.DocumentElement;

            var sports = root.GetElementsByTagName("Sport");

            Console.WriteLine("Started seeding data...");

            int addedElements = 0;

            for (int i = 0; i < sports.Count; i++)
            {
                string name = sports[i].Attributes["Name"].Value;
                int id = int.Parse(sports[i].Attributes["ID"].Value);

                Console.WriteLine($"{i} {name} {id}");

                var events = new List<Event>();
                var eventsNodes = sports[i].ChildNodes;
                for (int j = 0; j < eventsNodes.Count; j++)
                {
                    string eventName = eventsNodes[j].Attributes["Name"].Value;
                    int eventId = int.Parse(eventsNodes[j].Attributes["ID"].Value);
                    bool isLive = bool.Parse(eventsNodes[j].Attributes["IsLive"].Value);
                    int categoryId = int.Parse(eventsNodes[j].Attributes["CategoryID"].Value);

                    //Console.WriteLine($"{eventName} {eventId} {isLive} {categoryId}");

                    var matches = new List<Match>();
                    var matchesNodes = eventsNodes[j].ChildNodes;

                    for (int k = 0; k < matchesNodes.Count; k++)
                    {
                        string matchName = matchesNodes[k].Attributes["Name"].Value;
                        int matchId = int.Parse(matchesNodes[k].Attributes["ID"].Value);
                        string matchStartDate = matchesNodes[k].Attributes["StartDate"].Value;
                        string matchType = matchesNodes[k].Attributes["MatchType"].Value;

                        //Console.WriteLine($"{matchName} {matchId} {matchStartDate} {matchType}");

                        var bets = new List<Bet>();
                        var betsNodes = matchesNodes[k].ChildNodes;

                        for (int l = 0; l < betsNodes.Count; l++)
                        {
                            string betName = betsNodes[l].Attributes["Name"].Value;
                            int betId = int.Parse(betsNodes[l].Attributes["ID"].Value);
                            bool betIsLive = bool.Parse(betsNodes[l].Attributes["IsLive"].Value);

                            //Console.WriteLine($"{betName} {betId} {betIsLive}");

                            var odds = new List<Odd>();
                            if (betsNodes[l].HasChildNodes)
                            {
                                var oddsNodes = betsNodes[l].ChildNodes;

                                for (int m = 0; m < oddsNodes.Count; m++)
                                {
                                    string oddName = oddsNodes[m].Attributes["Name"].Value;
                                    int oddId = int.Parse(oddsNodes[m].Attributes["ID"].Value);
                                    double oddValue = double.Parse(oddsNodes[m].Attributes["Value"].Value);

                                    var odd = new Odd();

                                    //Console.WriteLine($"{oddName} {oddId} {oddValue}");

                                    if (oddsNodes[m].Attributes["SpecialBetValue"] != null)
                                    {
                                        double specialBetValue =
                                        double.Parse(oddsNodes[m].Attributes["SpecialBetValue"].Value);
                                        odd.SpecialBetValue = specialBetValue;

                                        //Console.WriteLine(" " + specialBetValue);
                                    }

                                    odd = new Odd
                                    {
                                        Name = oddName,
                                        Id = oddId,
                                        Value = oddValue,
                                    };

                                    odds.Add(odd);

                                    _db.Odds.Add(odd);

                                    addedElements++;
                                    CheckForSave(addedElements);
                                }
                            }

                            var bet = new Bet
                            {
                                Name = betName,
                                Id = betId,
                                IsLive = betIsLive,
                                Odds = odds
                            };

                            bets.Add(bet);

                            _db.Bets.Add(bet);

                            addedElements++;
                            CheckForSave(addedElements);
                        }

                        var match = new Match
                        {
                            Name = matchName,
                            Id = matchId,
                            StartDate = matchStartDate,
                            MatchType = matchType,
                            Bets = bets
                        };

                        matches.Add(match);

                        _db.Matches.Add(match);

                        addedElements++;
                        CheckForSave(addedElements);
                    }

                    var eventToAdd = new Event
                    {
                        Name = eventName,
                        Id = eventId,
                        IsLive = isLive,
                        CategoryId = categoryId,
                        Matches = matches
                    };

                    events.Add(eventToAdd);
                    _db.Events.Add(eventToAdd);

                    addedElements++;
                    CheckForSave(addedElements);
                }

                //bool isLive = bool.Parse(sports[i].Attributes["IsLive"].Value);
                Console.WriteLine($"{name} {id}, data added: {addedElements}");

                var sport = new Sport
                {
                    Name = name,
                    ID = id,
                    Events = events
                };

                //Console.WriteLine($"{i} {name}");
                _db.Sports.Add(sport);

                addedElements++;
                CheckForSave(addedElements);
            }

            _db.SaveChanges();
            Console.WriteLine("Seeding compleated.");
        }

        private void CheckForSave(int index)
        {
            if (index % 1000 == 0)
            {
                Console.Write(".");
            }

            if (index % 10000 == 0)
            {
                Console.Write("/");
                _db.SaveChanges();
                _db = new SportSystemData();
            }
        }

        private string GetWebData(string link)
        {
            var request = WebRequest.Create(link) as HttpWebRequest;

            request.ContentType = "application/json";
            request.Method = "GET";

            var response = request.GetResponse();

            var responseReader = new StreamReader(response.GetResponseStream());

            //Console.WriteLine(responseReader.ReadToEnd());

            //File.WriteAllText(@"response.txt", responseReader.ReadToEnd());
            var data = responseReader.ReadToEnd();

            responseReader.Close();

            return data;
        }

        private void CheckDirectory(string folderName)
        {
            string dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pathString = Path.Combine(dirName, folderName);
            if (!Directory.Exists(pathString))
            {
                Directory.CreateDirectory(pathString);
            }
        }

        private void CreateDataFile(string path, string content)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter file = File.CreateText(path))
                {
                    file.Write(content);
                }
            }
        }
    }
}
