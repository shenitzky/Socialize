using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Socialize.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public int Age { get; set; }
        public bool Premium { get; set; }
        public int DeclineNum { get; set; }
        public int AcceptNum { get; set; }

        public ICollection<FinalMatch> FinalMatches { get; set; }

        [ForeignKey("UserId")]
        public ICollection<Factor> Factors { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<FinalMatchLog> FinalMatchesLog { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<AvatarImg> AvatarImgs { get; set; }
        public DbSet<OptionalMatchLog> OptionalMatchLog { get; set; }
        public DbSet<MatchRequestLog> MatchRequestLog { get; set; }


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}