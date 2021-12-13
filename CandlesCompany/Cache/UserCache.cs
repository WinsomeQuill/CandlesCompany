using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandlesCompany.Cache
{
    public static class UserCache
    {
        public static int Id { get; set; }
        public static string LastName { get; set; }
        public static string FirstName { get; set; }
        public static string MiddleName { get; set; }
        public static string Phone { get; set; }
        public static string Email { get; set; }
        public static int Priority { get; set; }
    }
}
