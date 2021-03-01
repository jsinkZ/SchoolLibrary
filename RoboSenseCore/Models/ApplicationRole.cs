using Microsoft.AspNetCore.Identity;

namespace RoboSenseCore.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }

        public string Description { get; set; }
    }
}
