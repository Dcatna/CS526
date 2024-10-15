using System.Linq;
using System.Threading.Tasks;
using ImageSharingWithSecurity.DAL;
using ImageSharingWithSecurity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageSharingWithSecurity.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db;

        protected UserManager<ApplicationUser> userManager;

        protected BaseController(UserManager<ApplicationUser> userManager, 
                                 ApplicationDbContext db)
        {
            this.db = db;
            this.userManager = userManager;
        }


        protected void CheckAda()
        {
            ViewBag.isADA = GetAdaFlag();
        }

        private bool GetAdaFlag()
        {
            var cookie = Request.Cookies["ADA"];
            return (cookie != null && "true".Equals(cookie));
        }

        protected async Task<ApplicationUser> GetLoggedInUser()
        {
            var user = HttpContext.User;
            if (user.Identity == null || user.Identity.Name == null)
            {
                return null;
            }
            return await userManager.FindByNameAsync(user.Identity.Name);
        }

        protected ActionResult ForceLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private IQueryable<Image> ApprovedImages(IQueryable<Image> images)
        {
            return images.Where(im => im.Valid && im.Approved);
        }

        protected IQueryable<Image> ApprovedImages()
        {
            return ApprovedImages(db.Images);
        }

        protected IQueryable<ApplicationUser> ActiveUsers()
        {
            return userManager.Users.Where(u => u.Active);
        }
    }
}