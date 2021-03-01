using Microsoft.AspNetCore.Identity;
using System;

namespace SchoolLibrary3.Models
{
    public class User : IdentityUser
    {
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public String PatronymicName { get; set; }
    }
}
