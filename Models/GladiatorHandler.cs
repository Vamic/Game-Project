using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class GladiatorHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        
        public static Gladiator[] GetGladiators(string userId)
        {
            return db.Gladiators.Where(gtor => gtor.Owner.Id == userId && !gtor.IsNPC).ToArray();
        }

        public static HttpStatusCodeResult CreateGladiator(GladiatorBindingModel model, string userId)
        {
            Gladiator gladiator = new Gladiator(model, userId);

            //Count how many gladiators are still alive
            Gladiator[] gladiators = GetGladiators(userId);
            int amountOfGladiators = gladiators.Where(gtor => gtor.Health > 0).Count();

            if(amountOfGladiators < 3)
            {
                db.Gladiators.Add(gladiator);
                db.SaveChanges();

                return new HttpStatusCodeResult(200, "Successfully created a new Gladiator.");
            }
            else
            {
                return new HttpStatusCodeResult(400, "Too many gladiators still alive.");
            }
        }
    }
}