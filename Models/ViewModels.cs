using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class HomePageViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Gladiator> Gladiators { get; set; }
        
        public List<UserScore> AllUserScores { get; set; }
        public List<GladiatorScore> AllGladiatorScores { get; set; }
    }

    public class GladiatorColumnViewModel
    {
        public Gladiator Gladiator { get; set; }

        public string ColumnId { get; set; }
    }

    public class GladiatorOpponentsViewModel
    {
        public List<Gladiator> Gladiators { get; set; }

        public List<Opponent> Opponents { get; set; }
    }

    public class CombatViewModel
    {
        public Gladiator Gladiator { get; set; }

        public Gladiator Opponent { get; set; }
    }

    public class GladiatorDisplayViewModel
    {
        public Gladiator Gladiator { get; set; }

        public bool ShowHPBar { get; set; }

        public bool ShowExpBar { get; set; }

        public bool ShowLevel { get; set; }

        public bool IsDead { get; set; }
    }

    public class ScoreRowViewModel
    {
        public string Name { get; set; }

        public Score Score { get; set; }
    }
}