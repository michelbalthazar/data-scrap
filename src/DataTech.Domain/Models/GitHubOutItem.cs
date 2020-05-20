using System.Collections.Generic;

namespace DataTech.Domain.Models
{
    public class GitHubOutItem
    {
        public IEnumerable<User> User { get; set; }
    }

    public class User
    {
        public string Url { get; set; }
        public string Model { get; set; }
    }
}