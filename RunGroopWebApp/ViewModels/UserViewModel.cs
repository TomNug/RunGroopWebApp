using RunGroopWebApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunGroopWebApp.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }
    }
}
