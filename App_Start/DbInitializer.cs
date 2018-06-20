using GameProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GameProject.App_Start
{
    public class DbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            //Add admin role
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            //Add user admin/admin as admin
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser {
                    DisplayName = "Admin Guy",
                    UserName = "admin"
                };

                user.Score = new UserScore
                {
                    User = user
                };

                var result = manager.Create(user, "admin1");
                manager.AddToRole(user.Id, "Admin");

                Gladiator billy = new Gladiator(new GladiatorBindingModel
                {
                    Name = "Billy Herrington",
                    IsNPC = false

                }, user.Id);
                billy.Score = new GladiatorScore
                {
                    Gladiator = billy
                };
                Gladiator mememan = new Gladiator(new GladiatorBindingModel
                {
                    Name = "Mememan",
                    IsNPC = true

                }, user.Id);
                mememan.Score = new GladiatorScore
                {
                    Gladiator = mememan
                };

                context.Gladiators.Add(billy);
                context.Gladiators.Add(mememan);
                context.SaveChanges();

                Match match = new Match(new MatchBindingModel
                {
                    GladiatorID = billy.Id,
                    OpponentID = mememan.Id
                });

                context.Matches.Add(match);
                context.SaveChanges();
            }
        }
    }
}