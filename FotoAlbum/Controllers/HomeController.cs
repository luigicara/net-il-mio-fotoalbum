using FotoAlbum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FotoAlbum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PhotoAlbumContext _db;

        public HomeController(ILogger<HomeController> logger, PhotoAlbumContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
     
            ModelsHelper helper = new ModelsHelper();

            List<Photo> photos = new List<Photo>();

            if (User.IsInRole("ADMIN"))
                photos = _db.Photos.Include(p => p.ImageEntry).ToList();
            else
                photos = _db.Photos.Where(p => p.Visible == true).Include(p => p.ImageEntry).ToList();

            helper.Photos = photos;

            return View(helper);

        }

        public IActionResult Details(int id)
        {
            var photo = _db.Photos.Where(p => p.Id == id).Include(p => p.ImageEntry).FirstOrDefault();
            return View(photo);
        }

        public IActionResult FilterTitle(ModelsHelper data)
        {

            List<Photo> photos = _db.Photos.Where(p => p.Title.Contains(data.Value) && p.Visible == true).Include(p => p.ImageEntry).ToList();

            ModelsHelper helper = new ModelsHelper();

            helper.Photos = photos;

            data.Value = "";

            return View("Index", helper);

        }

        public IActionResult SendMessage(ModelsHelper data)
        {

            Message message = new Message();

            message.Email = data.Message.Email;

            message.Text = data.Message.Text;

            _db.Messages.Add(message);

            _db.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "ADMIN,USER")]
        public IActionResult Admin()
        {

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser user = _db.ApplicationUsers.Where(a => a.Id == id).FirstOrDefault();

            var photos = _db.Photos.Where(p => p.ApplicationUserId == id).Include(p => p.ImageEntry).ToList();

            return View(photos);

        }

        [Authorize(Roles = "ADMIN,USER")]
        public IActionResult Create()
        {
            
            PhotoFormModel data = new PhotoFormModel();
            var categoriesList = _db.Categories.ToList();

            List<SelectListItem> listCategories = new List<SelectListItem>();
            foreach (Category cat in categoriesList)
            {
                listCategories.Add(new SelectListItem() { Text = cat.Name, Value = cat.Id.ToString() });
            }

            data.Photo = new Photo();
            data.Categories = listCategories;

            return View(data);
            
        }

        [Authorize(Roles = "ADMIN,USER")]
        [HttpPost]
        public IActionResult Create(PhotoFormModel data)
        {

            if (!ModelState.IsValid)
            {

                var categoriesList = _db.Categories.ToList();

                List<SelectListItem> listCategories = new List<SelectListItem>();
                foreach (Category cat in categoriesList)
                {
                    listCategories.Add(new SelectListItem() { Text = cat.Name, Value = cat.Id.ToString() });
                }
                data.Categories = listCategories;
                return View(data);
            }

            Photo photo = new Photo();
            photo.Title = data.Photo.Title;
            photo.Description = data.Photo.Description;
            photo.Visible = data.Photo.Visible;
            photo.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            photo.Categories = new List<Category>();

            if (data.SelectedCategories != null)
            {
                foreach (var categoryid in data.SelectedCategories)
                {
                    int intcategoryid = int.Parse(categoryid);
                    var category = _db.Categories.Where(c => c.Id == intcategoryid).FirstOrDefault();
                    photo.Categories.Add(category);
                }
            }

            if (data.Photo.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    data.Photo.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var newImage = new ImageEntry()
                    {
                        Data = fileBytes
                    };
                    _db.ImageEntries.Add(newImage);
                    _db.SaveChanges();
                    photo.ImageEntryId = newImage.Id;
                }

            }

            _db.Photos.Add(photo);
            _db.SaveChanges();
            return RedirectToAction("Admin");
            
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public IActionResult AdminVisibility(int id, ModelsHelper helper)
        {
            if (helper.Visibility == null)
                return RedirectToAction("Index");
 
            Photo photo = _db.Photos.Where(p => p.Id == id).FirstOrDefault();
            if (helper.Visibility == "Visible")
                photo.Visible = true;
            else
                photo.Visible = false;

            _db.Photos.Update(photo);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "ADMIN,USER")]
        public IActionResult Edit(int id)
        {

            Photo photo = _db.Photos.Where(p => p.Id == id).Include(p => p.Categories).FirstOrDefault();
            PhotoFormModel data = new PhotoFormModel();
            var categoriesList = _db.Categories.ToList();

            List<SelectListItem> listCategories = new List<SelectListItem>();
            foreach (Category cat in categoriesList)
            {
                listCategories.Add(new SelectListItem() { Text = cat.Name, Value = cat.Id.ToString(), Selected = photo.Categories.Any(c => c.Id == cat.Id) });
            }
            data.Photo = photo;
            data.Categories = listCategories;
            return View(data);

        }

        [Authorize(Roles = "ADMIN,USER")]
        [HttpPost]
        public IActionResult Edit(int id, PhotoFormModel data)
        {
            
            if (!ModelState.IsValid)
            {

                var categoriesList = _db.Categories.ToList();

                List<SelectListItem> listCategories = new List<SelectListItem>();
                foreach (Category cat in categoriesList)
                {
                    listCategories.Add(new SelectListItem() { Text = cat.Name, Value = cat.Id.ToString(), });
                }
                data.Categories = listCategories;
                return View(data);

            }

            Photo photo = _db.Photos.Where(p => p.Id == id).Include(p => p.Categories).FirstOrDefault();
            photo.Title = data.Photo.Title;
            photo.Description = data.Photo.Description;
            photo.Visible = data.Photo.Visible;
            photo.Categories.Clear();


            if (data.SelectedCategories != null)
            {
                foreach (var categoryid in data.SelectedCategories)
                {
                    int intcategoryid = int.Parse(categoryid);
                    var category = _db.Categories.Where(c => c.Id == intcategoryid).FirstOrDefault();
                    photo.Categories.Add(category);
                }

            }

            if (data.Photo.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    data.Photo.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var newImage = new ImageEntry()
                    {
                        Data = fileBytes
                    };
                    _db.ImageEntries.Add(newImage);
                    _db.SaveChanges();
                    photo.ImageEntryId = newImage.Id;
                }

            }

            _db.Photos.Update(photo);
            _db.SaveChanges();
            return RedirectToAction("Admin");

        }

        [Authorize(Roles = "ADMIN,USER")]
        public IActionResult Delete(int id)
        {
            
                Photo photo = _db.Photos.Where(p => p.Id == id).FirstOrDefault();
                if (photo != null)
                {
                    _db.Photos.Remove(photo);
                    _db.SaveChanges();
                    return RedirectToAction("Admin");
                }
                return RedirectToAction("Admin");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}