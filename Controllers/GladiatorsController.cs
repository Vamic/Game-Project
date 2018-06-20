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
            GladiatorOpponentsViewModel model = new GladiatorOpponentsViewModel
            {
                Gladiators = GladiatorHandler.GetCurrentGladiators(userId)
            };
            if (User.IsInRole("Admin"))
                model.Opponents = GladiatorHandler.GetOpponents(userId);
            return PartialView(model);
        }
        
        public ActionResult Create()
        {
            return PartialView(new GladiatorBindingModel());
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Name")]GladiatorBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errorList);
            }

            string userId = User.Identity.GetUserId();
            return GladiatorHandler.CreateGladiator(model, userId);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateOpponent([Bind]OpponentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errorList);
            }

            string userId = User.Identity.GetUserId();
            return GladiatorHandler.CreateOpponent(model, userId);
        }

        public ActionResult Edit(int id)
        {
            return PartialView(new GladiatorBindingModel());
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id, Name, IsNPC")]GladiatorBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errorList);
            }

            bool isAdmin = User.IsInRole("Admin");
            string userId = User.Identity.GetUserId();
            return GladiatorHandler.EditGladiator(model, userId, isAdmin);
        }
    }
}
