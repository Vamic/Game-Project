using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class ScoreItem
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }

        public ScoreItem()
        {
            Date = DateTime.Now;
        }

        public ScoreItem(int value) : this()
        { 
            Value = value;
        }
    }
    public class Score
    {
        public int Id { get; set; }

        public List<ScoreItem> Scores { get; set; }

        public int Total
        {
            get { return Scores.Sum(s => s.Value); }
        }

        public Score()
        {
            Scores = new List<ScoreItem>();
        }

        public void Add(int addition)
        {
            Scores.Add(new ScoreItem(addition));
        }
    }

    public class UserScore : Score
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class GladiatorScore : Score
    {
        public virtual Gladiator Gladiator { get; set; }
    }
}