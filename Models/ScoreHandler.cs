using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class ScoreHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        async public static Task<List<Score>> GetAllScores()
        {
            return await db.Scores.Include("Scores").ToListAsync();
        }

        async public static Task<Score> GetScore(int scoreId)
        {
            return await db.Scores.Where(s => s.Id == scoreId).Include("Scores").FirstOrDefaultAsync();
        }

        async public static Task<ScoreItem> GetScoreItem(int scoreItemId)
        {
            return await db.Set<ScoreItem>().FindAsync(scoreItemId);
        }

        async public static Task<(HttpStatusCodeResult result, Score score)> Add(int scoreId, int adjustment)
        {
            Score score = await GetScore(scoreId);
            if (score == null)
                return (new HttpStatusCodeResult(404, "Score not found"), null);
            score.Add(adjustment);
            db.Entry(score).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return (new HttpStatusCodeResult(200, "Successfully adjusted score"), score);
        }

        async public static Task<HttpStatusCodeResult> RemoveScoreItem(int scoreItemId)
        {
            ScoreItem scoreItem = await GetScoreItem(scoreItemId);

            db.Entry(scoreItem).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return new HttpStatusCodeResult(200, "Successfully removed ScoreItem");
        }
    }
}