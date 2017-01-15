namespace SportSystem.Data
{
    using System.Data.Entity;
    using Migrations;
    using Models;

    public class SportSystemDbContext : DbContext, ISportSystemDbContext
    {
        public SportSystemDbContext()
            : base("name=SportSystemDbContext")
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<SportSystemDbContext, Configuration>());
            //Database.SetInitializer(new DropCreateDatabaseAlways<SportSystemDbContext>());
        }

        public virtual IDbSet<Sport> Sports { get; set; }

        public virtual IDbSet<Event> Events { get; set; }
                
        public virtual IDbSet<Match> Matches { get; set; }
                
        public virtual IDbSet<Bet> Bets { get; set; }
                
        public virtual IDbSet<Odd> Odds { get; set; }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
    }
}