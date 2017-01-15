using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SportSystem.Data;
using SportSystem.Models;

namespace SportSystem.ConsoleClient.DatabaseUpdating
{
    public class UpdateProcessor
    {
        private readonly XmlNodeList _data;
        private readonly Type _type;
        private readonly int _startIndex;
        private readonly int _elementsToProcessCount;
        private object _generatedData;
        private SportSystemData _db;

        public UpdateProcessor(XmlNodeList data, Type type, int startIndex, int elementsToProcessCount)
        {
            this._data = data;
            this._type = type;
            this._startIndex = startIndex;
            this._elementsToProcessCount = elementsToProcessCount;
            this._db = new SportSystemData();
        }

        public object Data
        {
            get { return this._generatedData; }
        }

        public void UpdateData()
        {
            if (_type == typeof(Odd))
            {
                GenerateOdds();
            }
            else if (_type == typeof(Bet))
            {
                GenerateBets();
            }
            else if (_type == typeof(Match))
            {
                GenerateMatches();
            }
            else if (_type == typeof(Event))
            {
                GenerateEvents();
            }
            else if (_type == typeof(Sport))
            {
                GenerateSports();
            }
        }

        private void GenerateOdds()
        {
            var lastIndex = this._startIndex + this._elementsToProcessCount;

            var odds = new List<Odd>();

            for (int i = _startIndex; i < lastIndex; i++)
            {
                string oddName = _data[i].Attributes["Name"].Value;
                int oddId = int.Parse(_data[i].Attributes["ID"].Value);
                double oddValue = double.Parse(_data[i].Attributes["Value"].Value);

                var odd = new Odd();

                if (_data[i].Attributes["SpecialBetValue"] != null)
                {
                    double specialBetValue =
                        double.Parse(_data[i].Attributes["SpecialBetValue"].Value);
                    odd.SpecialBetValue = specialBetValue;
                }

                odd = new Odd
                {
                    Name = oddName,
                    Id = oddId,
                    Value = oddValue,
                };

                var existingElement = _db.Odds.All().FirstOrDefault(x => x.Id == odd.Id);

                if (existingElement != null)
                {
                    var needsUpdate = false;
                    
                    if (!existingElement.Name.Equals(odd.Name))
                    {
                        existingElement.Name = odd.Name;
                        needsUpdate = true;
                    }

                    if (!existingElement.SpecialBetValue.Equals(odd.SpecialBetValue))
                    {
                        existingElement.SpecialBetValue = odd.SpecialBetValue;
                        needsUpdate = true;
                    }

                    if (!existingElement.Value.Equals(odd.Value))
                    {
                        existingElement.Value = odd.Value;
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        _db.Odds.Update(existingElement);
                    }
                }
                else
                {
                    _db.Odds.Add(odd);
                }

                odds.Add(odd);

                CheckForSave(i);
            }

            _db.SaveChanges();
            _generatedData = odds;
        }

        private void GenerateBets()
        {
            var lastIndex = this._startIndex + this._elementsToProcessCount;

            var bets = new List<Bet>();

            for (int i = _startIndex; i < lastIndex; i++)
            {
                string betName = _data[i].Attributes["Name"].Value;
                int betId = int.Parse(_data[i].Attributes["ID"].Value);
                bool betIsLive = bool.Parse(_data[i].Attributes["IsLive"].Value);

                var odds = new List<Odd>();

                if (_data[i].HasChildNodes)
                {
                    var oddsNodes = _data[i].ChildNodes;

                    var seedManager = new UpdateManager();
                    var newOdds = seedManager.UpdateData(oddsNodes, typeof(Odd)) as IEnumerable<Odd>;
                    if (newOdds != null) odds.AddRange(newOdds);
                }

                var bet = new Bet
                {
                    Name = betName,
                    Id = betId,
                    IsLive = betIsLive,
                    Odds = odds
                };

                var existingElement = _db.Bets.All().FirstOrDefault(x => x.Id == bet.Id);

                if (existingElement != null)
                {
                    var needsUpdate = false;

                    if (!existingElement.Name.Equals(bet.Name))
                    {
                        existingElement.Name = bet.Name;
                        needsUpdate = true;
                    }

                    if (!existingElement.IsLive.Equals(bet.IsLive))
                    {
                        existingElement.IsLive = bet.IsLive;
                        needsUpdate = true;
                    }

                    if (!existingElement.Odds.Equals(bet.Odds))
                    {
                        existingElement.Odds = bet.Odds;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        _db.Bets.Update(existingElement);
                    }
                }
                else
                {
                    _db.Bets.Add(bet);
                }
                
                bets.Add(bet);

                CheckForSave(i);
            }

            _db.SaveChanges();
            _generatedData = bets;
        }

        private void GenerateMatches()
        {
            var lastIndex = this._startIndex + this._elementsToProcessCount;

            var matches = new List<Match>();

            for (int i = _startIndex; i < lastIndex; i++)
            {
                string matchName = _data[i].Attributes["Name"].Value;
                int matchId = int.Parse(_data[i].Attributes["ID"].Value);
                string matchStartDate = _data[i].Attributes["StartDate"].Value;
                string matchType = _data[i].Attributes["MatchType"].Value;

                var bets = new List<Bet>();
                var betsNodes = _data[i].ChildNodes;

                var seedManager = new UpdateManager();
                var newBets = seedManager.UpdateData(betsNodes, typeof(Bet)) as IEnumerable<Bet>;
                if (newBets != null) bets.AddRange(newBets);

                var match = new Match
                {
                    Name = matchName,
                    Id = matchId,
                    StartDate = matchStartDate,
                    MatchType = matchType,
                    Bets = bets
                };

                var existingElement = _db.Matches.All().FirstOrDefault(x => x.Id == match.Id);

                if (existingElement != null)
                {
                    var needsUpdate = false;

                    if (!existingElement.Name.Equals(match.Name))
                    {
                        existingElement.Name = match.Name;
                        needsUpdate = true;
                    }

                    if (!existingElement.StartDate.Equals(match.StartDate))
                    {
                        existingElement.StartDate = match.StartDate;
                        needsUpdate = true;
                    }

                    if (!existingElement.MatchType.Equals(match.MatchType))
                    {
                        existingElement.MatchType = match.MatchType;
                        needsUpdate = true;
                    }

                    if (!existingElement.Bets.Equals(match.Bets))
                    {
                        existingElement.Bets = match.Bets;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        _db.Matches.Update(existingElement);
                    }
                }
                else
                {
                    _db.Matches.Add(match);
                }

                matches.Add(match);

                CheckForSave(i);
            }

            _db.SaveChanges();
            _generatedData = matches;
        }

        private void GenerateEvents()
        {
            var lastIndex = this._startIndex + this._elementsToProcessCount;

            var events = new List<Event>();

            for (int i = _startIndex; i < lastIndex; i++)
            {
                string eventName = _data[i].Attributes["Name"].Value;
                int eventId = int.Parse(_data[i].Attributes["ID"].Value);
                bool isLive = bool.Parse(_data[i].Attributes["IsLive"].Value);
                int categoryId = int.Parse(_data[i].Attributes["CategoryID"].Value);

                Console.WriteLine($"    Index: {i}, Name: {eventName}, Id: {eventId}");

                var matches = new List<Match>();
                var matchesNodes = _data[i].ChildNodes;

                var seedManager = new UpdateManager();
                var newEvents = seedManager.UpdateData(matchesNodes, typeof(Match)) as IEnumerable<Match>;
                if (newEvents != null) matches.AddRange(newEvents);

                var eventToAdd = new Event
                {
                    Name = eventName,
                    Id = eventId,
                    IsLive = isLive,
                    CategoryId = categoryId,
                    Matches = matches
                };

                var existingElement = _db.Events.All().FirstOrDefault(x => x.Id == eventToAdd.Id);

                if (existingElement != null)
                {
                    var needsUpdate = false;

                    if (!existingElement.Name.Equals(eventToAdd.Name))
                    {
                        existingElement.Name = eventToAdd.Name;
                        needsUpdate = true;
                    }

                    if (!existingElement.IsLive.Equals(eventToAdd.IsLive))
                    {
                        existingElement.IsLive = eventToAdd.IsLive;
                        needsUpdate = true;
                    }

                    if (!existingElement.CategoryId.Equals(eventToAdd.CategoryId))
                    {
                        existingElement.CategoryId = eventToAdd.CategoryId;
                        needsUpdate = true;
                    }

                    if (!existingElement.Matches.Equals(eventToAdd.Matches))
                    {
                        existingElement.Matches = eventToAdd.Matches;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        _db.Events.Update(existingElement);
                        Console.WriteLine($"Updated event: {existingElement.Id} {existingElement.Name}");
                    }
                }
                else
                {
                    _db.Events.Add(eventToAdd);
                }

                events.Add(eventToAdd);

                CheckForSave(i);
            }

            _db.SaveChanges();
            _generatedData = events;
        }

        private void GenerateSports()
        {
            var lastIndex = this._startIndex + this._elementsToProcessCount;

            var sports = new List<Sport>();

            for (int i = _startIndex; i < lastIndex; i++)
            {
                string name = _data[i].Attributes["Name"].Value;
                int id = int.Parse(_data[i].Attributes["ID"].Value);

                Console.WriteLine($"{i} {name} {id}");

                var events = new List<Event>();
                var eventsNodes = _data[i].ChildNodes;

                var seedManager = new UpdateManager();
                var newSports = seedManager.UpdateData(eventsNodes, typeof(Event)) as IEnumerable<Event>;
                if (newSports != null) events.AddRange(newSports);

                Console.WriteLine($"index: {i}, name: {name}, Id: {id}");

                var sport = new Sport
                {
                    Name = name,
                    ID = id,
                    Events = events
                };

                var existingElement = _db.Sports.All().FirstOrDefault(x => x.ID == sport.ID);

                if (existingElement != null)
                {
                    var needsUpdate = false;

                    if (!existingElement.Name.Equals(sport.Name))
                    {
                        existingElement.Name = sport.Name;
                        needsUpdate = true;
                    }

                    if (!existingElement.Events.Equals(sport.Events))
                    {
                        existingElement.Events = sport.Events;
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        _db.Sports.Update(existingElement);
                        Console.WriteLine($"Updated sport: {existingElement.ID} {existingElement.Name}");
                    }
                }
                else
                {
                    _db.Sports.Add(sport);
                }

                sports.Add(sport);

                CheckForSave(i);
            }

            _db.SaveChanges();
            _generatedData = sports;
        }

        private void CheckForSave(int index)
        {
            if (index % 500 == 0)
            {
                _db.SaveChanges();
                _db = new SportSystemData();
            }
        }
    }
}
