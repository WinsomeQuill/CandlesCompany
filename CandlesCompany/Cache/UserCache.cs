using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CandlesCompany.Cache
{
    public static class UserCache
    {
        //public static List<Candles> Basket = new List<Candles>();
        public static Dictionary<Candles, int> Basket { get; set; } = new Dictionary<Candles, int>(); 
        public static int _id { get; set; }
        public static string _last_name { get; set; }
        public static string _first_name { get; set; }
        public static string _middle_name { get; set; }
        public static string _phone { get; set; }
        public static string _email { get; set; }
        public static Roles _role { get; set; }
        public static BitmapImage _avatar { get; set; }
    }
}
