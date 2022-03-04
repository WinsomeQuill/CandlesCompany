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
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }
        private void ButtonSignUpAlreadyRegistered_Click(object sender, RoutedEventArgs e)
        {
            new SignInWindow().Show();
            Close();
        }
        private void ButtonSignUpRegister_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    string email = TextBoxSignUpEmail.Text;
                    string pass = PasswordBoxSignUp.Password;
                    string first_name = TextBoxSignUpFirstName.Text;
                    string last_name = TextBoxSignUpLastName.Text;
                    string middle_name = TextBoxSignUpMiddleName.Text;

                    if (await DBManager.ExistUser(email))
                    {
                        MessageBox.Show("Данный Email уже зарегестрирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (middle_name.Length == 0)
                    {
                        middle_name = null;
                    }

                    DBManager.Registration(email, pass, first_name, last_name, middle_name);
                    MessageBox.Show("Вы зарегистрировались! Спасибо!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }
    }
}
