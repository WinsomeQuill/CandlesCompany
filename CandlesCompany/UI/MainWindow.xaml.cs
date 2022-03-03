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
        private int _listPageSize { get; } = 25;

        private int _usersListCurrentPage { get; set; } = 1;
        private int _employeesListCurrentPage { get; set; } = 1;
        private int _ordersListCurrentPage { get; set; } = 1;

        private int _usersListTotalPages { get; set; }
        private int _employeesListTotalPages { get; set; }
        private int _ordersListTotalPages { get; set; }

        private List<Users> _usersList = new List<Users>();
        private List<Users> _employeesList = new List<Users>();
        private List<Orders> _ordersList { get; set; } = new List<Orders>();

        private bool _isSearchUsersList = false;
        private bool _isSearchEmployeesList = false;
        private bool _isSearchAddressesList = false;
        private bool _isSearchOrdersList = false;
        public MainWindow()
        {
            InitializeComponent();
            Utils.Utils._mainWindow = this;
            Utils.Utils._defaultImage = new BitmapImage(new Uri(@"pack://application:,,,/CandlesCompany;component/Resources/Images/Items/notfound.png"));
            Utils.Utils._listViewBasket = ListViewBasket;
            Utils.Utils._dataGridOrdersList = DataGridOrdersList;
            Utils.Utils._roles = DBManager.GetRoles();

            CatalogInit();
            BasketInit();
            ReloadOrdersList();
            ProfileInit();
            SelectedItemInit();
            SummaryInformationInit();

            ReloadWindowManagementUsersList();
            ReloadWindowManagementEmployeesList();
            ReloadWindowManagementAddressesList();
            ReloadWindowManagementOrdersList();
        }
        private void SummaryInformationInit()
        {
            Utils.Utils._summaryInformation = new SummaryInformation();
            GridBasketItems.Children.Add(Utils.Utils._summaryInformation);
            Utils.Utils._summaryInformation.SetValue(Grid.ColumnProperty, 1);
            Utils.Utils._summaryInformation.SetValue(Grid.RowProperty, 0);
            ReloadComboBoxSummaryInformationAddress();
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


        private void ReloadWindowManagementEmployeesList()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    ProgressBarManagementEmployeesList.Visibility = Visibility.Visible;
                    DataGridManagementEmployeesList.Visibility = Visibility.Collapsed;

                    DataGridManagementEmployeesList.Items.Clear();
                    if (!_isSearchEmployeesList)
                    {
                        _employeesList = await DBManager.GetEmployees(_employeesListCurrentPage, 1, 6, _listPageSize);
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

                            DataGridManagementEmployeesList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                        });
                        SetPagesEmployeesList(DBManager.GetEmployeesCount());
                    }
                    else
                    {
                        _employeesList.Skip(_listPageSize * (_employeesListCurrentPage - 1)).Take(_listPageSize).ToList().ForEach(user =>
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

                            DataGridManagementEmployeesList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                        });
                        SetPagesEmployeesList(_employeesList.Count() - 1);
                    }

                    ProgressBarManagementEmployeesList.Visibility = Visibility.Collapsed;
                    DataGridManagementEmployeesList.Visibility = Visibility.Visible;
                });
            }).Start();
        }
        private void SetPagesEmployeesList(int countEmployees)
        {
            ButtonManagementEmployeesPageBack.IsEnabled = ButtonManagementEmployeesPageNext.IsEnabled = true;
            _employeesListTotalPages = (int)Math.Ceiling((decimal)countEmployees / _listPageSize);
            TextBlockManagementEmployeesTotalPage.Text = $"Всего страниц: {_employeesListTotalPages}";
            TextBoxManagementEmployeesPage.Text = _employeesListCurrentPage.ToString();
            if (_employeesListCurrentPage == 1)
            {
                ButtonManagementEmployeesPageBack.IsEnabled = false;
            }
            else
            {
                ButtonManagementEmployeesPageBack.IsEnabled = true;
            }

            if (_employeesListCurrentPage >= _employeesListTotalPages)
            {
                ButtonManagementEmployeesPageNext.IsEnabled = false;
            }
            else
            {
                ButtonManagementEmployeesPageNext.IsEnabled = true;
            }

            if (countEmployees <= _listPageSize)
            {
                ButtonManagementEmployeesPageNext.IsEnabled = false;
            }
        }
        private async void ButtonDataGridManagementEmployeesRemove_Click(object sender, RoutedEventArgs e)
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

                DataGridManagementEmployeesList.Items.Clear();

                _employeesList = await DBManager.GetEmployees(_employeesListCurrentPage, 1, 6, _listPageSize);
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

                    DataGridManagementEmployeesList.Items.Add(new UI.UsersList(employee, avatar, Utils.Utils._roles));
                });
            }
        }
        private void ButtonManagementEmployeesPageBack_Click(object sender, RoutedEventArgs e)
        {
            _employeesListCurrentPage -= 1;
            if (_employeesListCurrentPage <= 1)
            {
                ButtonManagementEmployeesPageBack.IsEnabled = false;
            }

            ButtonManagementEmployeesPageNext.IsEnabled = true;
            TextBoxManagementEmployeesPage.Text = _employeesListCurrentPage.ToString();
            ReloadWindowManagementEmployeesList();
        }
        private void ButtonManagementEmployeesPageNext_Click(object sender, RoutedEventArgs e)
        {
            _employeesListCurrentPage += 1;
            if (_employeesListCurrentPage >= _employeesListTotalPages)
            {
                ButtonManagementEmployeesPageNext.IsEnabled = false;
            }

            ButtonManagementEmployeesPageBack.IsEnabled = true;
            TextBoxManagementEmployeesPage.Text = _employeesListCurrentPage.ToString();
            ReloadWindowManagementEmployeesList();
        }
        private void ButtonManagementEmployeesSearch_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSearchEmployeesList = true;
                    ProgressBarManagementEmployeesList.Visibility = Visibility.Visible;
                    DataGridManagementEmployeesList.Visibility = Visibility.Collapsed;
                    string search = TextBoxManagementEmployeesSearch.Text;
                    if (search.Length == 0)
                    {
                        _isSearchEmployeesList = false;
                        ReloadWindowManagementEmployeesList();
                        return;
                    }

                    ButtonManagementEmployeesSearch.IsEnabled = false;
                    _employeesList.Clear();
                    _employeesList = await DBManager.FindEmployees(search);
                    ReloadWindowManagementEmployeesList();
                    ButtonManagementEmployeesSearch.IsEnabled = true;
                });
            }).Start();
        }
        private void ComboBoxManagementEmployeesRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                Users user = grid.Tag as Users;
                DBManager.ChangeRoleById(user.Id, comboBox.SelectedItem.ToString());
                ReloadWindowManagementEmployeesList();
            }
        }
        private void TextBoxManagementEmployeesPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxManagementEmployeesPage.Text;
                if (!new Regex(@"^[0-9]+$").IsMatch(text))
                {
                    return;
                }

                int page = Convert.ToInt32(text);
                if (_employeesListCurrentPage == page)
                {
                    return;
                }

                _employeesListCurrentPage = page;

                if (_employeesListCurrentPage >= _employeesListTotalPages)
                {
                    _employeesListCurrentPage = _employeesListTotalPages;
                    TextBoxManagementEmployeesPage.Text = $"{_employeesListCurrentPage}";
                }

                ReloadWindowManagementEmployeesList();
            }
        }
        private void DataGridManagementEmployeesListRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadWindowManagementEmployeesList();
        }


        public void ReloadWindowManagementUsersList()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    ProgressBarManagementUsersList.Visibility = Visibility.Visible;
                    DataGridManagementUsersList.Visibility = Visibility.Collapsed;

                    DataGridManagementUsersList.Items.Clear();
                    if (!_isSearchUsersList)
                    {
                        _usersList = await DBManager.GetUsersForPage(_usersListCurrentPage, _listPageSize);
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

                            DataGridManagementUsersList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                        });
                        SetPagesUsersList(DBManager.GetUsersCount());
                    }
                    else
                    {
                        _usersList.Skip(_listPageSize * (_usersListCurrentPage - 1)).Take(_listPageSize).ToList().ForEach(user =>
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

                            DataGridManagementUsersList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                        });
                        SetPagesUsersList(_usersList.Count() - 1);
                    }

                    ProgressBarManagementUsersList.Visibility = Visibility.Collapsed;
                    DataGridManagementUsersList.Visibility = Visibility.Visible;
                });
            }).Start();
        }
        private void SetPagesUsersList(int countUsers)
        {
            ButtonManagementUsersListPageBack.IsEnabled = ButtonManagementUsersListPageNext.IsEnabled = true;
            _usersListTotalPages = (int)Math.Ceiling((decimal)countUsers / _listPageSize);
            TextBlockManagementUsersTotalPage.Text = $"Всего страниц: {_usersListTotalPages}";
            TextBoxManagementUsersPage.Text = _usersListCurrentPage.ToString();
            if (_usersListCurrentPage == 1)
            {
                ButtonManagementUsersListPageBack.IsEnabled = false;
            }
            else
            {
                ButtonManagementUsersListPageBack.IsEnabled = true;
            }

            if (_usersListCurrentPage >= _usersListTotalPages)
            {
                ButtonManagementUsersListPageNext.IsEnabled = false;
            }
            else
            {
                ButtonManagementUsersListPageNext.IsEnabled = true;
            }

            if (countUsers <= _listPageSize)
            {
                ButtonManagementUsersListPageNext.IsEnabled = false;
            }
        }
        private void ButtonManagementUsersListPageBack_Click(object sender, RoutedEventArgs e)
        {
            _usersListCurrentPage -= 1;
            ReloadWindowManagementUsersList();
        }
        private void ButtonManagementUsersListPageNext_Click(object sender, RoutedEventArgs e)
        {
            _usersListCurrentPage += 1;
            ReloadWindowManagementUsersList();
        }
        private void ButtonManagementUsersSearch_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSearchUsersList = true;
                    ProgressBarManagementUsersList.Visibility = Visibility.Visible;
                    DataGridManagementUsersList.Visibility = Visibility.Collapsed;
                    string search = TextBoxManagementUsersSearch.Text;
                    if (search.Length == 0)
                    {
                        _isSearchUsersList = false;
                        ReloadWindowManagementUsersList();
                        return;
                    }

                    ButtonManagementUsersSearch.IsEnabled = false;
                    _usersList.Clear();
                    _usersList = await DBManager.FindUsers(search);
                    ReloadWindowManagementUsersList();
                    ButtonManagementUsersSearch.IsEnabled = true;
                });
            }).Start();
        }
        private void ComboBoxManagementUsersListSetRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                Users user = grid.Tag as Users;
                DBManager.ChangeRoleById(user.Id, comboBox.SelectedItem.ToString());
                ReloadWindowManagementUsersList();
            }
        }
        private void TextBoxManagementUsersPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxManagementUsersPage.Text;
                if (!new Regex(@"^[0-9]+$").IsMatch(text))
                {
                    return;
                }

                int page = Convert.ToInt32(text);
                if (_usersListCurrentPage == page)
                {
                    return;
                }

                _usersListCurrentPage = page;

                if (_usersListCurrentPage >= _usersListTotalPages)
                {
                    _usersListCurrentPage = _usersListTotalPages;
                    TextBoxManagementUsersPage.Text = $"{_usersListCurrentPage}";
                }

                ReloadWindowManagementUsersList();
            }
        }
        private void DataGridManagementUsersListMenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadWindowManagementUsersList();
        }


        private void ReloadWindowManagementAddressesList()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    ProgressBarManagementAddressesList.Visibility = Visibility.Visible;
                    DataGridManagementAddressesList.Visibility = Visibility.Collapsed;

                    DataGridManagementAddressesList.Items.Clear();
                    Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Clear();
                    if (!_isSearchAddressesList)
                    {
                        Utils.Utils._addresses = await DBManager.GetAddresses();
                        Utils.Utils._addresses.ForEach(address =>
                        {
                            DataGridManagementAddressesList.Items.Add(address);
                        });
                    }
                    else
                    {
                        Utils.Utils._addresses.ForEach(address =>
                        {
                            if (address.Address.Contains(TextBoxManagementAddressesListSearch.Text))
                            {
                                DataGridManagementAddressesList.Items.Add(address);
                            }
                        });
                    }

                    ProgressBarManagementAddressesList.Visibility = Visibility.Collapsed;
                    DataGridManagementAddressesList.Visibility = Visibility.Visible;
                });
            }).Start();
        }
        private void ReloadComboBoxSummaryInformationAddress()
        {
            new Task(async () =>
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    if (Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Count != 0)
                    {
                        Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Clear();
                    }

                    Utils.Utils._addresses = await DBManager.GetAddresses();
                    Utils.Utils._addresses.ForEach(address =>
                    {
                        Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Add(new ComboBoxItem
                        {
                            Content = $"{address.Address}",
                            Tag = address
                        });
                    });
                });
            }).Start();
        }
        private void ButtonManagementAddressesListRemove_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DBManager.RemoveAddress((int)button.Tag);
            ReloadWindowManagementAddressesList();
            ReloadComboBoxSummaryInformationAddress();
        }
        private void ButtonManagementAddressesListAdd_Click(object sender, RoutedEventArgs e)
        {
            string address = TextBoxManagementAddressesListAdd.Text;

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
                    Order_Address result = DBManager.AddAddresses(address);
                    Utils.Utils._addresses.Add(result);
                    ReloadWindowManagementAddressesList();
                    ReloadComboBoxSummaryInformationAddress();
                    TextBoxManagementAddressesListAdd.Text = String.Empty;
                    MessageBox.Show("Вы добавили новый адрес!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }
        private void DataGridManagementAddressesListMenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadWindowManagementAddressesList();
        }
        private void ButtonManagementAddressesListPageBack_Click(object sender, RoutedEventArgs e) // ?
        {

        }
        private void TextBoxManagementAddressesListPage_KeyDown(object sender, KeyEventArgs e) // ?
        {

        }
        private void ButtonManagementAddressesListPageNext_Click(object sender, RoutedEventArgs e) // ?
        {

        }
        private void ButtonManagementAddressesListSearch_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(() =>
                {
                    _isSearchAddressesList = true;
                    ButtonManagementAddressesListSearch.IsEnabled = false;
                    ProgressBarManagementAddressesList.Visibility = Visibility.Visible;
                    DataGridManagementAddressesList.Visibility = Visibility.Collapsed;
                    string search = TextBoxManagementAddressesListSearch.Text;
                    if (search.Length == 0)
                    {
                        _isSearchAddressesList = false;
                    }

                    ReloadWindowManagementAddressesList();
                    ButtonManagementAddressesListSearch.IsEnabled = true;
                });
            }).Start();
        }


        private void ReloadOrdersList()
        {
            new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    if (DataGridOrdersList.Items.Count != 0)
                    {
                        DataGridOrdersList.Items.Clear();
                    }

                    DBManager.GetOrders(UserCache._id).ForEach(o =>
                    {
                        DataGridOrdersList.Items.Add(new Custom.Orders.OrderList(o));
                    });
                });
            }).Start();
        }
        private void DatGridOrdersListRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadOrdersList();
        }

        
        private void SetPagesOrdersList(int countOrders)
        {
            ButtonManagementOrdersPageBack.IsEnabled = ButtonManagementOrdersPageNext.IsEnabled = true;
            _ordersListTotalPages = (int)Math.Ceiling((decimal)countOrders / _listPageSize);
            TextBlockManagementOrdersTotalPage.Text = $"Всего страниц: {_ordersListTotalPages}";
            TextBoxManagementOrdersPage.Text = _ordersListCurrentPage.ToString();
            if (_ordersListCurrentPage == 1)
            {
                ButtonManagementOrdersPageBack.IsEnabled = false;
            }
            else
            {
                ButtonManagementOrdersPageBack.IsEnabled = true;
            }

            if (_ordersListCurrentPage >= _ordersListTotalPages)
            {
                ButtonManagementOrdersPageNext.IsEnabled = false;
            }
            else
            {
                ButtonManagementOrdersPageNext.IsEnabled = true;
            }

            if (countOrders <= _listPageSize)
            {
                ButtonManagementOrdersPageNext.IsEnabled = false;
            }
        }
        private void ReloadWindowManagementOrdersList()
        {
            new Thread(delegate ()
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    ProgressBarManagementOrdersList.Visibility = Visibility.Visible;
                    DataGridManagementOrdersList.Visibility= Visibility.Collapsed;

                    DataGridManagementOrdersList.Items.Clear();
                    List<string> orders_statutes = await DBManager.GetStatusList();
                    if (!_isSearchOrdersList)
                    {
                        
                        _ordersList.Clear();
                        _ordersList = await DBManager.GetOrdersForPage(_ordersListCurrentPage, _listPageSize);
                        _ordersList.ForEach(o =>
                        {
                            DataGridManagementOrdersList.Items.Add(new UI.Custom.Orders.OrderList(o, o.Users, orders_statutes));
                        });
                        SetPagesOrdersList(await DBManager.GetOrdersCount());
                    }
                    else
                    {
                        _ordersList.Skip(_listPageSize * (_ordersListCurrentPage - 1)).Take(_listPageSize).ToList().ForEach(o =>
                        {
                            DataGridManagementOrdersList.Items.Add(new UI.Custom.Orders.OrderList(o, o.Users, orders_statutes));
                        });
                        SetPagesOrdersList(_ordersList.Count() - 1);
                    }

                    ProgressBarManagementOrdersList.Visibility = Visibility.Collapsed;
                    DataGridManagementOrdersList.Visibility = Visibility.Visible;
                    GC.Collect();
                });
            }).Start();
        }
        private void DataGridManagementOrdersListRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadWindowManagementOrdersList();
        }
        private void ComboBoxManagementOrdersStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                Orders order = grid.Tag as Orders;
                DBManager.ChangeOrderStatus(order.Id, comboBox.SelectedItem.ToString());
                ReloadWindowManagementOrdersList();
            }
        }
        private void ButtonManagementOrdersPageBack_Click(object sender, RoutedEventArgs e)
        {
            _ordersListCurrentPage -= 1;
            ReloadWindowManagementOrdersList();
        }
        private void ButtonManagementOrdersPageNext_Click(object sender, RoutedEventArgs e)
        {
            _ordersListCurrentPage += 1;
            ReloadWindowManagementOrdersList();
        }
        private void TextBoxManagementOrdersPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxManagementOrdersPage.Text;
                if (!new Regex(@"^[0-9]+$").IsMatch(text))
                {
                    return;
                }

                int page = Convert.ToInt32(text);
                if (_ordersListCurrentPage == page)
                {
                    return;
                }

                _ordersListCurrentPage = page;

                if (_ordersListCurrentPage >= _ordersListTotalPages)
                {
                    _ordersListCurrentPage = _ordersListTotalPages;
                    TextBoxManagementUsersPage.Text = $"{_ordersListCurrentPage}";
                }

                ReloadWindowManagementOrdersList();
            }
        }
        private void ButtonManagementOrdersSearch_Click(object sender, RoutedEventArgs e)
        {
            new Task(async () =>
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    _isSearchOrdersList = true;
                    ProgressBarManagementOrdersList.Visibility = Visibility.Visible;
                    DataGridManagementOrdersList.Visibility = Visibility.Collapsed;
                    string search = TextBoxManagementOrdersSearch.Text;
                    if (search.Length == 0)
                    {
                        _isSearchOrdersList = false;
                        ReloadWindowManagementOrdersList();
                        return;
                    }

                    ButtonManagementOrdersSearch.IsEnabled = false;
                    _ordersList.Clear();
                    _ordersList = await DBManager.FindOrders(search);
                    ReloadWindowManagementOrdersList();
                    ButtonManagementOrdersSearch.IsEnabled = true;
                });
            }).Start();
        }
    }
}
