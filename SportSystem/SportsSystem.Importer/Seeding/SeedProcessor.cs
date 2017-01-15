namespace SportsSystem.Importer.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using SportSystem.Data;
    using SportSystem.Models;

    public class SeedProcessor
    {
        private readonly XmlNodeList _data;
        private readonly Type _type;
        private readonly int _startIndex;
        private readonly int _elementsToProcessCount;
        private object _generatedData;
        private SportSystemData _db;

        public SeedProcessor(XmlNodeList data, Type type, int startIndex, int elementsToProcessCount)
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

        public void SeedData()
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

                odds.Add(odd);
                _db.Odds.Add(odd);

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

                    var seedManager = new SeedManager();
                    var newOdds = seedManager.SeedData(oddsNodes, typeof(Odd)) as IEnumerable<Odd>;
                    if (newOdds != null) odds.AddRange(newOdds);
                }

                var bet = new Bet
                {
                    Name = betName,
                    Id = betId,
                    IsLive = betIsLive,
                    Odds = odds
                };

                _db.Bets.Add(bet);
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

                var seedManager = new SeedManager();
                var newBets = seedManager.SeedData(betsNodes, typeof(Bet)) as IEnumerable<Bet>;
                if (newBets != null) bets.AddRange(newBets);

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

                var seedManager = new SeedManager();
                var newEvents = seedManager.SeedData(matchesNodes, typeof(Match)) as IEnumerable<Match>;
                if (newEvents != null) matches.AddRange(newEvents);

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

                var seedManager = new SeedManager();
                var newSports = seedManager.SeedData(eventsNodes, typeof(Event)) as IEnumerable<Event>;
                if (newSports != null) events.AddRange(newSports);

                Console.WriteLine($"index: {i}, name: {name}, Id: {id}");

                var sport = new Sport
                {
                    Name = name,
                    ID = id,
                    Events = events
                };

                sports.Add(sport);
                _db.Sports.Add(sport);

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
