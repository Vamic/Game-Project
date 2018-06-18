using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class GladiatorHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static Random RNG = new Random();

        public static Gladiator GetGladiator(int id)
        {
            return db.Gladiators.Where(gtor => gtor.Id == id)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent")
                .Include("Score.Scores").Include("Owner")
                .FirstOrDefault();
        }

        internal static List<Gladiator> GetAllGladiators()
        {
            return db.Gladiators.Where(gtor => !gtor.IsNPC)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent")
                .Include("Score.Scores").Include("Owner")
                .ToList();
        }

        public static List<Gladiator> GetCurrentGladiators(string userId)
        {
            //Gets all living gladiators for the user
            return db.Gladiators.Where(gtor => gtor.Owner.Id == userId && !gtor.IsNPC && gtor.Health > 0)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent").ToList();
        }

        public static List<Gladiator> GetOpponents()
        {
            //Get all opponents
            return db.Gladiators.Where(gtor => gtor.IsNPC)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent").ToList();
        }

        public static List<Gladiator> GetOpponents(string userId)
        {
            //Get opponents made by the specified user
            return db.Gladiators.Where(gtor => gtor.Owner.Id == userId && gtor.IsNPC)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent")
                .ToList();
        }

        public static Match GetActiveMatch(string userId)
        {
            //Gets a match where there is no winner and the user is participating, or null if there is none
            return db.Matches.Where(match => match.Winner == null
                && match.Gladiator.Owner.Id == userId)
                .Include("Turns").Include("Opponent.Score.Scores")
                .Include("Gladiator.Score.Scores").Include("Gladiator.Owner.Score.Scores")
                .FirstOrDefault();
        }

        public static List<Gladiator> GetRandomOpponents(int amount = 10)
        {
            //Get the opponents that are not in battle
            List<Gladiator> opponents = GetOpponents().Where(o => !o.Matches.Any(m => m.Winner == null)).ToList();
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

        public static void AttackTurn(Match match)
        {
            CombatHandler.ExecuteTurn(match);
            //Update the match
            db.Entry(match).State = EntityState.Modified;
            db.SaveChanges();
        }

        internal static void YieldTurn(Match match)
        {
            CombatHandler.AttemptYield(match);
            //Update the match
            db.Entry(match).State = EntityState.Modified;
            db.SaveChanges();
        }

        public static HttpStatusCodeResult CreateGladiator(GladiatorBindingModel model, string userId)
        {
            Gladiator gladiator = new Gladiator(model, userId);
            gladiator.Score = new GladiatorScore
            {
                Gladiator = gladiator
            };
            //Count how many gladiators are still alive
            List<Gladiator> gladiators = GetCurrentGladiators(userId);
            int amountOfGladiators = gladiators.Where(gtor => gtor.Health > 0).Count();

            if(amountOfGladiators < 3 || gladiator.IsNPC)
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

        public static HttpStatusCodeResult EditGladiator(GladiatorBindingModel model, string userId, bool isAdmin)
        {
            Gladiator gladiator = GetGladiator(model.Id);
            if(gladiator == null)
                return new HttpStatusCodeResult(404, "No gladiator found.");
            if (gladiator.Owner.Id != userId && !isAdmin)
                return new HttpStatusCodeResult(401, "Tried to edit another players gladiator.");

            gladiator.Update(model);
            db.Entry(gladiator).State = EntityState.Modified;
            db.SaveChanges();
            
            return new HttpStatusCodeResult(200, "Successfully Edited Gladiator.");
        }

        public static (HttpStatusCodeResult result, Gladiator opponent) EditGladiator(OpponentBindingModel model)
        {
            Gladiator gladiator = GetGladiator(model.Id);
            if (gladiator == null)
                return (new HttpStatusCodeResult(404, "No gladiator found."), null);
            if (!gladiator.IsNPC)
                return (new HttpStatusCodeResult(400, "Selected gladiator is not an NPC."), null);

            gladiator.Update(model);
            db.Entry(gladiator).State = EntityState.Modified;
            db.SaveChanges();

            return (new HttpStatusCodeResult(200, "Successfully Edited Gladiator."), gladiator);
        }

        public static void CreateMatch(MatchBindingModel model, string userId)
        {
            Match match = new Match(model);

            db.Matches.Add(match);
            db.SaveChanges();
        }
    }
}