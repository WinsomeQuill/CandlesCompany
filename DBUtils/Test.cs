using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBUtils
{
    public class Test
    {
        private candlesEntities db { get; set; }
        private object locker { get; set; }
        private int total { get; set; }
        public Test(candlesEntities db, int total, object locker)
        { 
            this.db = db;
            this.total = total;
            this.locker = locker;
        }
        public Thread Start()
        {
            Console.WriteLine("Thread run!");
            Thread thr = new Thread(() =>
            {
                lock (this.locker)
                {
                    for (int k = 0; k < this.total; k++)
                    {
                        Users users = new Users
                        {
                            Avatar = null,
                            Email = $"{k}_test@gmail.com",
                            First_Name = $"Test_{k}",
                            Last_Name = $"Test_{k}",
                            Middle_Name = null,
                            Id_Role = 6,
                            Password = $"Test_{k}",
                            Phone = null,
                        };

                        db.Users.Add(users);
                    }

                    db.SaveChanges();
                    Console.WriteLine($"Save");
                }
            });
            return thr;
        }
    }
}
