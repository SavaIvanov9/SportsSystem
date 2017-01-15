namespace SportSystem.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Event
    {
        private ICollection<Match> _matches;

        public Event()
        {
            this._matches = new HashSet<Match>();
        }

        [Key]
        public int Index { get; set; }

        public string Name { get; set; }

        [Index(IsUnique = true)]
        public int Id { get; set; }

        public bool IsLive { get; set; }

        public int CategoryId { get; set; }

        public virtual ICollection<Match> Matches
        {
            get { return this._matches; }
            set { this._matches = value; }
        }
    }
}
