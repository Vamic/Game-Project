using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class GladiatorHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static Random RNG = new Random();

        async public static Task<Gladiator> GetGladiator(int id)
        {
            return await db.Gladiators.Where(gtor => gtor.Id == id)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent")
                .Include("Score.Scores").Include("Owner")
                .FirstOrDefaultAsync();
        }

        async public static Task<List<Gladiator>> GetAllGladiators()
        {
            return await db.Gladiators.Where(gtor => !gtor.IsNPC)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent")
                .Include("Score.Scores").Include("Owner")
                .ToListAsync();
        }

        async public static Task<List<Gladiator>> GetCurrentGladiators(string userId)
        {
            //Gets all living gladiators for the user
            return await db.Gladiators.Where(gtor => gtor.Owner.Id == userId && !gtor.IsNPC && gtor.Health > 0)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent").ToListAsync();
        }

        async public static Task<List<Opponent>> GetOpponents()
        {
            //Get all opponents
            return await db.Set<Opponent>().Where(gtor => gtor.IsNPC)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent").ToListAsync();
        }

        async public static Task<List<Opponent>> GetOpponents(string userId)
        {
            //Get opponents made by the specified user
            return await db.Set<Opponent>().Where(gtor => gtor.Owner.Id == userId && gtor.IsNPC)
                .Include("MatchesAsGladiator").Include("MatchesAsOpponent")
                .ToListAsync();
        }

        async public static Task<Match> GetActiveMatch(string userId)
        {
            //Gets a match where there is no winner and the user is participating, or null if there is none
            return await db.Matches.Where(match => match.Winner == null
                && match.Gladiator.Owner.Id == userId)
                .Include("Turns").Include("Opponent.Score.Scores")
                .Include("Gladiator.Score.Scores").Include("Gladiator.Owner.Score.Scores")
                .FirstOrDefaultAsync();
        }

        async public static Task<List<Opponent>> GetRandomOpponents(int amount = 10)
        {
            //Get the opponents that are not in battle
            List<Opponent> opponents = (await GetOpponents()).Where(o => !o.Matches.Any(m => m.Winner == null)).ToList();
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

        async public static Task<HttpStatusCodeResult> CreateGladiator(GladiatorBindingModel model, string userId)
        {
            Gladiator gladiator = new Gladiator(model, userId);
            gladiator.Score = new GladiatorScore
            {
                Gladiator = gladiator
            };
            //Count how many gladiators are still alive
            List<Gladiator> gladiators = await GetCurrentGladiators(userId);
            int amountOfGladiators = gladiators.Where(gtor => gtor.Health > 0).Count();

            if(amountOfGladiators < 3 || gladiator.IsNPC)
            {
                db.Gladiators.Add(gladiator);
                await db.SaveChangesAsync();

                return new HttpStatusCodeResult(200, "Successfully created a new Gladiator.");
            }
            else
            {
                return new HttpStatusCodeResult(400, "Too many gladiators still alive.");
            }
        }

        async public static Task<HttpStatusCodeResult> CreateOpponent(OpponentBindingModel model, string userId)
        {
            Opponent gladiator = new Opponent(model, userId);
            gladiator.Score = new GladiatorScore
            {
                Gladiator = gladiator
            };
            //Count how many gladiators are still alive
            List<Gladiator> gladiators = await GetCurrentGladiators(userId);
            int amountOfGladiators = gladiators.Where(gtor => gtor.Health > 0).Count();

            if (amountOfGladiators < 3 || gladiator.IsNPC)
            {
                db.Gladiators.Add(gladiator);
                await db.SaveChangesAsync();

                return new HttpStatusCodeResult(200, "Successfully created a new Gladiator.");
            }
            else
            {
                return new HttpStatusCodeResult(400, "Too many gladiators still alive.");
            }
        }

        async public static Task<HttpStatusCodeResult> EditGladiator(GladiatorBindingModel model, string userId, bool isAdmin)
        {
            Gladiator gladiator = await GetGladiator(model.Id);
            if(gladiator == null)
                return new HttpStatusCodeResult(404, "No gladiator found.");
            if (gladiator.Owner.Id != userId && !isAdmin)
                return new HttpStatusCodeResult(401, "Tried to edit another players gladiator.");

            gladiator.Update(model);
            db.Entry(gladiator).State = EntityState.Modified;
            await db.SaveChangesAsync();
            
            return new HttpStatusCodeResult(200, "Successfully Edited Gladiator.");
        }

        async public static Task<(HttpStatusCodeResult result, Gladiator opponent)> EditOpponent(OpponentBindingModel model)
        {
            Opponent opponent = await GetGladiator(model.Id) as Opponent;
            if (opponent == null)
                return (new HttpStatusCodeResult(404, "No opponent found."), null);

            opponent.Update(model);
            db.Entry(opponent).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return (new HttpStatusCodeResult(200, "Successfully Edited Gladiator."), opponent);
        }

        async public static Task CreateMatch(MatchBindingModel model, string userId)
        {
            Match match = new Match(model);

            db.Matches.Add(match);
            await db.SaveChangesAsync();
        }

        async public static Task AttackTurn(Match match)
        {
            CombatHandler.ExecuteTurn(match);
            //Update the match
            db.Entry(match).State = EntityState.Modified;
            if(match.Winner != null)
                db.Entry(match.Winner.Owner).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        async public static Task YieldTurn(Match match)
        {
            CombatHandler.AttemptYield(match);
            //Update the match
            db.Entry(match).State = EntityState.Modified;
            if (match.Winner != null)
                db.Entry(match.Winner.Owner).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
    }
}