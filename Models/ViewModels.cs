using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class GladiatorColumnViewModel
    {
        public Gladiator Gladiator { get; set; }

        public string ColumnId { get; set; }
    }

    public class GladiatorOpponentsViewModel
    {
        public List<Gladiator> Gladiators { get; set; }

        public List<Gladiator> Opponents { get; set; }
    }

    public class CombatViewModel
    {
        public Gladiator Gladiator { get; set; }

        public Gladiator Opponent { get; set; }
    }

    public class CombatGladiatorDisplayViewModel
    {
        public Gladiator Gladiator { get; set; }

        public bool IsDead { get; set; }
    }
}