using ImageSharingWithSecurity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImageSharingWithSecurity.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /*
         * There is a table for users (ApplicationUser) defined in the Identity DB Context.
         */

        public DbSet<Image> Images { get; init; }

        public DbSet<Tag> Tags { get; init; }

    }

}