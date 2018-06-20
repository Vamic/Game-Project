using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class ScoreHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static List<Score> GetAllScores()
        {
            return db.Scores.Include("Scores").ToList<Score>();
        }
        
        public static Score GetScore(int scoreId)
        {
            return db.Scores.Where(s => s.Id == scoreId).Include("Scores").FirstOrDefault();
        }

        public static ScoreItem GetScoreItem(int scoreItemId)
        {
            return db.Set<ScoreItem>().Find(scoreItemId);
        }

        public static (HttpStatusCodeResult result, Score score) Add(int scoreId, int adjustment)
        {
            Score score = GetScore(scoreId);
            if (score == null)
                return (new HttpStatusCodeResult(404, "Score not found"), null);
            score.Add(adjustment);
            db.Entry(score).State = EntityState.Modified;
            db.SaveChanges();
            return (new HttpStatusCodeResult(200, "Successfully adjusted score"), score);
        }

        public static HttpStatusCodeResult RemoveScoreItem(int scoreItemId)
        {
            ScoreItem scoreItem = GetScoreItem(scoreItemId);

            db.Entry(scoreItem).State = EntityState.Deleted;
            db.SaveChanges();
            return new HttpStatusCodeResult(200, "Successfully removed ScoreItem");
        }
    }
}