namespace SportSystem.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Match
    {
        private ICollection<Bet> _bets;

        public Match()
        {
            this._bets = new HashSet<Bet>();
        }

        [Key]
        public int Index { get; set; }

        public string Name { get; set; }

        [Index(IsUnique = true)]
        public int Id { get; set; }

        public string StartDate { get; set; }

        public string MatchType { get; set; }

        public virtual ICollection<Bet> Bets
        {
            get { return this._bets; }
            set { this._bets = value; }
        }
    }
}
