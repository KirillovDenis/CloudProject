using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CloudProject.Models;

namespace CloudProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        CloudProjectContext db = new CloudProjectContext();


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(User model, string returnUrl)
        {
        //    db.Users.RemoveRange(db.Users);
        //    db.Tasks.RemoveRange(db.Tasks);
        //    db.SaveChanges();

            var users = db.Users;
            var user = db.Users.Where(u => u.Password == model.Password && u.Login == model.Login).FirstOrDefault();


            if (user != null)
            {
                if (TempData.ContainsKey("id"))
                {
                    TempData.Remove("id");
                }
                TempData.Add("id", user.Id);
                return RedirectToAction("Index", "CloudProject");
            }

            return View();
        }



        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(User model)
        {
            model.Id = Guid.NewGuid();
            if (db.Users.Where(u => u.Login == model.Login).FirstOrDefault() == null)
            {
                db.Users.Add(model);
                db.SaveChanges();
                TempData.Add("id", model.Id);

                return RedirectToAction("Index", "CloudProject");
            }


            return View();
        }



    }
}