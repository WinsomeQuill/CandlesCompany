using Newtonsoft.Json.Linq;
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
        public static Dictionary<JToken, int> Basket { get; set; } = new Dictionary<JToken, int>(); 
        public static int _id { get; set; }
        public static string _last_name { get; set; }
        public static string _first_name { get; set; }
        public static string _middle_name { get; set; }
        public static string _phone { get; set; }
        public static string _email { get; set; }
        public static int _role { get; set; }
        public static string _roleName { get; set; }
        public static BitmapImage _avatar { get; set; }
    }
}
