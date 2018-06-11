using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GameProject.Models;
using Microsoft.AspNet.Identity;

namespace GameProject.Controllers
{
    [Authorize]
    public class GladiatorsController : Controller
    {
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            return PartialView(GladiatorHandler.GetGladiators(userId));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return PartialView(new GladiatorBindingModel());
        }

        public ActionResult PostCreate([Bind(Include = "Name, IsNPC")]GladiatorBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errorList);
            }
            //Opponents can only be made by admins.
            if (!User.IsInRole("Admin")) model.IsNPC = false;

            string userId = User.Identity.GetUserId();
            return GladiatorHandler.CreateGladiator(model, userId);
        }
    }
}
