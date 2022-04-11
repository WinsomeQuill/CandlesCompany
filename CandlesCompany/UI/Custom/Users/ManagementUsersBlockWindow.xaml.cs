using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CandlesCompany.UI.Custom.Users
{
    /// <summary>
    /// Логика взаимодействия для ManagementUsersBlockWindow.xaml
    /// </summary>
    public partial class ManagementUsersBlockWindow : Window
    {
        private string _formatName { get; set; }
        private int _userId { get; set; }
        public ManagementUsersBlockWindow(string formatName, int userId)
        {
            InitializeComponent();
            _formatName = formatName;
            _userId = userId;

            TextBlockManagementUsersBlockName.Text = _formatName;
        }

        private void ButtonManagementUsersBlockCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ButtonManagementUsersBlockConfirm_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите заблокировать пользователя?", _formatName,
                       MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            //await DBManager.AddBan(_userId, Cache.UserCache._id, TextBoxManagementUsersBlockReason.Text);
            MessageBox.Show("Вы заблокировали пользователя!", _formatName, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
