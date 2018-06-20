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

        public static List<Gladiator> GetAllGladiators()
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

        public static List<Opponent> GetOpponents()
        {
            //Get all opponents
            return db.Set<Opponent>().Where(gtor => gtor.IsNPC)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent").ToList();
        }

        public static List<Opponent> GetOpponents(string userId)
        {
            //Get opponents made by the specified user
            return db.Set<Opponent>().Where(gtor => gtor.Owner.Id == userId && gtor.IsNPC)
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

        public static List<Opponent> GetRandomOpponents(int amount = 10)
        {
            //Get the opponents that are not in battle
            List<Opponent> opponents = GetOpponents().Where(o => !o.Matches.Any(m => m.Winner == null)).ToList();
            List<Opponent> randoms = new List<Opponent>();

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

        public static HttpStatusCodeResult CreateOpponent(OpponentBindingModel model, string userId)
        {
            Opponent gladiator = new Opponent(model, userId);
            gladiator.Score = new GladiatorScore
            {
                Gladiator = gladiator
            };
            //Count how many gladiators are still alive
            List<Gladiator> gladiators = GetCurrentGladiators(userId);
            int amountOfGladiators = gladiators.Where(gtor => gtor.Health > 0).Count();

            if (amountOfGladiators < 3 || gladiator.IsNPC)
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

        public static (HttpStatusCodeResult result, Gladiator opponent) EditOpponent(OpponentBindingModel model)
        {
            Opponent opponent = GetGladiator(model.Id) as Opponent;
            if (opponent == null)
                return (new HttpStatusCodeResult(404, "No opponent found."), null);

            opponent.Update(model);
            db.Entry(opponent).State = EntityState.Modified;
            db.SaveChanges();

            return (new HttpStatusCodeResult(200, "Successfully Edited Gladiator."), opponent);
        }

        public static void CreateMatch(MatchBindingModel model, string userId)
        {
            Match match = new Match(model);

            db.Matches.Add(match);
            db.SaveChanges();
        }
    }
}