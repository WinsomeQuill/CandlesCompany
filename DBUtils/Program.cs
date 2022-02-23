using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DBUtils
{
    internal class Program
    {
        private static candlesEntities db { get; } = new candlesEntities();
        private static readonly int threads = 1;
        private static List<Thread> list = new List<Thread>();
        private static object locker { get; set; } = new object();
        private static void Main(string[] args)
        {
            CreateUsers(100000);
            Console.WriteLine($"Threads count: {list.Count()}");

            for (int i = 0; i < list.Count(); i++)
            {
                list[i].IsBackground = true;
                list[i].Start();
            }

            Console.ReadLine();
        }
        private static void CreateUsers(int count)
        {
            for (int i = 2; i < 10; i++)
            {
                if (count % i == 0)
                {
                    int total = count / threads;
                    Console.WriteLine($"Thread per count: {total}");
                    for (int j = 0; j < threads; j++)
                    {
                        try
                        {
                            Test test = new Test(db, total, locker);
                            Thread thr = test.Start();
                            list.Add(thr);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    return;
                }
            }
        }
    }
}
