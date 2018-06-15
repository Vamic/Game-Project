using GameProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            if (!Request.IsAuthenticated)
            {
                if (Request.IsAjaxRequest())
                    return PartialView("Login");
                if (!Request.IsAjaxRequest())
                    return View("Login");
            }

            string userId = User.Identity.GetUserId();
            HomePageViewModel model = new HomePageViewModel
            {
                User = UserHandler.GetUser(userId),
                Gladiators = GladiatorHandler.GetCurrentGladiators(userId),
                AllUserScores = UserHandler.GetAllUsers().Select(u => u.Score).Where(s => s != null).ToList(),
                AllGladiatorScores = GladiatorHandler.GetAllGladiators().Select(g => g.Score).Where(s => s != null).ToList()
            };

            if (Request.IsAjaxRequest())
                return PartialView(model);
            return View(model);
        }

        public ActionResult Navbar()
        {
            return PartialView("_Navbar");
        }

        public ActionResult Login()
        {
            ViewBag.Title = "Login";

            return PartialView();
        }

        public ActionResult Register()
        {
            ViewBag.Title = "Register";

            return PartialView();
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
