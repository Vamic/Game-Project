using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class Match
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        
        public virtual List<Turn> Turns { get; set; }
        
        public int? GladiatorID { get; set; }
        
        public int? OpponentID { get; set; }

        public int? NextAttackerID { get; set; }

        public virtual Gladiator Gladiator { get; set; }

        public virtual Gladiator Opponent { get; set; }

        public Gladiator NextAttacker { get; set; }

        public Gladiator Winner { get; set; }

        public Match()
        {
            Date = DateTime.Now;
        }
        
        public Match(MatchBindingModel model) : this()
        {
            GladiatorID = NextAttackerID = model.GladiatorID;
            OpponentID = model.OpponentID;
        }

        public void ExecuteTurn(int damage, int roll)
        {
            int attacker = NextAttacker.Id;
            int reciever = attacker == Gladiator.Id ? Opponent.Id : Gladiator.Id;
            Turn nextTurn = new Turn(this, attacker, reciever);
            nextTurn.Damage = damage;
            nextTurn.Roll = roll;
            Turns.Add(nextTurn);
            NextAttackerID = reciever;
        }
    }
}