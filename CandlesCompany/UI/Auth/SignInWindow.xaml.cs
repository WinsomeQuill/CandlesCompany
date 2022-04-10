using CandlesCompany.Cache;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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
            TextBoxSignInEmail.Text = null;
            TextBoxSignInEmail.Text = "alexmaroni392@gmail.com";
            PasswordBoxSignIn.Password = "admin";
        }
        private void ButtonSignInRegistered_Click(object sender, RoutedEventArgs e)
        {
            new SignUpWindow().Show();
            Close();
        }
        private async void ButtonSignInJoin_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBoxSignInEmail.Text;
            string pass = PasswordBoxSignIn.Password;

            if (email.Length == 0 || pass.Length == 0)
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ButtonSignInJoin.IsEnabled = PasswordBoxSignIn.IsEnabled = 
                TextBoxSignInEmail.IsEnabled = ButtonSignInRegistered.IsEnabled = false;
            ProgressBarSignInLoading.Visibility = Visibility.Visible;
            JObject result = await Api.Login(email, pass);
            if (result["Error"].ToString() != "None")
            {
                ButtonSignInJoin.IsEnabled = PasswordBoxSignIn.IsEnabled = 
                    TextBoxSignInEmail.IsEnabled = ButtonSignInRegistered.IsEnabled = true;
                ProgressBarSignInLoading.Visibility = Visibility.Collapsed;
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProgressBarSignInLoading.Visibility = Visibility.Collapsed;
            Utils.Utils._defaultAvatar = new BitmapImage(new Uri(@"pack://application:,,,/CandlesCompany;component/Resources/Images/Users/default_avatar.png"));
            JObject roles = await Api.GetRoles();
            roles["Result"].ToList().ForEach(x =>
            {
                Utils.Utils._roles.Add(x.ToString());
            });

            JObject role = await Api.GetRole(email);
            UserCache._id = (int)result["Result"]["Id"];
            UserCache._first_name = result["Result"]["First_Name"].ToString();
            UserCache._last_name = result["Result"]["Last_Name"].ToString();
            UserCache._middle_name = result["Result"]["Middle_Name"].ToString();
            UserCache._phone = string.IsNullOrEmpty(result["Result"]["Phone"].ToString()) ? 
                "Телефон: Не указан" : "Телефон: " + result["Result"]["Phone"].ToString();
            UserCache._email = result["Result"]["Email"].ToString();
            UserCache._role = (int)role["Result"]["Id"];
            UserCache._roleName = role["Result"]["Name"].ToString();
            UserCache._avatar = string.IsNullOrEmpty(result["Result"]["Avatar"].ToString()) ? 
                Utils.Utils._defaultAvatar : Utils.Utils.BinaryToImage((byte[])result["Result"]["Avatar"]);

            MainWindow mainWindow = new MainWindow();
            await mainWindow.Init();
            mainWindow.Show();
            Close();
        }
    }
}
