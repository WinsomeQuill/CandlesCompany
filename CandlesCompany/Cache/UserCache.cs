using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandlesCompany.Cache
{
    public static class UserCache
    {
        public static int _id { get; set; }
        public static string _last_name { get; set; }
        public static string _first_name { get; set; }
        public static string _middle_name { get; set; }
        public static string _phone { get; set; }
        public static string _email { get; set; }
        public static int _priority { get; set; }
        public static string _role_name { get; set; }
    }
}
