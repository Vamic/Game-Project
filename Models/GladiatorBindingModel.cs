using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class GladiatorBindingModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Gladiator Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Create as Opponent")]
        public bool IsNPC { get; set; }
    }
}