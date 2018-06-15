using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class UserHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static List<ApplicationUser> GetAllUsers()
        {
            return db.Users.Include("Score.Scores").ToList();
        }

        public static ApplicationUser GetUser(string userId)
        {
            return db.Users.Where(user => user.Id == userId).Include("Score.Scores").FirstOrDefault();
        }
    }
}