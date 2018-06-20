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
            if (attacker.IsNPC)
            {
                damageMultiplier -= 0.25f;
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
            bool recieverDied = reciever.Health < 1;
            //Check if reciever died
            if (recieverDied)
            {
                match.Winner = attacker;
                int exp = (1 + reciever.Level - attacker.Level) * 500;
                exp = exp < 0 ? 0 : exp;
                attacker.GainExp(exp);

                //Restore NPCs to full health on match end
                if (reciever.IsNPC)
                {
                    //Reset levels on defeat
                    (reciever as Opponent).Reset();
                }
                else if (attacker.IsNPC)
                    attacker.Health = attacker.MaxHealth;

                //Award score
                if (!reciever.IsNPC)
                {
                    reciever.Owner.GainExp(50);
                    reciever.Owner.Score.Add(50);
                }
                if (!attacker.IsNPC)
                {
                    attacker.Owner.GainExp(100);
                    attacker.Owner.Score.Add(100);
                }
            }
            match.ExecuteTurn(damage, roll, recieverDied);
        }

        public static void AttemptYield(Match match)
        {
            Gladiator attacker = match.NextAttacker;
            Gladiator reciever = match.NextAttacker.Id == match.Gladiator.Id ? match.Opponent : match.Gladiator;
            int roll = RNG.Next(100);
            bool success = roll < 50;
            if (success)
            {
                match.Winner = reciever;
                int exp = (1 + reciever.Level - attacker.Level) * 250;
                reciever.GainExp(exp);
                if (reciever.IsNPC)
                    reciever.Health = reciever.MaxHealth;
                else
                {
                    reciever.Owner.GainExp(25);
                    reciever.Owner.Score.Add(25);
                }
            }
            match.ExecuteTurn(success, roll);
        }
    }
}