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

namespace CandlesCompany
{
    /// <summary>
    /// Логика взаимодействия для EmployeeAddWindow.xaml
    /// </summary>
    public partial class EmployeeAddWindow : Window
    {
        public EmployeeAddWindow()
        {
            InitializeComponent();
            DBManager.GetRoles().ForEach(role =>
            {
                ComboBoxEmployeeAddRole.Items.Add(role);
            });
        }

        private void ButtonEmployeeAdd_Click(object sender, RoutedEventArgs e)
        {
            string email_or_id = TextBoxEmployeeAddEmail.Text;
            Regex regex = new Regex(@"^[0-9]+$"); // is ID?
            if(regex.IsMatch(email_or_id))
            {
                int id = Convert.ToInt32(email_or_id);

            }
        }
    }
}
