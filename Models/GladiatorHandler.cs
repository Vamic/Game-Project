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
        private static Random RNG = new Random();
        
        public static List<Gladiator> GetCurrentGladiators(string userId)
        {
            //Gets all living gladiators for the user
            return db.Gladiators.Where(gtor => gtor.Owner.Id == userId && !gtor.IsNPC && gtor.Health > 0).ToList();
        }
        
        public static List<Gladiator> GetOpponents(string userId)
        {
            //Get all opponents or from specified user
            return userId == null ? db.Gladiators.Where(gtor => gtor.IsNPC).ToList()
                                  : db.Gladiators.Where(gtor => gtor.Owner.Id == userId && gtor.IsNPC).ToList();
        }

        public static Match GetActiveMatch(string userId)
        {
            //Gets a match where there is no winner and the user is participating, or null if there is none
            return db.Matches.FirstOrDefault(match => match.Winner == null && match.Gladiator.Owner.Id == userId);
        }

        public static List<Gladiator> GetRandomOpponents(int amount = 10)
        {
            List<Gladiator> opponents = GetOpponents(null);
            List<Gladiator> randoms = new List<Gladiator>();

            //Take random opponents until the amount is hit or we run out of opponents
            while(randoms.Count < amount && opponents.Count > 0)
            {
                int i = RNG.Next(opponents.Count);
                randoms.Add(opponents[i]);
                opponents.RemoveAt(i);
            }
            return randoms;
        }

        public static HttpStatusCodeResult CreateGladiator(GladiatorBindingModel model, string userId)
        {
            Gladiator gladiator = new Gladiator(model, userId);

            //Count how many gladiators are still alive
            List<Gladiator> gladiators = GetCurrentGladiators(userId);
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

        internal static void CreateMatch(MatchBindingModel model, string userId)
        {
            Match match = new Match(model);

            db.Matches.Add(match);
            db.SaveChanges();
        }
    }
}