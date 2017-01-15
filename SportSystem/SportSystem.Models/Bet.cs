namespace SportSystem.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public class Bet
    {
        private ICollection<Odd> _odds;

        public Bet()
        {
            this._odds = new HashSet<Odd>();
        }

        [Key]
        public int Index { get; set; }

        public string Name { get; set; }

        [Index(IsUnique = true)]
        public int Id { get; set; }

        public bool IsLive { get; set; }

        public virtual ICollection<Odd> Odds
        {
            get { return this._odds; }
            set { this._odds = value; }
        }
    }
}
