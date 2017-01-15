
using System.Diagnostics;

namespace SportSystem.Data
{
    using SportSystem.Data.Repositories.Base;
    using System;
    using System.Collections.Generic;
    using Repositories;
    using Models;

    public class SportSystemData : ISportSystemData
    {
        private readonly ISportSystemDbContext _context;
        private readonly IDictionary<Type, object> _repositories;

        public SportSystemData() 
            : this(new SportSystemDbContext())
        {

        }

        public SportSystemData(ISportSystemDbContext context)
        {
            this._context = context;
            this._repositories = new Dictionary<Type, object>();
        }
        
        public EventsRepository Events
        {
            get
            {
                return (EventsRepository)this.GetRepository<Event>();
            }
        }

        public MatchesRepository Matches
        {
            get
            {
                return (MatchesRepository)this.GetRepository<Match>();
            }
        }

        public BetsRepository Bets
        {
            get
            {
                return (BetsRepository)this.GetRepository<Bet>();
            }
        }

        public OddsRepository Odds
        {
            get
            {
                return (OddsRepository)this.GetRepository<Odd>();
            }
        }
        
        public SportsRepository Sports {
            get
            {
                return (SportsRepository)this.GetRepository<Sport>();
            }
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var repositoryType = typeof(T);

            if (!this._repositories.ContainsKey(repositoryType))
            {
                var type = typeof(GenericRepository<T>);

                if (repositoryType.IsAssignableFrom(typeof(Sport)))
                {
                    type = typeof(SportsRepository);
                }
                else if (repositoryType.IsAssignableFrom(typeof(Event)))
                {
                    type = typeof(EventsRepository);
                }
                else if (repositoryType.IsAssignableFrom(typeof(Match)))
                {
                    type = typeof(MatchesRepository);
                }
                else if (repositoryType.IsAssignableFrom(typeof(Bet)))
                {
                    type = typeof(BetsRepository);
                }
                else if (repositoryType.IsAssignableFrom(typeof(Odd)))
                {
                    type = typeof(OddsRepository);
                }

                this._repositories.Add(repositoryType, Activator.CreateInstance(type, this._context));
            }

            return (IRepository<T>)this._repositories[repositoryType];
        }
    }
}
