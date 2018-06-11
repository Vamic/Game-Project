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
            if (Request.IsAjaxRequest())
            {
                return Request.IsAuthenticated ? PartialView() : PartialView("Login");
            }
            return Request.IsAuthenticated ? View() : View("Login");
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
