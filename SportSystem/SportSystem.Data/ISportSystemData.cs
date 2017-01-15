namespace SportSystem.Data
{
    using Repositories;

    public interface ISportSystemData
    {
        EventsRepository Events
        {
            get;
        }

        MatchesRepository Matches
        {
            get;
        }

        BetsRepository Bets
        {
            get;
        }

        OddsRepository Odds
        {
            get;
        }

        SportsRepository Sports
        {
            get;
        }

        void SaveChanges();
    }
}
