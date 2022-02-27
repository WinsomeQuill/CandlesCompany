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

namespace CandlesCompany.UI.Employee
{
    /// <summary>
    /// Логика взаимодействия для EmployeeChangeWindow.xaml
    /// </summary>
    public partial class EmployeeChangeWindow : Window
    {
        public EmployeeChangeWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            /*DBManager.GetRoles().ForEach(role =>
            {
                ComboBoxEmployeeChangeRole.Items.Add(role);
            });

            List<Users> employees = DBManager.GetEmployees();

            foreach (Users user in employees)
            {
                int index = ComboBoxEmployeeChangeEmail.Items.Add(new ComboBoxItem
                {
                    Content = $"{user.Last_Name} {user.First_Name} | {user.Email}",
                    Tag = user
                });
            }

            ComboBoxEmployeeChangeRole.SelectedIndex = ComboBoxEmployeeChangeEmail.SelectedIndex = 0;
            if (ComboBoxEmployeeChangeRole.Items.Count <= 0 || ComboBoxEmployeeChangeEmail.Items.Count <= 0)
            {
                ButtonEmployeeChange.IsEnabled = false;
            }*/
        }

        private void ButtonEmployeeChange_Click(object sender, RoutedEventArgs e)
        {
            Users user = (Users)(ComboBoxEmployeeChangeEmail.SelectedItem as ComboBoxItem).Tag;
            string role = ComboBoxEmployeeChangeRole.SelectedItem.ToString();

            if(user.Roles.Name == role)
            {
                MessageBox.Show($"Сотрудник \"{user.Last_Name} {user.First_Name}\" уже имеет данную должность!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DBManager.ChangeRoleById(user.Id, role);
            MessageBox.Show($"Вы изменили сотруднику \"{user.Last_Name} {user.First_Name}\" должность на \"{role}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            ComboBoxEmployeeChangeEmail.Items.Clear();
            ComboBoxEmployeeChangeRole.Items.Clear();
            Init();
        }
    }
}
