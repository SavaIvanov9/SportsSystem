namespace SportSystem.Data.Repositories
{
    using SportSystem.Data.Repositories.Base;
    using SportSystem.Models;

    public class EventsRepository : GenericRepository<Event>, IRepository<Event>
    {
        public EventsRepository(ISportSystemDbContext context)
            : base(context)
        {
        }
    }
}
