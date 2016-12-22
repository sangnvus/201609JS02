using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using BSNCapstone.ViewModels;

namespace BSNCapstone.Models
{
    //You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : AspNet.Identity.MongoDB.IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType

            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here

            return userIdentity;

        }

        public string Avatar { get; set; }

        public string Cover { get; set; }

        public string Address { get; set; }

        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? DOB { get; set; }


       public string SSNImgId { get; set; }

       public string Gender { get; set; }

       public List<string> Follower { get; set; }

       public List<string> Following { get; set; }

       public List<InteractBookViewModel> Interacbook { get; set; }

       public bool AuthorConfirmed { get; set; }

       public bool Locked { get; set; }

       public DateTime CreatedTime { get; set; }

       public string Text { get; set; }

       public ApplicationUser()
       {
           Follower = new List<string>();
           Following = new List<string>();
           AuthorConfirmed = false;
           Locked = false;
           CreatedTime = DateTime.Now.AddHours(7);
           Interacbook = new List<InteractBookViewModel>();
           Text = "";
           Avatar = "wehvqsenzprhkwbxeki0";
           Cover = "yvrjxpo5e4l7blnhopwt";
       }
    }

}