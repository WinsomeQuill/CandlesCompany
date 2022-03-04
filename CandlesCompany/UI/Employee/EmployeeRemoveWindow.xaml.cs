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
using System.Windows.Shapes;

namespace CandlesCompany.UI.Employee
{
    /// <summary>
    /// Логика взаимодействия для EmployeeRemoveWindow.xaml
    /// </summary>
    public partial class EmployeeRemoveWindow : Window
    {
        public EmployeeRemoveWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            /*new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    if (ComboBoxEmployeeRemove.Items.Count != 0)
                    {
                        ComboBoxEmployeeRemove.Items.Clear();
                    }

                    DBManager.GetEmployees().ForEach(e =>
                    {
                        ComboBoxEmployeeRemove.Items.Add(new ComboBoxItem
                        {
                            Content = $"{e.Last_Name} {e.First_Name} | {e.Email}",
                            Tag = e
                        });
                    });

                    ComboBoxEmployeeRemove.SelectedIndex = 0;
                    if (ComboBoxEmployeeRemove.Items.Count <= 0)
                    {
                        ButtonEmployeeRemove.IsEnabled = false;
                    }
                });
            }).Start();*/
        }

        private async void ButtonEmployeeRemove_Click(object sender, RoutedEventArgs e)
        {
            Users user = (Users)(ComboBoxEmployeeRemove.SelectedItem as ComboBoxItem).Tag;
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите снять с должности \"{user.Roles.Name}\" сотрудника \"{user.Last_Name} {user.First_Name}\"", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No) { return; }

            MessageBox.Show($"Вы сняли с должности \"{user.Roles.Name}\" сотрудника \"{user.Last_Name} {user.First_Name}\"!", "Успешно", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            await DBManager.ChangeRoleById(user.Id, "Пользователь");
            ComboBoxEmployeeRemove.Items.Clear();
            Init();
        }
    }
}
