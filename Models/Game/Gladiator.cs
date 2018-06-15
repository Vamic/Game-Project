using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class Gladiator
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public bool IsNPC { get; set; }

        public GladiatorScore Score { get; set; }

        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public virtual List<Match> MatchesAsGladiator { get; set; }
        public virtual List<Match> MatchesAsOpponent { get; set; }

        [NotMapped]
        public List<Match> Matches {
            get {
                var matches = new List<Match>();
                matches.AddRange(MatchesAsGladiator);
                matches.AddRange(MatchesAsOpponent);
                return matches;
            }
        }

        public int Wins
        {
            get
            {

                return Matches.Count == 0 ? 0 : Matches.Count(match => match.Winner == null ? false : match.Winner.Id == Id);
            }
        }
        public int Losses
        {
            get
            {
                return Matches.Count == 0 ? 0 : Matches.Count(match => match.Winner == null ? false : match.Winner.Id != Id);
            }
        }

        public void Reset()
        {
            Level = 1;
            Experience = 0;
            MaxHealth = Health = 100;
        }

        public Gladiator()
        {
            Reset();
        }

        public Gladiator(int id)
        {
            Id = id;
        }

        public Gladiator(GladiatorBindingModel model, string ownerId) : this()
        {
            Name = model.Name;
            IsNPC = model.IsNPC;
            OwnerId = ownerId;
        }

        internal void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public void LevelUp()
        {
            Level++;
            MaxHealth += 10;
            //Heal up to 50% max health if below 50%
            Health = Health > ((double)MaxHealth / 2) ? Health : (int)((double)MaxHealth / 2);
        }

        public void GainExp(int exp)
        {
            Experience += exp;
            //Add a level per 1000 experience
            int gainedLevels = (int)((double)Experience / 1000);
            for (int i = 0; i < gainedLevels; i++)
            {
                LevelUp();
            }
            //Remove any multiple of 1000
            Experience = Experience % 1000;
        }

        public void Update(GladiatorBindingModel model)
        {
            Name = model.Name;
        }
    }
}