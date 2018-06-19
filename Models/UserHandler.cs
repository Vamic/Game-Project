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

        public static (HttpStatusCodeResult result, ApplicationUser user) EditUser(UserBindingModel model)
        {
            ApplicationUser user = GetUser(model.Id);
            if (user == null)
                return (new HttpStatusCodeResult(404, "No user found."), null);

            user.DisplayName = model.DisplayName;
            user.UserName = model.UserName;
            user.Experience = model.Experience;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return (new HttpStatusCodeResult(200, "Successfully Edited User."), user);
        }
    }
}