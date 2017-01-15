namespace SportSystem.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Odd
    {
        [Key]
        public int Index { get; set; }

        public string Name { get; set; }

        [Index(IsUnique = true)]
        public int Id { get; set; }

        public double Value { get; set; }

        public double SpecialBetValue { get; set; }
    }
}
