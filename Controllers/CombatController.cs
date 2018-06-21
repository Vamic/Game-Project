using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GameProject.Models;
using Microsoft.AspNet.Identity;

namespace GameProject.Controllers
{
    [Authorize]
    public class CombatController : Controller
    {
        async public Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();
            Match match = await GladiatorHandler.GetActiveMatch(userId);
            if (match == null)
                return RedirectToAction("CreateMatch");

            return PartialView(match);
        }

        async public Task<ActionResult> CreateMatch()
        {
            string userId = User.Identity.GetUserId();
            GladiatorOpponentsViewModel model = new GladiatorOpponentsViewModel
            {
                Gladiators = await GladiatorHandler.GetCurrentGladiators(userId),
                Opponents = await GladiatorHandler.GetRandomOpponents()
            };
            return PartialView("_NewMatch", model);
        }

        [HttpPost]
        async public Task<ActionResult> CreateMatch([Bind(Include = "GladiatorID, OpponentID")]MatchBindingModel model)
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
            await GladiatorHandler.CreateMatch(model, userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        async public Task<ActionResult> Attack()
        {
            string userId = User.Identity.GetUserId();
            Match match = await GladiatorHandler.GetActiveMatch(userId);
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
            await GladiatorHandler.AttackTurn(match);
            //Make opponent attack back
            if(match.Winner == null && match.Opponent.IsNPC)
                await GladiatorHandler.AttackTurn(match);

            return PartialView("Index", match);
        }

        [HttpPost]
        async public Task<ActionResult> Yield()
        {
            string userId = User.Identity.GetUserId();
            Match match = await GladiatorHandler.GetActiveMatch(userId);
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
            await GladiatorHandler.YieldTurn(match);
            //If npc didnt accept the yield, attack
            if (match.Winner == null && match.NextAttacker.IsNPC)
                await GladiatorHandler.AttackTurn(match);

            return PartialView("Index", match);
        }
    }
}
