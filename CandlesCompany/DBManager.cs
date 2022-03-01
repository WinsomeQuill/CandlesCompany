using CandlesCompany.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CandlesCompany
{
    public static class DBManager
    {
        public static candlesEntities db { get; } = new candlesEntities();
        public async static Task<bool> Join(string email, string pass)
        {
            return await db.Users.Where(x => x.Email == email && x.Password == pass).Select(x => x.Id).SingleOrDefaultAsync() != 0;
        }
        public static bool ExistUser(string email)
        {
            return db.Users.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault() != 0;
        }
        public static void Registration(string email, string pass, string first_name, string last_name, string middle_name = null)
        {
            Users user = new Users
            {
                Email = email.ToLower(),
                Password = pass.ToLower(),
                First_Name = first_name,
                Last_Name = last_name,
                Middle_Name = middle_name,
                Id_Role = 5
            };

            db.Users.Add(user);
            db.SaveChanges();
        }
        public async static Task<Users> UserInfo(string email)
        {
            return await db.Users.Where(x => x.Email == email).Select(x => x).FirstOrDefaultAsync();
        }
        public static Users UserInfo(int id)
        {
            return db.Users.Where(x => x.Id == id).Select(x => x).FirstOrDefault();
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
        public static int GetUsersCount()
        {
            return db.Users.Count();
        }
        public async static Task<List<Users>> GetUsersForPage(int page)
        {
            return await db.Users.Where(x => x.Id_Role == 6).Select(x => x).OrderBy(x => x.Id).Skip(50 * (page - 1)).Take(50).ToListAsync();
        }
        public async static Task<List<Users>> FindUsers(string value)
        {
            List<Users> users = new List<Users>();
            users.AddRange(await db.Users.Where(x => x.First_Name.Contains(value) || x.First_Name.Contains(value) || x.Middle_Name.Contains(value)).Select(x => x).Distinct().Take(500).ToListAsync());
            return users;
        }
        public static List<Users> GetUsersByRoleDelivery()
        {
            return db.Users.Where(x => x.Id_Role == 3).Select(x => x).ToList();
        }
        public static List<Users> GetUsersByRoleManager()
        {
            return db.Users.Where(x => x.Id_Role == 4).Select(x => x).ToList();
        }
        public static List<Users> GetUsersByRoleAdministrator()
        {
            return db.Users.Where(x => x.Id_Role == 5).Select(x => x).ToList();
        }
        public async static Task<List<Users>> GetEmployees(int page, int start_id_role = 1, int end_id_role = 6)
        {
            return await db.Users.Where(x => x.Id_Role > start_id_role && x.Id_Role < end_id_role).OrderBy(x => x.Id).Skip(50 * (page - 1)).Take(50).ToListAsync();
        }
        public async static Task<List<Users>> FindEmployees(string value)
        {
            List<Users> users = new List<Users>();
            users.AddRange(await db.Users.Where(x => (x.Roles.Id > 1 && x.Roles.Id < 6) && (x.First_Name.Contains(value) || x.First_Name.Contains(value) || x.Middle_Name.Contains(value)))
                .Select(x => x).Distinct().Take(500).ToListAsync());
            return users;
        }
        public static int GetEmployeesCount(int start_id_role = 1, int end_id_role = 6)
        {
            return db.Users.Where(x => x.Id_Role > start_id_role && x.Id_Role < end_id_role).Count();
        }
        public static byte[] GetImageItem(int id)
        {
            byte[] result = db.Candles.Where(x => x.Id == id).Select(x => x.Image).FirstOrDefault();
            if (result.Length == 0 || result == null) { return null; }
            return result;
        }
        public static List<Candles> GetCandles()
        {
            return db.Candles.Select(x => x).ToList();
        }
        public static List<Type_Candle> GetTypeCandles()
        {
            return db.Type_Candle.Select(x => x).ToList();
        }
        public static void UpdateItem(int id, int id_type, string name, string description, int count, double price, byte[] image)
        {
            Candles candle = db.Candles.SingleOrDefault(x => x.Id == id);
            candle.Image = image;
            candle.Price = (decimal)price;
            candle.Name = name;
            candle.Description = description;
            candle.Count = count;
            candle.Id_Type_Candle = id_type;
            db.SaveChanges();
        }
        public static void AddItem(int id_type, string name, string description, int count, double price, byte[] image)
        {
            Candles candle = new Candles
            {
                Description = description,
                Name = name,
                Image = image,
                Count = count,
                Price = (decimal)price,
                Id_Type_Candle = id_type
            };
            db.Candles.Add(candle);
            db.SaveChanges();
        }
        public static void RemoveItem(int id)
        {
            Candles candle = db.Candles.Where(x => x.Id == id).SingleOrDefault();
            db.Candles.Remove(candle);
            db.SaveChanges();
        }
        public static Dictionary<Candles, int> GetCandlesBasket(int id_user)
        {
            return db.Users_Baskets.Where(x => x.Id_User == id_user && x.Candles.Id == x.Id_Candles).ToDictionary(x => x.Candles, x => x.Count);
        }
        public static void AddCandlesBasket(int id_user, Candles candle)
        {
            db.Users_Baskets.Add(new Users_Baskets { Id_Candles = candle.Id, Id_User = id_user, Count = 1 });
            db.SaveChanges();
            UserCache.Basket.Add(candle, 1);
        }
        public static void UpdateCandlesBasket(int id_user, Candles candle, int count)
        {
            Users_Baskets basket = db.Users_Baskets.Where(x => x.Id_User == id_user && x.Id_Candles == candle.Id).SingleOrDefault();
            basket.Count = count;
            db.SaveChanges();
            UserCache.Basket.Remove(candle);
            UserCache.Basket.Add(candle, count);
        }
        public static void RemoveCandlesBasket(int id_user, Candles candle)
        {
            Users_Baskets basket = db.Users_Baskets.Where(x => x.Id_User == id_user && x.Id_Candles == candle.Id).SingleOrDefault();
            db.Users_Baskets.Remove(basket);
            UserCache.Basket.Remove(candle);
            db.SaveChanges();
        }
        public static List<Orders> GetOrders(int id_user)
        {
            return db.Orders.Where(x => x.Id_User == id_user).ToList();
        }
        public static Orders AddOrder(int id_user, int id_candle, int count, double price, Order_Address address)
        {
            Candles_Order candles_Order = new Candles_Order
            {
                Count = count,
                Id_Candles = id_candle,
            };

            db.Candles_Order.Add(candles_Order);
            db.SaveChanges();

            Orders order = new Orders
            {
                Id_User = id_user,
                Id_Candles_Order = candles_Order.Id,
                Price = (decimal)price,
                Id_Status = 1,
                Order_Address = address,
                Date = DateTime.Now,
            };

            db.Orders.Add(order);
            db.SaveChanges();
            return order;
        }
        public static void RemoveAvatarUser()
        {
            Users user = db.Users.Where(x => x.Id == UserCache._id).SingleOrDefault();
            UserCache._avatar = null;
            user.Avatar = null;
            db.SaveChanges();
        }
        public static void SetAvatarUser(byte[] image)
        {
            Users user = db.Users.Where(x => x.Id == UserCache._id).SingleOrDefault();
            UserCache._avatar = Utils.Utils.BinaryToImage(image);
            user.Avatar = image;
            db.SaveChanges();
        }
        public static Order_Address AddAddresses(string name)
        {
            Order_Address address = new Order_Address
            {
                Address = name,
            };

            Order_Address result = db.Order_Address.Add(address);
            db.SaveChanges();
            return result;
        }
        public async static Task<List<Order_Address>> GetAddresses()
        {
            return await db.Order_Address.Select(x => x).ToListAsync();
        }
        public static void RemoveAddress(int id_address)
        {
            List<Orders> orders = db.Orders.Where(x => x.Id_Address == id_address).ToList();
            orders.ForEach(o =>
            {
                o.Id_Status = 5;
                o.Id_Address = null;
            });

            Order_Address order_address = db.Order_Address.Where(x => x.Id == id_address).SingleOrDefault();
            db.Order_Address.Remove(order_address);
            db.SaveChanges();
        }
    }
}
