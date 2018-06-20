using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public ActionResult Opponents()
        {
            return PartialView(GladiatorHandler.GetOpponents());
        }

        public ActionResult EditOpponent(int id)
        {
            Gladiator opponent = GladiatorHandler.GetGladiator(id);
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
        public ActionResult EditOpponent([Bind]OpponentBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(errorList);
            }

            (HttpStatusCodeResult result, Gladiator opponent) = GladiatorHandler.EditGladiator(model);
            if(result.StatusCode == 200)
            {
                return PartialView("_OpponentRow", opponent);
            } else
            {
                return result;
            }
        }

        public ActionResult Users()
        {
            List<ApplicationUser> users = UserHandler.GetAllUsers().Where(user => !UserManager.IsInRole(user.Id, "Admin")).ToList();
            return PartialView(users);
        }

        public ActionResult EditUser(string id)
        {
            ApplicationUser user = UserHandler.GetUser(id);
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
        public ActionResult EditUser([Bind]UserBindingModel model)
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

            (HttpStatusCodeResult result, ApplicationUser user) = UserHandler.EditUser(model);
            if (result.StatusCode == 200)
            {
                return PartialView("_UserRow", user);
            }
            else
            {
                return result;
            }
        }

        public ActionResult Scores()
        {
            return PartialView(ScoreHandler.GetAllScores());
        }

        public ActionResult EditScore(int id)
        {
            Score score = ScoreHandler.GetScore(id);
            if (score == null)
                return RedirectToAction("Scores");

            return PartialView(score);
        }

        [HttpPost]
        public ActionResult AdjustScore(int id, int adjustment)
        {
            if (adjustment == 0)
                return new HttpStatusCodeResult(400, "Adjustment cannot be 0");
            (HttpStatusCodeResult result, Score score) = ScoreHandler.Add(id, adjustment);
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
        public ActionResult RemoveScoreItem(int scoreId, int scoreItemId)
        {
            HttpStatusCodeResult result = ScoreHandler.RemoveScoreItem(scoreItemId);
                return result;
        }
    }
}
