using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace CandlesCompany.UI.Employee
{
    /// <summary>
    /// Логика взаимодействия для EmployeeAddWindow.xaml
    /// </summary>
    public partial class EmployeeAddWindow : Window
    {
        public EmployeeAddWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    DBManager.GetRoles().ForEach(role =>
                    {
                        ComboBoxEmployeeAddRole.Items.Add(role);
                    });

                    List<Users> users = await DBManager.GetUsersForPage(1);

                    users.ForEach(user =>
                    {
                        int index = ComboBoxEmployeeAdd.Items.Add(new ComboBoxItem
                        {
                            Content = $"{user.Last_Name} {user.First_Name} | {user.Email}",
                            Tag = user
                        });
                    });

                    ComboBoxEmployeeAddRole.SelectedIndex = ComboBoxEmployeeAdd.SelectedIndex = 0;
                    if (ComboBoxEmployeeAdd.Items.Count <= 0 || ComboBoxEmployeeAddRole.Items.Count <= 0)
                    {
                        ButtonEmployeeAdd.IsEnabled = false;
                    }
                });
            }).Start();
        }

        private void ButtonEmployeeAdd_Click(object sender, RoutedEventArgs e)
        {
            Users user = (Users)(ComboBoxEmployeeAdd.SelectedItem as ComboBoxItem).Tag;
            string role = ComboBoxEmployeeAddRole.SelectedItem.ToString();

            DBManager.ChangeRoleById(user.Id, role);
            MessageBox.Show($"Вы назначили пользователя \"{user.Last_Name} {user.First_Name}\" на должность \"{role}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            ComboBoxEmployeeAddRole.Items.Clear();
            ComboBoxEmployeeAdd.Items.Clear();
            Init();
        }
    }
}
