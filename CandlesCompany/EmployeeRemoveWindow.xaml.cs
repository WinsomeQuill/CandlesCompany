using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CandlesCompany
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
            List<Users> employees = DBManager.GetEmployees();

            foreach (Users user in employees)
            {
                int index = ComboBoxEmployeeRemove.Items.Add(new ComboBoxItem
                {
                    Content = $"{user.Last_Name} {user.First_Name} | {user.Email}",
                    Tag = user
                });
            }

            ComboBoxEmployeeRemove.SelectedIndex = 0;
            if(ComboBoxEmployeeRemove.Items.Count <= 0)
            {
                ButtonEmployeeRemove.IsEnabled = false;
            }
        }

        private void ButtonEmployeeRemove_Click(object sender, RoutedEventArgs e)
        {
            Users user = (Users)(ComboBoxEmployeeRemove.SelectedItem as ComboBoxItem).Tag;
            MessageBox.Show($"Вы сняли с должности \"{user.Roles.Name}\" сотрудника \"{user.Last_Name} {user.First_Name}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            DBManager.ChangeRoleById(user.Id, "Пользователь");
            ComboBoxEmployeeRemove.Items.Clear();
            Init();
        }
    }
}
