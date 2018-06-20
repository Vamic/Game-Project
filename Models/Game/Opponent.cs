using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class Opponent : Gladiator
    {
        public int MinLevel { get; set; }

        new public void LevelUp()
        {
            Level++;
        }

        public void Reset()
        {
            Level = MinLevel;
            Experience = 0;
            Health = MaxHealth;
        }

        public Opponent()
        {
        }

        public Opponent(int id)
        {
            Id = id;
        }

        public Opponent(OpponentBindingModel model, string ownerId)
        {
            Update(model);
            IsNPC = true;
            OwnerId = ownerId;
        }
        
        public void Update(OpponentBindingModel model)
        {
            Name = model.Name;
            Level = MinLevel = model.Level;
            Experience = model.Experience;
            Health = MaxHealth = model.MaxHealth;
        }
    }
}