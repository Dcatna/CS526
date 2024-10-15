using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ImageSharingWithSecurity.Models
{
    public sealed class ApplicationUser : IdentityUser
    {

        public bool Ada { get; set; }
        public bool Active { get; set; }

        /*
         * Looks like this property is never used in ImagesController::DoListByUser
         */
        public ICollection<Image> Images { get; set; }

        public ApplicationUser()
        {
            Active = true;
            Ada = false;
            Images = new List<Image>();
        }

        public ApplicationUser(string u)
        {
            Active = true;
            UserName = u;
            Email = u;
            Ada = false;
            Images = new List<Image>();
        }

        public ApplicationUser(string u, bool isAda) 
        {
            Active = true;
            UserName = u;
            Email = u;
            Ada = isAda;
            Images = new List<Image>();
        }
    }
}
