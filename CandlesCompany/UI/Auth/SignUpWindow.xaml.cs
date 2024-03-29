﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
            TextBoxSignUpEmail.Text = TextBoxSignUpFirstName.Text =
                TextBoxSignUpFirstName.Text = TextBoxSignUpLastName.Text =
                TextBoxSignUpMiddleName.Text = null;
        }
        private void ButtonSignUpAlreadyRegistered_Click(object sender, RoutedEventArgs e)
        {
            new SignInWindow().Show();
            Close();
        }
        private async void ButtonSignUpRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBoxSignUpEmail.Text;
            string pass = PasswordBoxSignUp.Password;
            string first_name = TextBoxSignUpFirstName.Text;
            string last_name = TextBoxSignUpLastName.Text;
            string middle_name = TextBoxSignUpMiddleName.Text;


            if (email.Length == 0 || pass.Length == 0 || first_name.Length == 0 || last_name.Length == 0)
            {
                MessageBox.Show("Не все поля заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (pass.Length < 8)
            {
                MessageBox.Show("Короткий пароль! Минимум 8 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                MailAddress m = new MailAddress(email);
            }
            catch(FormatException)
            {
                MessageBox.Show("Неверный формат Email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!new Regex(@"^[A-z0-9]+$").IsMatch(pass))
            {
                MessageBox.Show("В пароле можно использовать только следующие символы: цифры, латинские буквы и \"_\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!new Regex(@"^([A-z]+|[А-я]+)$").IsMatch(first_name))
            {
                MessageBox.Show("Используйте только латиницу или кириллицу в поле \"Имя\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!new Regex(@"^([A-z]+|[А-я]+)$").IsMatch(last_name))
            {
                MessageBox.Show("Используйте только латиницу или кириллицу в поле \"Фамилия\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!new Regex(@"^([A-z]+|[А-я]+)$").IsMatch(middle_name) && middle_name.Length != 0)
            {
                MessageBox.Show("Используйте только латиницу или кириллицу в поле \"Отчество\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (middle_name.Length == 0)
            {
                middle_name = null;
            }

            Loading();
            JObject result = await Api.Registration(email, pass, first_name, last_name, middle_name);
            if (result["Error"].ToString() != "None")
            {
                int errorCode = (int)result["ErrorCode"];
                if (errorCode == 3)
                {
                    MessageBox.Show("Данный Email уже зарегестрирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Loading(false);
                    return;
                }
            }

            MessageBox.Show("Вы зарегистрировались! Спасибо!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            Loading(false);

            new SignInWindow().Show();
            Close();
        }
        private void Loading(bool enabled = true)
        {
            if (enabled)
            {
                ProgressBarSignUpLoading.Visibility = Visibility.Visible;
                ButtonSignUpAlreadyRegistered.IsEnabled = ButtonSignUpRegister.IsEnabled =
                    TextBoxSignUpEmail.IsEnabled = TextBoxSignUpFirstName.IsEnabled = 
                    TextBoxSignUpLastName.IsEnabled = TextBoxSignUpMiddleName.IsEnabled = 
                    PasswordBoxSignUp.IsEnabled = false;
                return;
            }

            ProgressBarSignUpLoading.Visibility = Visibility.Collapsed;
            ButtonSignUpAlreadyRegistered.IsEnabled = ButtonSignUpRegister.IsEnabled =
                TextBoxSignUpEmail.IsEnabled = TextBoxSignUpFirstName.IsEnabled = 
                TextBoxSignUpLastName.IsEnabled = TextBoxSignUpMiddleName.IsEnabled = 
                PasswordBoxSignUp.IsEnabled = true;
        }
    }
}
