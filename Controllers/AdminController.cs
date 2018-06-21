using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GameProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace GameProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        async public Task<ActionResult> Opponents()
        {
            return PartialView(await GladiatorHandler.GetOpponents());
        }

        async public Task<ActionResult> EditOpponent(int id)
        {
            Gladiator opponent = await GladiatorHandler.GetGladiator(id);
            if (opponent == null)
                return RedirectToAction("Opponents");
            OpponentBindingModel model = new OpponentBindingModel
            {
                Id = id,
                Name = opponent.Name,
                Level = opponent.Level,
                Experience = opponent.Experience,
                MaxHealth = opponent.MaxHealth
            };
            return PartialView(model);
        }

        [HttpPost]
        async public Task<ActionResult> EditOpponent([Bind]OpponentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errorList);
            }

            (HttpStatusCodeResult result, Gladiator opponent) = await GladiatorHandler.EditOpponent(model);
            if(result.StatusCode == 200)
            {
                return PartialView("_OpponentRow", opponent);
            } else
            {
                return result;
            }
        }

        async public Task<ActionResult> Users()
        {
            List<ApplicationUser> users = (await UserHandler.GetAllUsers()).Where(user => !UserManager.IsInRole(user.Id, "Admin")).ToList();
            return PartialView(users);
        }

        async public Task<ActionResult> EditUser(string id)
        {
            ApplicationUser user = await UserHandler.GetUser(id);
            if (user == null)
                return RedirectToAction("Users");
            if (UserManager.IsInRole(user.Id, "Admin"))
                return new HttpStatusCodeResult(401, "Cannot edit admins");
            UserBindingModel model = new UserBindingModel
            {
                Id = id,
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Experience = user.Experience
            };
            return PartialView(model);
        }

        [HttpPost]
        async public Task<ActionResult> EditUser([Bind]UserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errorList);
            }
            if (UserManager.IsInRole(model.Id, "Admin"))
                return new HttpStatusCodeResult(401, "Cannot edit admins");

            (HttpStatusCodeResult result, ApplicationUser user) = await UserHandler.EditUser(model);
            if (result.StatusCode == 200)
            {
                return PartialView("_UserRow", user);
            }
            else
            {
                return result;
            }
        }

        async public Task<ActionResult> Scores()
        {
            return PartialView(await ScoreHandler.GetAllScores());
        }

        async public Task<ActionResult> EditScore(int id)
        {
            Score score = await ScoreHandler.GetScore(id);
            if (score == null)
                return RedirectToAction("Scores");

            return PartialView(score);
        }

        [HttpPost]
        async public Task<ActionResult> AdjustScore(int id, int adjustment)
        {
            if (adjustment == 0)
                return new HttpStatusCodeResult(400, "Adjustment cannot be 0");
            (HttpStatusCodeResult result, Score score) = await ScoreHandler.Add(id, adjustment);
            if (result.StatusCode == 200)
            {
                return PartialView("EditScore", score);
            }
            else
            {
                return result;
            }
        }

        [HttpDelete]
        async public Task<ActionResult> RemoveScoreItem(int scoreId, int scoreItemId)
        {
            HttpStatusCodeResult result = await ScoreHandler.RemoveScoreItem(scoreItemId);
            return result;
        }
    }
}
