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
                return Request.IsAuthenticated ? PartialView() : PartialView("_Login");
            }
            return Request.IsAuthenticated ? View() : View("_Login");
        }

        public ActionResult Navbar()
        {
            return PartialView("_Navbar");
        }

        public ActionResult Login()
        {
            ViewBag.Title = "Login";

            return PartialView("_Login");
        }

        public ActionResult Register()
        {
            ViewBag.Title = "Register";

            return PartialView("_Register");
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
