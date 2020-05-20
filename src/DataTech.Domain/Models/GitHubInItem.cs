using System.Collections.Generic;

namespace DataTech.Domain.Models
{
    public class GitHubInItem
    {
        public List<string> Language { get; set; }
        public List<string> Location { get; set; }
        public int Repository { get; set; }
        public int Followers { get; set; }
    }
}