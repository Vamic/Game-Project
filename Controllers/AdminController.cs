using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GameProject.Models;
using Microsoft.AspNet.Identity;

namespace GameProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
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
    }
}
