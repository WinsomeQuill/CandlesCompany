using CandlesCompany.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CandlesCompany
{
    public static class DBManager
    {
        public static candlesEntities db { get; set; } = new candlesEntities();
        public static bool Join(string email, string pass)
        {
            int user = (from p in db.Users
                        where p.Email == email && p.Password == pass
                        select p.Id).FirstOrDefault();
            return user != 0;
        }
        public static bool ExistUser(string email)
        {
            int user = (from p in db.Users
                        where p.Email == email
                        select p.Id).FirstOrDefault();
            return user != 0;
        }
        public static bool Registration(string email, string pass, string first_name, string last_name, string middle_name = null)
        {
            if(ExistUser(email))
            {
                return false;
            }

            Users user = new Users();
            user.Email = email;
            user.Password = pass;
            user.First_Name = first_name;
            user.Last_Name = last_name;
            user.Middle_Name = middle_name;
            user.Id_Role = 5;

            db.Users.Add(user);
            db.SaveChanges();
            return true;
        }
        public static object UserInfo(string email)
        {
            List<object> users = new List<object>();
             var parts = db.Users
                .Where(x => x.Email == email)
                .Select(x => new { x.Id, x.Last_Name, x.First_Name, x.Middle_Name, x.Phone, x.Email, x.Roles.Priority }).ToList();
            users.AddRange(parts);
            return users.FirstOrDefault();
        }
    }
}
