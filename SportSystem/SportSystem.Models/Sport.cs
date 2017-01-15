
namespace SportSystem.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Sport
    {
        private ICollection<Event> _events;

        public Sport()
        {
            this._events = new HashSet<Event>();
        }

        [Key]
        public int Index { get; set; }

        public string Name { get; set; }

        [Index(IsUnique = true)]
        public int ID { get; set; }

        public virtual ICollection<Event> Events
        {
            get { return this._events; }
            set { this._events = value; }
        }
    }
}
