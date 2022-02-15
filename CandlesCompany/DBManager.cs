﻿using CandlesCompany.Cache;
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
            return db.Users.Where(x => x.Email == email && x.Password == pass).Select(x => x.Id).FirstOrDefault() != 0;
        }
        public static bool ExistUser(string email)
        {
            return db.Users.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefault() != 0;
        }
        public static void Registration(string email, string pass, string first_name, string last_name, string middle_name = null)
        {
            Users user = new Users
            {
                Email = email,
                Password = pass,
                First_Name = first_name,
                Last_Name = last_name,
                Middle_Name = middle_name,
                Id_Role = 5
            };

            db.Users.Add(user);
            db.SaveChanges();
        }
        public static Users UserInfo(string email)
        {
            return db.Users.Where(x => x.Email == email).Select(x => x).FirstOrDefault();
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
        public static List<Users> GetEmployees()
        {
            return db.Users.Where(x => x.Id_Role != 1 && x.Id_Role != 6).Select(x => x).ToList();
        }
        public static List<Users> GetUsers()
        {
            return db.Users.Where(x => x.Id_Role == 6).Select(x => x).ToList();
        }
        public static byte[] GetImageItem(int id)
        {
            byte[] result = db.Candles.Where(x => x.Id == id).Select(x => x.Image).FirstOrDefault();
            if(result.Length == 0 || result == null) { return null; }
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
    }
}
