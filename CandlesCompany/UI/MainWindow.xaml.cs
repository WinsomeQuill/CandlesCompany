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
using System.Text.RegularExpressions;

namespace CandlesCompany.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _usersListCurrentPage = 1;
        private int _usersListPageSize = 50;
        private int _usersListTotalPages = 0;
        public MainWindow()
        {
            InitializeComponent();
            Utils.Utils._mainWindow = this;
            Utils.Utils._defaultImage = new BitmapImage(new Uri(@"pack://application:,,,/CandlesCompany;component/Resources/Images/Items/notfound.png"));
            Utils.Utils._listViewBasket = ListViewBasket;
            Utils.Utils._dataGridOrdersList = DataGridOrdersList;
            _usersListTotalPages = (int)Math.Ceiling((decimal)DBManager.GetUsersCount() / _usersListPageSize);
            TextBlockUsersTotalPage.Text = $"Всего страниц: {_usersListTotalPages}";
            TextBoxUsersPage.Text = "1";
            ButtonUsersListPageBack.IsEnabled = false;

            CatalogInit();
            BasketInit();
            OrdersInit();
            ProfileInit();
            SelectedItemInit();
            SummaryInformationInit();

            ReloadWindowManagementUserList();
            AddressesListReload();
        }
        private void SummaryInformationInit()
        {
            Utils.Utils._summaryInformation = new SummaryInformation();
            GridBasketItems.Children.Add(Utils.Utils._summaryInformation);
            Utils.Utils._summaryInformation.SetValue(Grid.ColumnProperty, 1);
            Utils.Utils._summaryInformation.SetValue(Grid.RowProperty, 0);
        }
        private void SelectedItemInit()
        {
            Utils.Utils._selectediteminfo = new SelectedItemInfo();
            GridCatalogItems.Children.Add(Utils.Utils._selectediteminfo);
            Utils.Utils._selectediteminfo.SetValue(Grid.ColumnProperty, 1);
            Utils.Utils._selectediteminfo.SetValue(Grid.RowProperty, 0);
            Utils.Utils._selectediteminfo.Visibility = Visibility.Collapsed;
        }
        private void ProfileInit()
        {
            TextBlockProfileName.Text = $"ФИО: {UserCache._last_name} {UserCache._first_name} {UserCache._middle_name}";
            TextBlockProfilePhone.Text = $"Телефон: {UserCache._phone}";
            TextBlockProfileEmail.Text = $"Эл. почта: {UserCache._email}";
            TextBlockProfileRole.Text = $"Должность: {UserCache._role.Name}";
            ImageBrushProfileAvatar.ImageSource = UserCache._avatar;
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
                            o.Order_Status.Id,
                            o.Order_Address.Address
                        ));
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
        private void AddressesListReload()
        {
            DataGridAddressesList.Items.Clear();
            Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Clear();
            Task.Run(async () =>
            {
                Utils.Utils._Addresses = DBManager.GetAddresses();
                await Dispatcher.InvokeAsync(() =>
                {
                    Utils.Utils._Addresses.ForEach((address) =>
                    {
                        DataGridAddressesList.Items.Add(address);
                        Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Add(new ComboBoxItem
                        {
                            Content = $"{address.Address}",
                            Tag = address
                        });
                    });
                });
            });
        }
        private void ButtonManagementAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    new Employee.EmployeeAddWindow().Show();
                });
            });
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
        private void ButtonProfileSetAvatar_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Utils.Utils.GetImageWindowsDialog();
            if (image == null)
            {
                return;
            }
            ImageBrushProfileAvatar.ImageSource = image;
            DBManager.SetAvatarUser(Utils.Utils.ImageToBinary(image));
        }
        private void ButtonProfileRemoveAvatar_Click(object sender, RoutedEventArgs e)
        {
            if (ImageBrushProfileAvatar.ImageSource == Utils.Utils._defaultAvatar)
            {
                MessageBox.Show("У вас нету фотографии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ImageBrushProfileAvatar.ImageSource = Utils.Utils._defaultAvatar;
            DBManager.RemoveAvatarUser();
        }
        private void TextBoxSearchAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TextBoxSearchAddress.Text;

            if (search.Length == 0)
            {
                AddressesListReload();
                return;
            }

            DataGridAddressesList.Items.Clear();
            Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Utils.Utils._Addresses.ForEach(address =>
                    {
                        if (address.Address.Contains(search))
                        {
                            DataGridAddressesList.Items.Add(address);
                        }
                    });
                });
            });
        }
        private void ButtonAddressListSelected_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DBManager.RemoveAddress((int)button.Tag);
            AddressesListReload();
        }
        private void ButtonAddAddresses_Click(object sender, RoutedEventArgs e)
        {
            string address = TextBoxAddAddress.Text;

            if (address.Length == 0)
            {
                MessageBox.Show("Пустой адрес!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (address.Length < 5)
            {
                MessageBox.Show("Слишмко короткий адрес!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    DBManager.AddAddresses(address);
                    AddressesListReload();
                    MessageBox.Show("Вы добавили новый адрес!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }
        public void ReloadWindowManagementUserList()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(delegate ()
                {
                    DataGridManagementUsersList.Items.Clear();
                    DBManager.GetUsersForPage(_usersListCurrentPage).ForEach(user =>
                    {
                        BitmapImage avatar = null;
                        if (user.Avatar == null)
                        {
                            avatar = Utils.Utils._defaultAvatar;
                        }
                        else
                        {
                            avatar = Utils.Utils.BinaryToImage(user.Avatar);
                        }

                        DataGridManagementUsersList.Items.Add(new UI.UsersList(user.Id, $"{user.Last_Name} {user.First_Name} {user.Middle_Name}", user.Email, avatar));
                    });
                });
            }).Start();
        }
        private void DataGridManagementMenuItemRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadWindowManagementUserList();
        }
        private void ButtonUsersListPageBack_Click(object sender, RoutedEventArgs e)
        {
            _usersListCurrentPage -= 1;
            if (_usersListCurrentPage <= 1)
            {
                ButtonUsersListPageBack.IsEnabled = false;
            }

            ButtonUsersListPageNext.IsEnabled = true;
            TextBoxUsersPage.Text = _usersListCurrentPage.ToString();
            ReloadWindowManagementUserList();
        }
        private void ButtonUsersListPageNext_Click(object sender, RoutedEventArgs e)
        {
            _usersListCurrentPage += 1;
            if (_usersListCurrentPage >= _usersListTotalPages)
            {
                ButtonUsersListPageNext.IsEnabled = false;
            }

            ButtonUsersListPageBack.IsEnabled = true;
            TextBoxUsersPage.Text = _usersListCurrentPage.ToString();
            ReloadWindowManagementUserList();
        }
        private void TextBoxUsersSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TextBoxUsersSearch.Text;
            Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    if (search.Length == 0)
                    {
                        ReloadWindowManagementUserList();
                        return;
                    }

                    DataGridManagementUsersList.Items.Clear();
                    DBManager.FindUsers(search).ForEach(user =>
                    {
                        BitmapImage avatar = null;
                        if (user.Avatar == null)
                        {
                            avatar = Utils.Utils._defaultAvatar;
                        }
                        else
                        {
                            avatar = Utils.Utils.BinaryToImage(user.Avatar);
                        }

                        DataGridManagementUsersList.Items.Add(new UI.UsersList(user.Id, $"{user.Last_Name} {user.First_Name} {user.Middle_Name}", user.Email, avatar));
                    });
                });
            });
        }
        private void TextBoxUsersPage_DragEnter(object sender, DragEventArgs e)
        {
            string text = TextBoxUsersPage.Text;
            if (!new Regex(@"^[0-9]+$").IsMatch(text))
            {
                return;
            }

            _usersListCurrentPage = Convert.ToInt32(text);
            ReloadWindowManagementUserList();
        }
    }
}
