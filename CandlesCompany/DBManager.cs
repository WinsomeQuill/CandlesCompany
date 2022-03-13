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
        public async static Task<bool> ExistUser(string email)
        {
            return await db.Users.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefaultAsync() != 0;
        }
        public async static Task Registration(string email, string pass, string first_name, string last_name, string middle_name = null)
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
            await db.SaveChangesAsync();
        }
        public async static Task<Users> UserInfo(string email)
        {
            return await db.Users.Where(x => x.Email == email).Select(x => x).FirstOrDefaultAsync();
        }
        public async static Task<Users> UserInfo(int id)
        {
            return await db.Users.Where(x => x.Id == id).Select(x => x).FirstOrDefaultAsync();
        }
        public async static Task<List<string>> GetRoles()
        {
            return await db.Roles.Where(x => x.Id != 1 && x.Id != 6).Select(x => x.Name).ToListAsync();
        }
        public async static Task ChangeRoleById(int id, string role)
        {
            Users user = await db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user.Roles.Name == role) { return; }
            user.Id_Role = await db.Roles.Where(x => x.Name == role).Select(x => x.Id).FirstOrDefaultAsync();
            await db.SaveChangesAsync();
        }
        public async static Task ChangeRoleById(string email, string role)
        {
            Users user = await db.Users.SingleOrDefaultAsync(x => x.Email == email);
            user.Id_Role = await db.Roles.Where(x => x.Name == role).Select(x => x.Id).FirstOrDefaultAsync();
            await db.SaveChangesAsync();
        }
        public async static Task<int> GetUsersCount()
        {
            return await db.Users.CountAsync();
        }
        public async static Task<List<Users>> GetUsersForPage(int page, int count = 50)
        {
            return await db.Users.Where(x => x.Id_Role == 6).Select(x => x).OrderBy(x => x.Id).Skip(count * (page - 1)).Take(count).ToListAsync();
        }
        public async static Task<List<Users>> FindUsers(string value)
        {
            List<Users> users = new List<Users>();
            users.AddRange(await db.Users.Where(x => x.First_Name.Contains(value) || x.First_Name.Contains(value) || x.Middle_Name.Contains(value)).Select(x => x).Distinct().Take(500).ToListAsync());
            return users;
        }
        public async static Task<List<Users>> GetUsersByRoleDelivery()
        {
            return await db.Users.Where(x => x.Id_Role == 3).Select(x => x).ToListAsync();
        }
        public async static Task<List<Users>> GetUsersByRoleManager()
        {
            return await db.Users.Where(x => x.Id_Role == 4).Select(x => x).ToListAsync();
        }
        public async static Task<List<Users>> GetUsersByRoleAdministrator()
        {
            return await db.Users.Where(x => x.Id_Role == 5).Select(x => x).ToListAsync();
        }
        public async static Task<List<Users>> GetEmployees(int page, int start_id_role = 1, int end_id_role = 6, int count = 50)
        {
            return await db.Users.Where(x => x.Id_Role > start_id_role && x.Id_Role < end_id_role).OrderBy(x => x.Id).Skip(count * (page - 1)).Take(count).ToListAsync();
        }
        public async static Task<List<Users>> FindEmployees(string value)
        {
            List<Users> users = new List<Users>();
            users.AddRange(await db.Users.Where(x => (x.Roles.Id > 1 && x.Roles.Id < 6) && (x.First_Name.Contains(value) || x.First_Name.Contains(value) || x.Middle_Name.Contains(value)))
                .Select(x => x).Distinct().Take(500).ToListAsync());
            return users;
        }
        public async static Task<int> GetEmployeesCount(int start_id_role = 1, int end_id_role = 6)
        {
            return await db.Users.Where(x => x.Id_Role > start_id_role && x.Id_Role < end_id_role).CountAsync();
        }
        public async static Task<byte[]> GetImageItem(int id)
        {
            return await db.Candles.Where(x => x.Id == id).Select(x => x.Image).FirstOrDefaultAsync();
        }
        public async static Task<List<Candles>> GetCandles()
        {
            return await db.Candles.Select(x => x).ToListAsync();
        }
        public async static Task<List<Type_Candle>> GetTypeCandles()
        {
            return await db.Type_Candle.Select(x => x).ToListAsync();
        }
        public async static Task UpdateItem(int id, int id_type, string name, string description, int count, double price, byte[] image)
        {
            Candles candle = await db.Candles.SingleOrDefaultAsync(x => x.Id == id);
            candle.Image = image;
            candle.Price = (decimal)price;
            candle.Name = name;
            candle.Description = description;
            candle.Count = count;
            candle.Id_Type_Candle = id_type;
            await db.SaveChangesAsync();
        }
        public async static Task AddItem(int id_type, string name, string description, int count, double price, byte[] image)
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
            await db.SaveChangesAsync();
        }
        public async static Task RemoveItem(int id)
        {
            Candles candle = await db.Candles.Where(x => x.Id == id).SingleOrDefaultAsync();
            db.Candles.Remove(candle);
            await db.SaveChangesAsync();
        }
        public async static Task<Dictionary<Candles, int>> GetCandlesBasket(int id_user)
        {
            return await db.Users_Baskets.Where(x => x.Id_User == id_user && x.Candles.Id == x.Id_Candles).ToDictionaryAsync(x => x.Candles, x => x.Count);
        }
        public async static Task AddCandlesBasket(int id_user, Candles candle)
        {
            db.Users_Baskets.Add(new Users_Baskets { Id_Candles = candle.Id, Id_User = id_user, Count = 1 });
            await db.SaveChangesAsync();
            UserCache.Basket.Add(candle, 1);
        }
        public async static Task UpdateCandlesBasket(int id_user, Candles candle, int count)
        {
            Users_Baskets basket = await db.Users_Baskets.Where(x => x.Id_User == id_user && x.Id_Candles == candle.Id).SingleOrDefaultAsync();
            basket.Count = count;
            await db.SaveChangesAsync();
            UserCache.Basket.Remove(candle);
            UserCache.Basket.Add(candle, count);
        }
        public async static Task RemoveCandlesBasket(int id_user, Candles candle)
        {
            Users_Baskets basket = await db.Users_Baskets.Where(x => x.Id_User == id_user && x.Id_Candles == candle.Id).SingleOrDefaultAsync();
            db.Users_Baskets.Remove(basket);
            UserCache.Basket.Remove(candle);
            await db.SaveChangesAsync();
        }
        public async static Task<List<Orders>> GetOrders(int id_user)
        {
            return await db.Orders.Where(x => x.Id_User == id_user).ToListAsync();
        }
        public async static Task<List<Orders>> GetOrdersForPage(int page, int count = 50)
        {
            return await db.Orders.Select(x => x).OrderBy(x => x.Id).Skip(count * (page - 1)).Take(count).ToListAsync();
        }
        public async static Task<List<Orders>> FindOrders(string name_candle)
        {
            return await db.Orders.Where(x => x.Candles_Order.Candles.Name.Contains(name_candle)).Select(x => x).Distinct().Take(250).ToListAsync();
        }
        public async static Task ChangeOrderStatus(int id_order, string status)
        {
            Orders order = await db.Orders.Where(x => x.Id == id_order).Select(x => x).SingleOrDefaultAsync();
            if (order.Order_Status.Name == status)
            {
                return;
            }
            order.Id_Status = await db.Order_Status.Where(x => x.Name == status).Select(x => x.Id).SingleOrDefaultAsync();
            await db.SaveChangesAsync();
        }
        public async static Task<int> GetOrdersCount()
        {
            return await db.Orders.CountAsync();
        }
        public async static Task<List<string>> GetStatusList()
        {
            return await db.Order_Status.Select(x => x.Name).ToListAsync();
        }
        public async static Task<Orders> AddOrder(int id_user, int id_candle, int count, double price, Order_Address address)
        {
            Candles_Order candles_Order = new Candles_Order
            {
                Count = count,
                Id_Candles = id_candle,
            };

            db.Candles_Order.Add(candles_Order);
            await db.SaveChangesAsync();

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
            await db.SaveChangesAsync();
            return order;
        }
        public async static Task RemoveAvatarUser()
        {
            Users user = await db.Users.Where(x => x.Id == UserCache._id).SingleOrDefaultAsync();
            UserCache._avatar = null;
            user.Avatar = null;
            await db.SaveChangesAsync();
        }
        public async static Task SetAvatarUser(byte[] image)
        {
            Users user = await db.Users.Where(x => x.Id == UserCache._id).SingleOrDefaultAsync();
            UserCache._avatar = Utils.Utils.BinaryToImage(image);
            user.Avatar = image;
            await db.SaveChangesAsync();
        }
        public async static Task<Order_Address> AddAddresses(string name)
        {
            Order_Address address = new Order_Address
            {
                Address = name,
            };

            Order_Address result = db.Order_Address.Add(address);
            await db.SaveChangesAsync();
            return result;
        }
        public async static Task<List<Order_Address>> GetAddresses()
        {
            return await db.Order_Address.Select(x => x).ToListAsync();
        }
        public async static Task RemoveAddress(int id_address)
        {
            List<Orders> orders = await db.Orders.Where(x => x.Id_Address == id_address).ToListAsync();
            orders.ForEach(o =>
            {
                o.Id_Status = 5;
                o.Id_Address = null;
            });

            Order_Address order_address = await db.Order_Address.Where(x => x.Id == id_address).SingleOrDefaultAsync();
            db.Order_Address.Remove(order_address);
            await db.SaveChangesAsync();
        }
        public async static Task AddBan(int id_user, int id_admin, string reason)
        {
            Users_Block users_Block = new Users_Block
            {
                DateStart = DateTime.Now,
                Id_Admin = id_admin,
                Id_Users = id_user,
                Reason = reason,
                Users1 = await db.Users.Where(x => x.Id == id_admin).Select(x => x).SingleAsync(),
                Users = await db.Users.Where(x => x.Id == id_user).Select(x => x).SingleAsync()
            };

            db.Users_Block.Add(users_Block);
            await db.SaveChangesAsync();
        }
        public async static Task<bool> IsBanned(int id_user)
        {
            Users_Block result = await db.Users_Block.Where(x => x.Id_Users == id_user).Select(x => x).SingleOrDefaultAsync();
            return result !=  null;
        }
        public async static Task RemoveBan(int id_user)
        {
            db.Users_Block.Remove(await db.Users_Block.Where(x => x.Id_Users == id_user).Select(x => x).SingleOrDefaultAsync());
            await db.SaveChangesAsync();
        }
    }
}
