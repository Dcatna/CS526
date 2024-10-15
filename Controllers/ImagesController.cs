using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharingWithSecurity.DAL;
using ImageSharingWithSecurity.Models;
using ImageSharingWithSecurity.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImageSharingWithSecurity.Controllers
{
    // TODO require authorization by default
    public class ImagesController : BaseController
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly ILogger<ImagesController> logger;

        // Dependency injection
        public ImagesController(UserManager<ApplicationUser> userManager,
                                ApplicationDbContext db,
                                IWebHostEnvironment environment,
                                ILogger<ImagesController> logger)
            : base(userManager, db)
        {
            this.hostingEnvironment = environment;
            this.logger = logger;
        }

        private void MkDirectories()
        {
            var dataDir = Path.Combine(hostingEnvironment.WebRootPath,
               "data", "images");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
        }

        private string ImageDataFile(int id)
        {
            return Path.Combine(
               hostingEnvironment.WebRootPath,
               "data", "images", "img-" + id + ".jpg");
        }

        public static string ImageContextPath(int id)
        {
            return "data/images/img-" + id + ".jpg";
        }

        // TODO
        public ActionResult Upload()
        {
            CheckAda();

            ViewBag.Message = "";
            ImageView imageView = new ImageView();
            imageView.Tags = new SelectList(db.Tags, "Id", "Name", 1);
            return View(imageView);
        }

        // TODO prevent CSRF

        public async Task<ActionResult> Upload(ImageView imageView)
        {
            CheckAda();

            logger.LogDebug("Processing the upload of an image....");

            await TryUpdateModelAsync(imageView);

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please correct the errors in the form!";
                ViewBag.Tags = new SelectList(db.Tags, "Id", "Name", 1);
                return View();
            }

            logger.LogDebug("...getting the current logged-in user....");
            ApplicationUser user = await GetLoggedInUser();

            if (imageView.ImageFile == null || imageView.ImageFile.Length <= 0)
            {
                ViewBag.Message = "No image file specified!";
                imageView.Tags = new SelectList(db.Tags, "Id", "Name", 1);
                return View(imageView);
            }

            logger.LogDebug("....saving image metadata in the database....");

            Image image = new Image();
            // TODO save image metadata in the database 

            // end TODO

            MkDirectories();

            logger.LogDebug("...saving image file on disk....");

            // TODO save image file on disk

            // end TODO

            logger.LogDebug("...forwarding to the details page, image id = "+image.Id);

            return RedirectToAction("Details", new { Id = image.Id });
        }

        // TODO
        public ActionResult Query()
        {
            CheckAda();

            ViewBag.Message = "";
            return View();
        }

        // TODO
        public ActionResult Details(int Id)
        {
            CheckAda();

            Image image = db.Images.Find(Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "Details:" + Id });
            }

            ImageView imageView = new ImageView();
            imageView.Id = image.Id;
            imageView.Caption = image.Caption;
            imageView.Description = image.Description;
            imageView.DateTaken = image.DateTaken;
            /*
             * Eager loading of related entities
             */
            var imageEntry = db.Entry(image);
            imageEntry.Reference(i => i.Tag).Load();
            imageEntry.Reference(i => i.User).Load();
            imageView.TagName = image.Tag.Name;
            imageView.Username = image.User.UserName;
            return View(imageView);
        }

        // TODO
        public async Task<ActionResult> Edit(int Id)
        {
            CheckAda();
            ApplicationUser user = await GetLoggedInUser();

            Image image =  await db.Images.FindAsync(Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotFound" });
            }

            await db.Entry(image).Reference(im => im.User).LoadAsync();  // Eager load of user
            if (image.User.UserName==null || !image.User.UserName.Equals(user.UserName))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotAuth" });
            }

            ViewBag.Message = "";

            ImageView imageView = new ImageView();
            imageView.Tags = new SelectList(db.Tags, "Id", "Name", image.TagId);
            imageView.Id = image.Id;
            imageView.TagId = image.TagId;
            imageView.Caption = image.Caption;
            imageView.Description = image.Description;
            imageView.DateTaken = image.DateTaken;

            return View("Edit", imageView);
        }

        // TODO prevent CSRF

        public async Task<ActionResult> DoEdit(int Id, ImageView imageView)
        {
            CheckAda();
            ApplicationUser user = await GetLoggedInUser();

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please correct the errors on the page";
                imageView.Id = Id;
                imageView.Tags = new SelectList(db.Tags, "Id", "Name", imageView.TagId);
                return View("Edit", imageView);
            }

            logger.LogDebug("Saving changes to image " + Id);
            Image image = await db.Images.FindAsync(Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotFound" });
            }

            await db.Entry(image).Reference(im => im.User).LoadAsync();  // Explicit load of user
            if (image.User.UserName==null || !image.User.UserName.Equals(user.UserName))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "EditNotAuth" });
            }

            image.TagId = imageView.TagId;
            image.Caption = imageView.Caption;
            image.Description = imageView.Description;
            image.DateTaken = imageView.DateTaken;
            db.Entry(image).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Details", new { Id = Id });
        }

        // TODO
        public async Task<ActionResult> Delete(int Id)
        {
            CheckAda();
            ApplicationUser user = await GetLoggedInUser();

            Image image = await db.Images.FindAsync(Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "Delete" });
            }

            await db.Entry(image).Reference(im => im.User).LoadAsync();  // Explicit load of user
            if (image.User.UserName==null || !image.User.UserName.Equals(user.UserName))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "DeleteNotAuth" });
            }

            ImageView imageView = new ImageView()
            {
                Id = image.Id, Caption = image.Caption, Description = image.Description, DateTaken = image.DateTaken
            };
            /*
             * Eager loading of related entities
             */
            await db.Entry(image).Reference(i => i.Tag).LoadAsync();
            imageView.TagName = image.Tag.Name;
            imageView.Username = image.User.UserName;
            return View(imageView);
        }

        // TODO prevent CSRF

        public async Task<ActionResult> DoDelete(int Id)
        {
            CheckAda();
            ApplicationUser user = await GetLoggedInUser();

            Image image = await db.Images.FindAsync(Id);
            if (image == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "DeleteNotFound" });
            }

            await db.Entry(image).Reference(im => im.User).LoadAsync();  // Explicit load of user
            if (image.User.UserName==null || !image.User.UserName.Equals(user.UserName))
            {
                return RedirectToAction("Error", "Home", new { ErrId = "DeleteNotAuth" });
            }

            //db.Entry(imageEntity).State = EntityState.Deleted;
            db.Images.Remove(image);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }

        // TODO
        public async Task<ActionResult> ListAll()
        {
            CheckAda();
            ApplicationUser user = await GetLoggedInUser();

            IList<Image> images = ApprovedImages().Include(im => im.User).Include(im => im.Tag).ToList();
            ViewBag.UserId = user.Id;
            return View(images);
        }

        // TODO
        public async Task<IActionResult> ListByUser()
        {
            CheckAda();

            // Return form for selecting a user from a drop-down list
            ListByUserModel userView = new ListByUserModel();
            var defaultId = (await GetLoggedInUser()).Id;

            userView.Users = new SelectList(ActiveUsers(), "Id", "UserName", defaultId);
            return View(userView);
        }

        // TODO
        public async Task<ActionResult> DoListByUser(ListByUserModel userView)
        {
            CheckAda();

            // TODO list all images uploaded by the user in userView (see List By Tag)
            return null;
            // End TODO


        }

        // TODO
        public ActionResult ListByTag()
        {
            CheckAda();

            ListByTagModel tagView = new ListByTagModel() { Tags = new SelectList(db.Tags, "Id", "Name", 1) };
            return View(tagView);
        }

        // TODO
        public async Task<ActionResult> DoListByTag(ListByTagModel tagView)
        {
            CheckAda();
            ApplicationUser user = await GetLoggedInUser();

            Tag tag = await db.Tags.FindAsync(tagView.Id);
            if (tag == null)
            {
                return RedirectToAction("Error", "Home", new { ErrId = "ListByTag" });
            }

            ViewBag.UserId = user.Id;
            /*
             * Eager loading of related entities
             */
            var images = db.Entry(tag)
                .Collection(t => t.Images)
                .Query().Where(im => im.Approved)
                .Include(im => im.User)
                .ToList();
            return View("ListAll", tag.Images);
        }

    }

}
