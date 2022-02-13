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
            int user = db.Users
                .Where(x => x.Email == email && x.Password == pass)
                .Select(x => x.Id).FirstOrDefault();

            return user != 0;
        }
        public static bool ExistUser(string email)
        {
            int user = db.Users
                .Where(x => x.Email == email)
                .Select(x => x.Id).FirstOrDefault();
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
             var parts = db.Users
                .Where(x => x.Email == email)
                .Select(x => new { x.Id, x.Last_Name, x.First_Name, x.Middle_Name, x.Phone, x.Email, x.Roles.Priority, x.Roles.Name }).ToList();
            return parts.FirstOrDefault();
        }
        public static object UserInfo(int id)
        {
            var parts = db.Users
               .Where(x => x.Id == id)
               .Select(x => new { x.Id, x.Last_Name, x.First_Name, x.Middle_Name, x.Phone, x.Email, x.Roles.Priority, x.Roles.Name }).ToList();
            return parts.FirstOrDefault();
        }
        public static List<string> GetRoles()
        {
            return db.Roles.Where(x => x.Id != 1 && x.Id != 6).Select(x => x.Name).ToList();
        }
        public static void ChangeRoleById(int id, string role)
        {
            Users user = db.Users.SingleOrDefault(x => x.Id == id);
            user.Id_Role = db.Roles.Where(x => x.Name == role).Select(x => x.Id).FirstOrDefault();
            db.SaveChanges();
        }
        public static void ChangeRoleById(string email, string role)
        {
            Users user = db.Users.SingleOrDefault(x => x.Email == email);
            user.Id_Role = db.Roles.Where(x => x.Name == role).Select(x => x.Id).FirstOrDefault();
            db.SaveChanges();
        }
        public static List<Users> GetEmployees()
        {
            return db.Users.Where(x => x.Id_Role != 1 && x.Id_Role != 6).Select(x => x).ToList();
        }
        public static List<Users> GetUsers()
        {
            return db.Users.Where(x => x.Id_Role == 6).Select(x => x).ToList();
        }
    }
}
