namespace SportSystem.Data.Repositories
{
    using SportSystem.Data.Repositories.Base;
    using SportSystem.Models;

    public class MatchesRepository : GenericRepository<Match>, IRepository<Match>
    {
        public MatchesRepository(ISportSystemDbContext context)
            : base(context)
        {
        }
    }
}
