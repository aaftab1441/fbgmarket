using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FBG.Market.Web.Identity.Models
{
    public class AspNetUsersLocal
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool? IsEnabled { get; set; }

        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public Nullable<int> VID { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

    }
}