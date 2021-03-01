using System;

namespace RoboSenseCore.ViewModels
{
    public class AccountsViewModel
    {
        public String Id { get; set; }
        public String UserName { get; set; }
        public String DeputyName { get; set; }
        public Boolean EmailConfirmed { get; set; }
        public Boolean FreeDeputizing { get; set; }
        public String PhoneNumber { get; set; }
    }
}
