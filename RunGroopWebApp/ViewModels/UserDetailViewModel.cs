﻿using RunGroopWebApp.Models;

namespace RunGroopWebApp.ViewModels
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public List<AppUser> RelatedUsers { get; set; }
    }
}
