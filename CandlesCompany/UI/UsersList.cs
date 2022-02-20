using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public UsersList(int id, string name, string email, BitmapImage avatar)
        {
            UserID = id;
            UserName = name;
            UserEmail = email;
            UserAvatar = avatar;
        }
    }
}
