using System.Collections.Generic;

namespace DataTech.Domain.Models
{
    public class GitHubOutItem
    {
        public GitHubOutItem()
        {
            this.User = new List<User>();
        }

        public List<User> User { get; set; }
    }

    public class User
    {
        public string Url { get; set; }
        public string Location { get; set; }
    }
}