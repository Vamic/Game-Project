using GameProject.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Controllers
{
    public class HomeController : Controller
    {
        async public Task<ActionResult> Index()
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
                User = await UserHandler.GetUser(userId),
                Gladiators = await GladiatorHandler.GetCurrentGladiators(userId),
                AllUserScores = (await UserHandler.GetAllUsers()).Select(u => u.Score).Where(s => s != null).ToList(),
                AllGladiatorScores = (await GladiatorHandler.GetAllGladiators()).Select(g => g.Score).Where(s => s != null).ToList()
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
