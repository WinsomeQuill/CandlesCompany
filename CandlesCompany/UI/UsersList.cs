using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CandlesCompany.UI
{
    public class UsersList
    {
        public int UserID { get; set; }
        public BitmapImage UserImage { get; set; } = Utils.Utils._defaultImage;
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public BitmapImage UserAvatar { get; set; }
        public JToken User { get; set; }
        public List<string> Roles { get; set; }
        public string RoleName { get; set; }
        public UsersList(JToken user, BitmapImage avatar, List<string> roles)
        {
            User = user;
            UserID = (int)user["Id"];
            UserName = $"{user["Last_Name"]} {user["First_Name"]} {user["Middle_Name"]}";
            UserEmail = (string)user["Email"];
            UserAvatar = avatar;
            RoleName = (string)user["Role"]["Name"];
            Roles = roles;
        }
        public UsersList(JToken user, BitmapImage avatar)
        {
            User = user;
            UserID = (int)user["Id"];
            UserName = $"{user["Last_Name"]} {user["First_Name"]} {user["Middle_Name"]}";
            UserEmail = (string)user["Email"];
            UserAvatar = avatar;
            Roles = new List<string>();
            Roles.Add("NULL");
        }
    }
}
