using System;
using System.Threading.Tasks;
using ImageSharingWithSecurity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ImageSharingWithSecurity.DAL
{
    public  class ApplicationDbInitializer
    {
        private ApplicationDbContext db;
        private ILogger<ApplicationDbInitializer> logger;
        public ApplicationDbInitializer(ApplicationDbContext db, ILogger<ApplicationDbInitializer> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            await db.Database.MigrateAsync();

            logger.LogDebug("Adding role: User");
            var idResult = await CreateRole(serviceProvider, "User");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create User role!");
            }
            var idResult2 = await CreateRole(serviceProvider, "Admin");
            if (!idResult2.Succeeded)
            {
                logger.LogDebug("Failed to create User role!");
            }
            var idResult3 = await CreateRole(serviceProvider, "Approver");
            if (!idResult3.Succeeded)
            {
                logger.LogDebug("Failed to create User role!");
            }
            // TODO add other roles
 

            logger.LogDebug("Adding user: jfk");
            idResult = await CreateAccount(serviceProvider, "jfk@example.org", "jfk123", "Admin");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create jfk user!");
            }
            logger.LogDebug("Adding user: jake");
            idResult = await CreateAccount(serviceProvider, "jake@example.org", "jake123", "Admin");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create jfk user!");
            }
            logger.LogDebug("Adding user: nixon");
            idResult = await CreateAccount(serviceProvider, "nixon@example.org", "nixon123", "User");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create nixon user!");
            }
            logger.LogDebug("Adding user: johnson");
            idResult = await CreateAccount(serviceProvider, "johnson@example.org", "john123", "Approver");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create johnson user!");
            }
            logger.LogDebug("Adding user: jerry");
            idResult = await CreateAccount(serviceProvider, "jerry@example.org", "jerry123", "Approver");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create jerry user!");
            }
            // TODO add other users and assign more roles
            

            if (!await db.Tags.AnyAsync(t => t.Name == "portrait")) //make sure tags dont exist first or else they will keep populating the db
            {
                Tag portrait = new Tag { Name = "portrait" };
                await db.Tags.AddAsync(portrait);
            }

            if (!await db.Tags.AnyAsync(t => t.Name == "architecture"))
            {
                Tag architecture = new Tag { Name = "architecture" };
                await db.Tags.AddAsync(architecture);
            }

            if (!await db.Tags.AnyAsync(t => t.Name == "Games"))
            {
                Tag games = new Tag { Name = "games" };
                await db.Tags.AddAsync(games);
            }
            if (!await db.Tags.AnyAsync(t => t.Name == "Show"))
            {
                Tag games = new Tag { Name = "show" };
                await db.Tags.AddAsync(games);
            }
            if (!await db.Tags.AnyAsync(t => t.Name == "Nature"))
            {
                Tag games = new Tag { Name = "nature" };
                await db.Tags.AddAsync(games);
            }
            // TODO add other tags

            await db.SaveChangesAsync();

        }

        private static async Task<IdentityResult> CreateRole(IServiceProvider provider,
                                                            string role)
        {
            RoleManager<IdentityRole> roleManager = provider
                .GetRequiredService
                       <RoleManager<IdentityRole>>();
            var idResult = IdentityResult.Success;
            if (await roleManager.FindByNameAsync(role) == null)
            {
                idResult = await roleManager.CreateAsync(new IdentityRole(role));
            }
            return idResult;
        }

        private static async Task<IdentityResult> CreateAccount(IServiceProvider provider,
                                                               string email, 
                                                               string password,
                                                               string role)
        {
            UserManager<ApplicationUser> userManager = provider
                .GetRequiredService
                       <UserManager<ApplicationUser>>();
            var idResult = IdentityResult.Success;

            if (await userManager.FindByNameAsync(email) == null)
            {
                ApplicationUser user = new ApplicationUser { UserName = email, Email = email };
                idResult = await userManager.CreateAsync(user, password);

                if (idResult.Succeeded)
                {
                    idResult = await userManager.AddToRoleAsync(user, role);
                }
            }

            return idResult;
        }

    }
}