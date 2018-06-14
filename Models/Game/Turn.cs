using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class Turn
    {
        public int Id { get; set; }

        public int Damage { get; set; }

        public int Roll { get; set; }
        
        public Match Match { get; set; }
        
        public int? AttackerID { get; set; }
        
        public int? RecieverID { get; set; }

        public virtual Gladiator Attacker { get; set; }

        public virtual Gladiator Reciever { get; set; }

        public Turn() { }

        public Turn(Match match, int? attackerID, int? recieverID)
        {
            Match = match;
            AttackerID = attackerID;
            RecieverID = recieverID;
        }
    }
}