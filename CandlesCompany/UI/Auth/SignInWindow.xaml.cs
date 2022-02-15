using CandlesCompany.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CandlesCompany.UI.Auth
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
        }
        private void ButtonSignInRegistered_Click(object sender, RoutedEventArgs e)
        {
            new SignUpWindow().Show();
            Close();
        }
        private void ButtonSignInJoin_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBoxSignInEmail.Text;
            string pass = PasswordBoxSignIn.Password;

            if (!DBManager.Join(email, pass))
            {
                Task.Run(async () =>
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        ButtonSignInJoin.IsEnabled = false;
                        MessageBoxResult result = MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (result == MessageBoxResult.OK)
                        {
                            ButtonSignInJoin.IsEnabled = true;
                        }
                    });
                });
                return;
            }

            Users user = DBManager.UserInfo(email);

            UserCache._id = user.Id;
            UserCache._first_name = user.First_Name;
            UserCache._last_name = user.Last_Name;
            UserCache._middle_name = user.Middle_Name;
            UserCache._phone = user.Phone;
            UserCache._email = user.Email;
            UserCache._role = user.Roles;

            new MainWindow().Show();
            Close();
        }
    }
}
