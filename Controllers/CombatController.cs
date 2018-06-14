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

        [HttpPost]
        public ActionResult Attack()
        {
            string userId = User.Identity.GetUserId();
            Match match = GladiatorHandler.GetActiveMatch(userId);
            if (match == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new string[] { "Tried to attack when there's no match." });
            }
            if (match.NextAttacker.Owner.Id != userId)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new string[] { "Not user's turn." });
            }
            GladiatorHandler.AttackTurn(match);
            //Make opponent attack back
            if(match.Winner == null && match.Opponent.IsNPC)
                GladiatorHandler.AttackTurn(match);

            return PartialView("Index", match);
        }

        [HttpPost]
        public ActionResult Yield()
        {
            string userId = User.Identity.GetUserId();
            Match match = GladiatorHandler.GetActiveMatch(userId);
            if (match == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new string[] { "Tried to yield when there's no match." });
            }
            if (match.NextAttacker.Owner.Id != userId)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new string[] { "Not user's turn." });
            }
            GladiatorHandler.YieldTurn(match);
            //If npc didnt accept the yield, attack
            if (match.Winner == null && match.NextAttacker.IsNPC)
                GladiatorHandler.AttackTurn(match);

            return PartialView("Index", match);
        }
    }
}
