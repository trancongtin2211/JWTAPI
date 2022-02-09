using System;
using Microsoft.AspNetCore.Identity;

namespace Claim.Data.Enties{

    public class AppUser:IdentityUser{
        
        public string FullName { get; set; }
        public DateTime DataCreated {get; set;} 
        public DateTime DataModified {get; set;}

    }
}