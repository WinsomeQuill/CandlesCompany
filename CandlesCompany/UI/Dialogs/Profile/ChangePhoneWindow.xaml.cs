using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CandlesCompany.UI.Dialogs.Profile
{
    /// <summary>
    /// Логика взаимодействия для ChangePhoneWindow.xaml
    /// </summary>
    public partial class ChangePhoneWindow : Window
    {
        public ChangePhoneWindow()
        {
            InitializeComponent();
            TextBoxDialogPhone.Text = null;
        }

        private async void ButtonDialogConfirm_Click(object sender, RoutedEventArgs e)
        {
            Loading();
            string phone = TextBoxDialogPhone.Text;
            if (!new Regex(@"^[0-9]+$").IsMatch(phone))
            {
                MessageBox.Show("Неверный формат номера!", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Loading(false);
                return;
            }


            Utils.Utils._mainWindow.TextBlockProfilePhone.Text = $"Телефон: +{phone}";
            Cache.UserCache._phone = phone;
            await DBManager.ChangePhone(Cache.UserCache._id, phone);

            Loading(false);
            Close();
        }

        private void Loading(bool enabled = true)
        {
            if (enabled)
            {
                TextBoxDialogPhone.IsEnabled = ButtonDialogCancel.IsEnabled =
                    ButtonDialogConfirm.IsEnabled = false;
                ProgressBarLoading.Visibility = Visibility.Visible;
                return;
            }

            ProgressBarLoading.Visibility = Visibility.Collapsed;
            TextBoxDialogPhone.IsEnabled = ButtonDialogCancel.IsEnabled =
                ButtonDialogConfirm.IsEnabled = true;
        }

        private void ButtonDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
