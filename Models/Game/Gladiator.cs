using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int Health { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        [Required]
        public bool IsNPC { get; set; }
    }
}