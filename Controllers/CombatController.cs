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
    public class CombatController : Controller
    {
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            Match match = GladiatorHandler.GetActiveMatch(userId);
            return PartialView(match);
        }

        public ActionResult CreateMatch()
        {
            string userId = User.Identity.GetUserId();
            GladiatorOpponentsViewModel model = new GladiatorOpponentsViewModel
            {
                Gladiators = GladiatorHandler.GetCurrentGladiators(userId),
                Opponents = GladiatorHandler.GetRandomOpponents()
            };
            return PartialView("_NewMatch", model);
        }

        [HttpPost]
        public ActionResult CreateMatch([Bind(Include = "GladiatorID, OpponentID")]MatchBindingModel model)
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
            GladiatorHandler.CreateMatch(model, userId);
            return RedirectToAction("Index");
        }
    }
}
