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
using System.Windows.Threading;

namespace CandlesCompany.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _listPageSize { get; set; } = 50;

        private int _usersListCurrentPage { get; set; }
        private int _employeesListCurrentPage { get; set; }

        private int _usersListTotalPages { get; set; }
        private int _employeesListTotalPages { get; set; }

        private List<Users> _usersList = new List<Users>();
        private List<Users> _employeesList = new List<Users>();

        private bool _isSearchUsersList = false;
        private bool _isSearchEmployeesList = false;
        public MainWindow()
        {
            InitializeComponent();
            Utils.Utils._mainWindow = this;
            Utils.Utils._defaultImage = new BitmapImage(new Uri(@"pack://application:,,,/CandlesCompany;component/Resources/Images/Items/notfound.png"));
            Utils.Utils._listViewBasket = ListViewBasket;
            Utils.Utils._dataGridOrdersList = DataGridOrdersList;
            Utils.Utils._roles = DBManager.GetRoles();

            SetPagesUsersList(DBManager.GetUsersCount());
            SetPagesEmployeesList(DBManager.GetEmployeesCount());

            CatalogInit();
            BasketInit();
            OrdersInit();
            ProfileInit();
            SelectedItemInit();
            SummaryInformationInit();

            ReloadWindowManagementUserList();
            ReloadWindowManagementEmployeeList();
            AddressesListReload();

        }
        private void ReloadWindowManagementEmployeeList()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    ProgressBarManagementEmployeeList.Visibility = Visibility.Visible;
                    DataGridManagementEmployeeList.Visibility = Visibility.Collapsed;

                    DataGridManagementEmployeeList.Items.Clear();
                    if (!_isSearchEmployeesList)
                    {
                        _employeesList = await DBManager.GetEmployees(_employeesListCurrentPage);
                        _employeesList.ForEach(user =>
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

                            DataGridManagementEmployeeList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                        });
                    }
                    else
                    {
                        _employeesList.Skip(_listPageSize * (_employeesListCurrentPage - 1)).Take(50).ToList().ForEach(user =>
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

                            DataGridManagementEmployeeList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                        });
                    }

                    ProgressBarManagementEmployeeList.Visibility = Visibility.Collapsed;
                    DataGridManagementEmployeeList.Visibility = Visibility.Visible;
                });
            }).Start();
        }
        public void ReloadWindowManagementUserList()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    ProgressBarUsersList.Visibility = Visibility.Visible;
                    DataGridManagementUsersList.Visibility = Visibility.Collapsed;

                    DataGridManagementUsersList.Items.Clear();
                    if (!_isSearchUsersList)
                    {
                        _usersList = await DBManager.GetUsersForPage(_usersListCurrentPage);
                        _usersList.ForEach(user =>
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

                            DataGridManagementUsersList.Items.Add(new UI.UsersList(user, avatar));
                        });
                    }
                    else
                    {
                        _usersList.Skip(_listPageSize * (_usersListCurrentPage - 1)).Take(50).ToList().ForEach(user =>
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

                            DataGridManagementUsersList.Items.Add(new UI.UsersList(user, avatar));
                        });
                    }

                    ProgressBarUsersList.Visibility = Visibility.Collapsed;
                    DataGridManagementUsersList.Visibility = Visibility.Visible;
                });
            }).Start();
        }
        private void SetPagesEmployeesList(int countEmployees)
        {
            ButtonManagementEmployeePageBack.IsEnabled = ButtonManagementEmployeePageNext.IsEnabled = true;
            _employeesListTotalPages = (int)Math.Ceiling((decimal)countEmployees / _listPageSize);
            TextBlockManagementEmployeeTotalPage.Text = $"Всего страниц: {_employeesListTotalPages}";
            TextBoxManagementEmployeesPage.Text = "1";
            _employeesListCurrentPage = 1;
            ButtonManagementEmployeePageBack.IsEnabled = false;
            if (countEmployees <= _listPageSize)
            {
                ButtonManagementEmployeePageNext.IsEnabled = false;
            }
        }
        private void SetPagesUsersList(int countUsers)
        {
            ButtonUsersListPageBack.IsEnabled = ButtonUsersListPageNext.IsEnabled = true;
            _usersListTotalPages = (int)Math.Ceiling((decimal)countUsers / _listPageSize);
            TextBlockUsersTotalPage.Text = $"Всего страниц: {_usersListTotalPages}";
            TextBoxUsersPage.Text = "1";
            _usersListCurrentPage = 1;
            ButtonUsersListPageBack.IsEnabled = false;
            if (countUsers <= _listPageSize)
            {
                ButtonUsersListPageNext.IsEnabled = false;
            }
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
        private void ButtonUsersSearch_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSearchUsersList = true;
                    ProgressBarUsersList.Visibility = Visibility.Visible;
                    DataGridManagementUsersList.Visibility = Visibility.Collapsed;
                    string search = TextBoxUsersSearch.Text;
                    if (search.Length == 0)
                    {
                        _isSearchUsersList = false;
                        ReloadWindowManagementUserList();
                        return;
                    }

                    ButtonUsersSearch.IsEnabled = false;
                    _usersList.Clear();
                    _usersList = await DBManager.FindUsers(search);
                    ReloadWindowManagementUserList();
                    ButtonUsersSearch.IsEnabled = true;
                    
                    SetPagesUsersList(_usersList.Count() - 1);
                });
            }).Start();
        }
        private async void ButtonDataGridManagementEmployeeRemove_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                Grid grid = button.Parent as Grid;
                Users user = grid.Tag as Users;

                MessageBoxResult result = MessageBox.Show($"Вы действительно хотите снять с должности \"{user.Roles.Name}\" сотрудника \"{user.Last_Name} {user.First_Name}\"", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No) { return; }

                MessageBox.Show($"Вы сняли с должности \"{user.Roles.Name}\" сотрудника \"{user.Last_Name} {user.First_Name}\"!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DBManager.ChangeRoleById(user.Id, "Пользователь");

                DataGridManagementEmployeeList.Items.Clear();

                _employeesList = await DBManager.GetEmployees(_employeesListCurrentPage);
                _employeesList.ForEach(employee =>
                {
                    BitmapImage avatar = null;
                    if (employee.Avatar == null)
                    {
                        avatar = Utils.Utils._defaultAvatar;
                    }
                    else
                    {
                        avatar = Utils.Utils.BinaryToImage(employee.Avatar);
                    }

                    DataGridManagementEmployeeList.Items.Add(new UI.UsersList(employee, avatar, Utils.Utils._roles));
                });
            }
        }
        private T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            T parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }
        private void ButtonManagementEmployeePageBack_Click(object sender, RoutedEventArgs e)
        {
            _employeesListCurrentPage -= 1;
            if (_employeesListCurrentPage <= 1)
            {
                ButtonManagementEmployeePageBack.IsEnabled = false;
            }

            ButtonManagementEmployeePageNext.IsEnabled = true;
            TextBoxManagementEmployeesPage.Text = _employeesListCurrentPage.ToString();
            ReloadWindowManagementEmployeeList();
        }
        private void ButtonManagementEmployeePageNext_Click(object sender, RoutedEventArgs e)
        {
            _employeesListCurrentPage += 1;
            if (_employeesListCurrentPage >= _employeesListTotalPages)
            {
                ButtonManagementEmployeePageNext.IsEnabled = false;
            }

            ButtonManagementEmployeePageBack.IsEnabled = true;
            TextBoxManagementEmployeesPage.Text = _employeesListCurrentPage.ToString();
            ReloadWindowManagementEmployeeList();
        }
        private void ButtonManagementEmployeeSearch_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSearchEmployeesList = true;
                    ProgressBarManagementEmployeeList.Visibility = Visibility.Visible;
                    DataGridManagementEmployeeList.Visibility = Visibility.Collapsed;
                    string search = TextBoxManagementEmployeeSearch.Text;
                    if (search.Length == 0)
                    {
                        _isSearchEmployeesList = false;
                        ReloadWindowManagementEmployeeList();
                        return;
                    }

                    ButtonManagementEmployeeSearch.IsEnabled = false;
                    _employeesList.Clear();
                    _employeesList = await DBManager.FindEmployees(search);
                    ReloadWindowManagementEmployeeList();
                    ButtonManagementEmployeeSearch.IsEnabled = true;

                    SetPagesEmployeesList(_employeesList.Count() - 1);
                });
            }).Start();
        }
        private void ComboBoxManagementEmployeeRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                Grid grid = comboBox.Parent as Grid;
                Users user = grid.Tag as Users;
                DBManager.ChangeRoleById(user.Id, comboBox.SelectedItem.ToString());
                ReloadWindowManagementEmployeeList();
            }
        }
        private void TextBoxManagementEmployeesPage_KeyDown(object sender, KeyEventArgs e)
        {
            string text = TextBoxManagementEmployeesPage.Text;
            if (!new Regex(@"^[0-9]+$").IsMatch(text))
            {
                return;
            }

            _employeesListCurrentPage = Convert.ToInt32(text);
            if (_employeesListCurrentPage >= _employeesListTotalPages)
            {
                _employeesListCurrentPage = _employeesListTotalPages;
                TextBoxManagementEmployeesPage.Text = $"{_employeesListCurrentPage}";
            }

            ReloadWindowManagementEmployeeList();
        }
        private void TextBoxUsersPage_KeyDown(object sender, KeyEventArgs e)
        {
            string text = TextBoxUsersPage.Text;
            if (!new Regex(@"^[0-9]+$").IsMatch(text))
            {
                return;
            }

            _usersListCurrentPage = Convert.ToInt32(text);
            if (_usersListCurrentPage >= _usersListTotalPages)
            {
                _usersListCurrentPage = _usersListTotalPages;
                TextBoxUsersPage.Text = $"{_usersListCurrentPage}";
            }

            ReloadWindowManagementUserList();
        }
        private void DataGridManagementUsersListMenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadWindowManagementUserList();
        }
        private void DataGridManagementEmployeesListRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadWindowManagementEmployeeList();
        }
    }
}
