namespace SportSystem.Data.Repositories
{
    using SportSystem.Data.Repositories.Base;
    using SportSystem.Models;

    public class OddsRepository : GenericRepository<Odd>, IRepository<Odd>
    {
        public OddsRepository(ISportSystemDbContext context)
            : base(context)
        {
        }
    }
}
