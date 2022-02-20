using CandlesCompany.Cache;
using CandlesCompany.UI.Custom;
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
using CandlesCompany.UI.Custom.Basket;
using CandlesCompany.UI.Custom.Catalog;

namespace CandlesCompany.UI
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
            BasketInit();
            OrdersInit();

            Utils.Utils._mainWindow = this;

            TextBlockProfileName.Text = $"ФИО: {UserCache._last_name} {UserCache._first_name} {UserCache._middle_name}";
            TextBlockProfilePhone.Text = $"Телефон: {UserCache._phone}";
            TextBlockProfileEmail.Text = $"Эл. почта: {UserCache._email}";
            TextBlockProfileRole.Text = $"Должность: {UserCache._role.Name}";

            Utils.Utils._selectediteminfo = new SelectedItemInfo(this);
            GridCatalogItems.Children.Add(Utils.Utils._selectediteminfo);
            Utils.Utils._selectediteminfo.SetValue(Grid.ColumnProperty, 1);
            Utils.Utils._selectediteminfo.SetValue(Grid.RowProperty, 0);
            Utils.Utils._selectediteminfo.Visibility = Visibility.Collapsed;

            Utils.Utils._summaryInformation = new SummaryInformation();
            GridBasketItems.Children.Add(Utils.Utils._summaryInformation);
            Utils.Utils._summaryInformation.SetValue(Grid.ColumnProperty, 1);
            Utils.Utils._summaryInformation.SetValue(Grid.RowProperty, 0);

            Utils.Utils._listViewBasket = ListViewBasket;
            Utils.Utils._dataGridOrdersList = DataGridOrdersList;
        }
        private void OrdersInit()
        {
            new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    DBManager.GetOrders(UserCache._id).ForEach(o =>
                    {
                        DataGridOrdersList.Items.Add(new Custom.Orders.OrderList(
                            o.Id,
                            o.Date,
                            o.Candles_Order.Candles.Name,
                            (double)o.Price,
                            o.Candles_Order.Count,
                            o.Order_Status.Id));
                    });
                });
            }).Start();
        }
        private void BasketInit()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(delegate ()
                {
                    foreach(var item in DBManager.GetCandlesBasket(UserCache._id))
                    {
                        Candles candle = item.Key;
                        UserCache.Basket.Add(candle, item.Value);
                        ListViewBasket.Items.Add(new BasketItem(candle, item.Value));
                        Utils.Utils._summaryInformation.AddCount(item.Value);
                        Utils.Utils._summaryInformation.AddPrice((double)candle.Price * item.Value);
                    }
                });
            }).Start();
        }
        private void CatalogInit()
        {
            new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    DBManager.db.Candles.ToList().ForEach(candle =>
                    {
                        ListViewCatalog.Items.Add(new Custom.Catalog.ListItem(candle.Name, candle.Description, candle));
                    });
                });
            }).Start();
        }
        private void ButtonManagementAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            new Employee.EmployeeAddWindow().Show();
        }
        private void ButtonManagementChangeEmployee_Click(object sender, RoutedEventArgs e)
        {
            new Employee.EmployeeChangeWindow().Show();
        }
        private void ButtonManagementRemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            new Employee.EmployeeRemoveWindow().Show();
        }
        private void ButtonManagementAddItem_Click(object sender, RoutedEventArgs e)
        {
            new Item.ItemAddWindow().Show();
        }
        private void ButtonManagementChangeItem_Click(object sender, RoutedEventArgs e)
        {
            new Item.ItemChangeWindow().Show();
        }
        private void ButtonManagementRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            new Item.ItemRemoveWindow().Show();
        }
    }
}
