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

namespace CandlesCompany
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
            Dictionary<string, string> cache = null;
            string email = TextBoxSignInEmail.Text;
            string pass = PasswordBoxSignIn.Password;

            Thread thr = new Thread(delegate ()
            {
                if (!DBManager.Join(email, pass))
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                object item = DBManager.UserInfo(email);
                cache = Utils.Utils.ToDictionary<string>(item);   
            });

            thr.Start();
            thr.Join();

            if(cache == null) { return; }

            
            UserCache._id = Convert.ToInt32(cache["Id"]);
            UserCache._first_name = cache["First_Name"];
            UserCache._last_name = cache["Last_Name"];
            UserCache._middle_name = cache["Middle_Name"];
            UserCache._phone = cache["Phone"];
            UserCache._email = cache["Email"];
            UserCache._priority = Convert.ToInt32(cache["Priority"]);
            UserCache._role_name = cache["Name"];

            new MainWindow().Show();
            Close();
        }
    }
}
