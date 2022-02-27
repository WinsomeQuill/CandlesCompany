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
        public Users User { get; set; }
        public List<string> Roles { get; set; }
        public UsersList(Users user, BitmapImage avatar, List<string> roles)
        {
            User = user;
            UserID = user.Id;
            UserName = $"{user.Last_Name} {user.First_Name} {user.Middle_Name}";
            UserEmail = user.Email;
            UserAvatar = avatar;
            Roles = roles;
        }
        public UsersList(Users user, BitmapImage avatar)
        {
            User = user;
            UserID = user.Id;
            UserName = $"{user.Last_Name} {user.First_Name} {user.Middle_Name}";
            UserEmail = user.Email;
            UserAvatar = avatar;
        }
    }
}
