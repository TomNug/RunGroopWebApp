using System.ComponentModel.DataAnnotations;

namespace RunGroopWebApp.Models
{
    public class Address
    {
        // Identifies it as a primary key
        [Key]
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }

    }
}
