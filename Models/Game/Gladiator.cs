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

        public Gladiator()
        {
            Level = 1;
            MaxHealth = Health = 100;
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

        public void Update(GladiatorBindingModel model)
        {
            Name = model.Name;
        }
    }
}