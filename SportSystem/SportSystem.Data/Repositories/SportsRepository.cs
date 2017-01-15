
namespace SportSystem.Data.Repositories
{
    using Models;
    using SportSystem.Data.Repositories.Base;

    public class SportsRepository : GenericRepository<Sport>, IRepository<Sport>
    {
        public SportsRepository(ISportSystemDbContext context)
            : base(context)
        {
        }
    }
}
