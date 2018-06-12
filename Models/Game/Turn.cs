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

        public Gladiator Attacker { get; set; }

        public Gladiator Reciever { get; set; }

        public Turn()
        {
        }
    }
}