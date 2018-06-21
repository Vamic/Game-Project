using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GameProject.Models
{
    public static class UserHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        async public static Task<List<ApplicationUser>> GetAllUsers()
        {
            return await db.Users.Include("Score.Scores").ToListAsync();
        }

        async public static Task<ApplicationUser> GetUser(string userId)
        {
            return await db.Users.Where(user => user.Id == userId).Include("Score.Scores").FirstOrDefaultAsync();
        }

        async public static Task<(HttpStatusCodeResult result, ApplicationUser user)> EditUser(UserBindingModel model)
        {
            ApplicationUser user = await GetUser(model.Id);
            if (user == null)
                return (new HttpStatusCodeResult(404, "No user found."), null);

            user.DisplayName = model.DisplayName;
            user.UserName = model.UserName;
            user.Experience = model.Experience;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return (new HttpStatusCodeResult(200, "Successfully Edited User."), user);
        }
    }
}