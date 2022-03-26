﻿using CandlesCompany.Cache;
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
        }
        private void ButtonSignInRegistered_Click(object sender, RoutedEventArgs e)
        {
            new SignUpWindow().Show();
            Close();
        }
        private async void ButtonSignInJoin_Click(object sender, RoutedEventArgs e)
        {
            try
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
                if (!await DBManager.Join(email, pass))
                {
                    ButtonSignInJoin.IsEnabled = PasswordBoxSignIn.IsEnabled = 
                        TextBoxSignInEmail.IsEnabled = ButtonSignInRegistered.IsEnabled = true;
                    ProgressBarSignInLoading.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ProgressBarSignInLoading.Visibility = Visibility.Collapsed;
                Utils.Utils._defaultAvatar = new BitmapImage(new Uri(@"pack://application:,,,/CandlesCompany;component/Resources/Images/Users/default_avatar.png"));
                Utils.Utils._roles = await DBManager.GetRoles();
                Roles role = await DBManager.GetRole(email);
                Users user = await DBManager.UserInfo(email);

                UserCache._id = user.Id;
                UserCache._first_name = user.First_Name;
                UserCache._last_name = user.Last_Name;
                UserCache._middle_name = user.Middle_Name;
                UserCache._phone = user.Phone;
                UserCache._email = user.Email;
                UserCache._role = role;
                UserCache._avatar = user.Avatar == null ? Utils.Utils._defaultAvatar : Utils.Utils.BinaryToImage(user.Avatar);

                MainWindow mainWindow = new MainWindow();
                await mainWindow.Init();
                mainWindow.Show();
                Close();
            }
            //catch (EntityException)
            catch (FormatException)
            {
                MessageBox.Show("Произошла ошибка! Удаленный сервер не смог принять запрос!", 
                    "Server TimeOut", MessageBoxButton.OK, MessageBoxImage.Error);
                ProgressBarSignInLoading.Visibility = Visibility.Collapsed;
                ButtonSignInJoin.IsEnabled = PasswordBoxSignIn.IsEnabled = 
                    TextBoxSignInEmail.IsEnabled = ButtonSignInRegistered.IsEnabled = true;
                return;
            }
        }
    }
}
