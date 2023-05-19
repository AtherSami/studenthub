using SocialProject.Controllers;
using SocialProject.Data;
using SocialProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SocialProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _applicationDb;

        public AdminController(ApplicationDbContext applicationDb)
        {

            _applicationDb = applicationDb;
        }





        public IActionResult Signup()
        {



            return View();
        }
        [HttpPost]
        public IActionResult Signup(Admin usr)
        {

            _applicationDb.Add(usr);
            _applicationDb.SaveChanges();


            return RedirectToActionPermanent("login", "Admin");
        }


        [HttpGet]
        public IActionResult login()
        {
            Admin _adminModel = new Admin();
            return View(_adminModel);
        }
        [HttpPost]
        public IActionResult login(Admin _adminModel)
        {
            // int UserIdd = Convert.ToInt32(contextAccessor.HttpContext.Session[UseId]);

            var Found = _applicationDb.Admins.Where(m => m.Name == _adminModel.Name && m.Password == _adminModel.Password).FirstOrDefault();


            if (Found == null)
            {
                ViewBag.LoginStatus = 0;
            }
            else
            {

                HttpContext.Session.SetString("AdminUserName", Found.Name);
                HttpContext.Session.SetInt32("AdminId", Found.AdminId);
                HttpContext.Session.SetString("AdminPassword", Found.Password);

                return RedirectToActionPermanent("AdminDashboard", "PostModels");
            }

            return View(_adminModel);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }


        public IActionResult Index()
        {
            return View();
        }


    }
}
