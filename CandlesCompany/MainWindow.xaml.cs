using CandlesCompany.Cache;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CatalogInit();

            TextBlockProfileName.Text = $"ФИО: {UserCache._last_name} {UserCache._first_name} {UserCache._middle_name}";
            TextBlockProfilePhone.Text = $"Телефон: {UserCache._phone}";
            TextBlockProfileEmail.Text = $"Эл. почта: {UserCache._email}";
            TextBlockProfileRole.Text = $"Должность: {UserCache._role_name}";
        }

        private void CatalogInit()
        {
            Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    DBManager.db.Candles.ToList().ForEach(candle =>
                    {
                        ListViewCatalog.Items.Add(new ListItem(candle.Name, candle.Description, candle));
                    });
                });
            });
        }

        private void ButtonManagementAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            new EmployeeAddWindow().Show();
        }

        private void ButtonManagementAddItem_Click(object sender, RoutedEventArgs e)
        {
            new ItemAddWindow().Show();
        }

        private void ButtonManagementRemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            new EmployeeRemoveWindow().Show();
        }

        private void ButtonManagementChangeItem_Click(object sender, RoutedEventArgs e)
        {
            new ItemChangeWindow().Show();
        }

        private void ButtonManagementRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            new ItemRemoveWindow().Show();
        }

        private void ButtonManagementChangeEmployee_Click(object sender, RoutedEventArgs e)
        {
            new EmployeeChangeWindow().Show();
        }
    }
}
