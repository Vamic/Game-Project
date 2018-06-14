using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class CombatHandler
    {
        private static Random RNG = new Random();

        private static (int damage, int roll) CalculateDamage(Gladiator attacker)
        {
            //6 sided die
            int roll = 1 + RNG.Next(6);
            float damageMultiplier = 1;
            //Last stand bonus
            if (attacker.Health / attacker.MaxHealth < 0.1)
            {
                damageMultiplier += 0.5f;
            }
            int damage = (int)((roll + attacker.Level) * damageMultiplier);
            return (damage, roll);
        }
        
        public static void ExecuteTurn(Match match)
        {
            Gladiator attacker = match.NextAttacker;
            //Reciever is the one that isn't the attacker
            Gladiator reciever = match.NextAttacker.Id == match.Gladiator.Id ? match.Opponent : match.Gladiator;
            (int damage, int roll) = CalculateDamage(attacker);
            reciever.TakeDamage(damage);
            //Check if reciever died
            if (reciever.Health < 1)
            {
                match.Winner = attacker;
                //Restore NPCs to full health on defeat so you can fight them again
                if (reciever.IsNPC)
                    reciever.Health = reciever.MaxHealth;
            }
            match.ExecuteTurn(damage, roll);
        }
    }
}