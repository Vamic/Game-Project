using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameProject.Models
{
    public class LoginBindingModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

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

    public class OpponentBindingModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Gladiator Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Experience")]
        public int Experience { get; set; }
        [Required]
        [Display(Name = "Level")]
        public int Level { get; set; }
        [Required]
        [Display(Name = "Max Health")]
        public int MaxHealth { get; set; }
    }

    public class UserBindingModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Experience")]
        public int Experience { get; set; }
    }

    public class MatchBindingModel
    {
        [Required]
        public int? GladiatorID { get; set; }

        public int? OpponentID { get; set; }
    }
}