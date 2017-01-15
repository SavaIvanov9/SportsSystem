namespace SportSystem.Data.Repositories
{
    using SportSystem.Data.Repositories.Base;
    using SportSystem.Models;

    public class BetsRepository : GenericRepository<Bet>, IRepository<Bet>
    {
        public BetsRepository(ISportSystemDbContext context)
            : base(context)
        {
        }
    }
}
