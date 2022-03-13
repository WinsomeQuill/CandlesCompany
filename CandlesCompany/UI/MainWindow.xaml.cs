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
using MaterialDesignThemes.Wpf;

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

        private List<Users> _usersList { get; set; } = new List<Users>();
        private List<Users> _employeesList { get; set; } = new List<Users>();
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
            ProfileInit();
            SelectedItemInit();

            if(Cache.UserCache._role.Id != 4 && Cache.UserCache._role.Id != 5) // if user not admin/manager
            {
                TabItemAdmin.Visibility = Visibility.Collapsed;

                Parallel.Invoke(
                () =>
                {
                    new Thread(async () =>
                    {
                        await Dispatcher.InvokeAsync(async () =>
                        {
                            await SummaryInformationInit();
                            await BasketInit();
                            await CatalogInit();
                            await ReloadOrdersList();
                        });
                    }).Start();
                });
            }
            else // if user is admin/manager
            {
                Parallel.Invoke(
                () =>
                {
                    new Thread(async () =>
                    {
                        await Dispatcher.InvokeAsync(async () =>
                        {
                            Utils.Utils._roles = await DBManager.GetRoles();
                        });
                    }).Start();
                },
                () =>
                {
                    new Thread(async () =>
                    {
                        await Dispatcher.InvokeAsync(async () =>
                        {
                            await SummaryInformationInit();
                            await BasketInit();
                            await CatalogInit();
                            await ReloadOrdersList();
                        });
                    }).Start();
                });

                Parallel.Invoke(
                async () =>
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        await ReloadWindowManagementEmployeesList();
                    });
                },
                async () =>
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        await ReloadWindowManagementUsersList();
                    });
                },
                async () =>
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        await ReloadWindowManagementAddressesList();
                    });
                },
                async () =>
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        await ReloadWindowManagementOrdersList();
                    });
                });
            }
        }
        private async Task SummaryInformationInit()
        {
            Utils.Utils._summaryInformation = new SummaryInformation();
            GridBasketItems.Children.Add(Utils.Utils._summaryInformation);
            Utils.Utils._summaryInformation.SetValue(Grid.ColumnProperty, 1);
            Utils.Utils._summaryInformation.SetValue(Grid.RowProperty, 0);
            await ReloadComboBoxSummaryInformationAddress();
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
            TextBlockProfilePhone.Text = UserCache._phone == null ? "Телефон: Не указан" : $"Телефон: +{UserCache._phone}";
            TextBlockProfileEmail.Text = $"Эл. почта: {UserCache._email}";
            TextBlockProfileRole.Text = $"Должность: {UserCache._role.Name}";
            ImageBrushProfileAvatar.ImageSource = UserCache._avatar;
        }
        private async Task BasketInit()
        {
            Dictionary<Candles, int> basket = await DBManager.GetCandlesBasket(UserCache._id);

            if (basket.Count() == 0)
            {
                TextBlockBasket.Visibility = Visibility.Visible;
                ListViewBasket.Visibility = Visibility.Collapsed;
                return;
            }

            TextBlockBasket.Visibility = Visibility.Collapsed;
            ListViewBasket.Visibility = Visibility.Visible;

            foreach (KeyValuePair<Candles, int> item in basket)
            {
                Candles candle = item.Key;
                UserCache.Basket.Add(candle, item.Value);
                ListViewBasket.Items.Add(new BasketItem(candle, item.Value));
                Utils.Utils._summaryInformation.AddCount(item.Value);
                Utils.Utils._summaryInformation.AddPrice((double)candle.Price * item.Value);
            }
        }
        private async Task CatalogInit()
        {
            List<Candles> candles = await DBManager.GetCandles();
            candles.ForEach(candle =>
            {
                ListViewCatalog.Items.Add(new Custom.Catalog.ListItem(candle.Name, candle.Description, candle));
            });
        }

        // Profile
        private async void ButtonProfileSetAvatar_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Utils.Utils.GetImageWindowsDialog();
            if (image == null)
            {
                return;
            }
            ImageBrushProfileAvatar.ImageSource = image;
            await DBManager.SetAvatarUser(Utils.Utils.ImageToBinary(image));
        }
        private async void ButtonProfileRemoveAvatar_Click(object sender, RoutedEventArgs e)
        {
            if (ImageBrushProfileAvatar.ImageSource == Utils.Utils._defaultAvatar)
            {
                MessageBox.Show("У вас нету фотографии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ImageBrushProfileAvatar.ImageSource = Utils.Utils._defaultAvatar;
            await DBManager.RemoveAvatarUser();
        }
        private void ButtonProfileChangePhone_Click(object sender, RoutedEventArgs e)
        {
            new UI.Dialogs.Profile.ChangePhoneWindow().ShowDialog();
        }

        // Items Management
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

        // Employees Management
        private async Task ReloadWindowManagementEmployeesList()
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
                SetPagesEmployeesList(await DBManager.GetEmployeesCount());
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

                await DBManager.ChangeRoleById(user.Id, "Пользователь");

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
        private async void ButtonManagementEmployeesPageBack_Click(object sender, RoutedEventArgs e)
        {
            _employeesListCurrentPage -= 1;
            if (_employeesListCurrentPage <= 1)
            {
                ButtonManagementEmployeesPageBack.IsEnabled = false;
            }

            ButtonManagementEmployeesPageNext.IsEnabled = true;
            TextBoxManagementEmployeesPage.Text = _employeesListCurrentPage.ToString();
            await ReloadWindowManagementEmployeesList();
        }
        private async void ButtonManagementEmployeesPageNext_Click(object sender, RoutedEventArgs e)
        {
            _employeesListCurrentPage += 1;
            if (_employeesListCurrentPage >= _employeesListTotalPages)
            {
                ButtonManagementEmployeesPageNext.IsEnabled = false;
            }

            ButtonManagementEmployeesPageBack.IsEnabled = true;
            TextBoxManagementEmployeesPage.Text = _employeesListCurrentPage.ToString();
            await ReloadWindowManagementEmployeesList();
        }
        private async void ButtonManagementEmployeesSearch_Click(object sender, RoutedEventArgs e)
        {
            _isSearchEmployeesList = true;
            ProgressBarManagementEmployeesList.Visibility = Visibility.Visible;
            DataGridManagementEmployeesList.Visibility = Visibility.Collapsed;
            string search = TextBoxManagementEmployeesSearch.Text;
            if (search.Length == 0)
            {
                _isSearchEmployeesList = false;
                await ReloadWindowManagementEmployeesList();
                return;
            }

            ButtonManagementEmployeesSearch.IsEnabled = false;
            _employeesList.Clear();
            _employeesList = await DBManager.FindEmployees(search);
            await ReloadWindowManagementEmployeesList();
            ButtonManagementEmployeesSearch.IsEnabled = true;
        }
        private async void ComboBoxManagementEmployeesRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                Users user = grid.Tag as Users;
                await DBManager.ChangeRoleById(user.Id, comboBox.SelectedItem.ToString());
                await ReloadWindowManagementEmployeesList();
            }
        }
        private async void TextBoxManagementEmployeesPage_KeyDown(object sender, KeyEventArgs e)
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

                await ReloadWindowManagementEmployeesList();
            }
        }
        private async void DataGridManagementEmployeesListRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ReloadWindowManagementEmployeesList();
        }
        private void ButtonManagementUsersListBlock_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Users user = button.Tag as Users;
                string format_name = $"{user.Last_Name} {user.First_Name} {user.Middle_Name}";
                new UI.Custom.Users.ManagementUsersBlockWindow(format_name, user.Id).Show();
            }
        }

        // Users Management
        public async Task ReloadWindowManagementUsersList()
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
                SetPagesUsersList(await DBManager.GetUsersCount());
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
        private async void ButtonManagementUsersListPageBack_Click(object sender, RoutedEventArgs e)
        {
            _usersListCurrentPage -= 1;
            await ReloadWindowManagementUsersList();
        }
        private async void ButtonManagementUsersListPageNext_Click(object sender, RoutedEventArgs e)
        {
            _usersListCurrentPage += 1;
            await ReloadWindowManagementUsersList();
        }
        private async void ButtonManagementUsersSearch_Click(object sender, RoutedEventArgs e)
        {
            _isSearchUsersList = true;
            ProgressBarManagementUsersList.Visibility = Visibility.Visible;
            DataGridManagementUsersList.Visibility = Visibility.Collapsed;
            string search = TextBoxManagementUsersSearch.Text;
            if (search.Length == 0)
            {
                _isSearchUsersList = false;
                await ReloadWindowManagementUsersList();
                return;
            }

            ButtonManagementUsersSearch.IsEnabled = false;
            _usersList.Clear();
            _usersList = await DBManager.FindUsers(search);
            await ReloadWindowManagementUsersList();
            ButtonManagementUsersSearch.IsEnabled = true;
        }
        private async void ComboBoxManagementUsersListSetRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                Users user = grid.Tag as Users;
                await DBManager.ChangeRoleById(user.Id, comboBox.SelectedItem.ToString());
                await ReloadWindowManagementUsersList();
            }
        }
        private async void TextBoxManagementUsersPage_KeyDown(object sender, KeyEventArgs e)
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

                await ReloadWindowManagementUsersList();
            }
        }
        private async void DataGridManagementUsersListMenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ReloadWindowManagementUsersList();
        }

        // Addresses Management
        private async Task ReloadWindowManagementAddressesList()
        {
            ProgressBarManagementAddressesList.Visibility = Visibility.Visible;
            DataGridManagementAddressesList.Visibility = Visibility.Collapsed;

            DataGridManagementAddressesList.Items.Clear();
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
        }
        private async Task ReloadComboBoxSummaryInformationAddress()
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
        }
        private async void ButtonManagementAddressesListRemove_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            await DBManager.RemoveAddress((int)button.Tag);
            await ReloadWindowManagementAddressesList();
            await ReloadComboBoxSummaryInformationAddress();
        }
        private async void ButtonManagementAddressesListAdd_Click(object sender, RoutedEventArgs e)
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

            Order_Address result = await DBManager.AddAddresses(address);
            Utils.Utils._addresses.Add(result);
            await ReloadWindowManagementAddressesList();
            await ReloadComboBoxSummaryInformationAddress();
            TextBoxManagementAddressesListAdd.Text = String.Empty;
            MessageBox.Show("Вы добавили новый адрес!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private async void DataGridManagementAddressesListMenuRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ReloadWindowManagementAddressesList();
        }
        private async void ButtonManagementAddressesListSearch_Click(object sender, RoutedEventArgs e)
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

            await ReloadWindowManagementAddressesList();
            ButtonManagementAddressesListSearch.IsEnabled = true;
        }

        // Orders Profile
        private async Task ReloadOrdersList()
        {
            if (DataGridOrdersList.Items.Count != 0)
            {
                DataGridOrdersList.Items.Clear();
            }

            List<Orders> orders = await DBManager.GetOrders(UserCache._id);
            orders.ForEach(o =>
            {
                DataGridOrdersList.Items.Add(new Custom.Orders.OrderList(o));
            });
        }
        private async void DatGridOrdersListRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ReloadOrdersList();
        }
        private void ButtonOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            new Task(async () =>
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    Button button = (Button)sender;
                    if (button.Tag is Orders order)
                    {
                        MessageBoxResult result = MessageBox.Show("Вы действительно хотите отменить заказа?", "Подтверждение",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.No)
                        {
                            return;
                        }


                        await DBManager.ChangeOrderStatus(order.Id, "Отменён");
                        await ReloadOrdersList();
                        MessageBox.Show($"Вы отменили заказ товара \"{order.Candles_Order.Candles.Name}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                });
            }).Start();
        }

        // Orders Management
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
        private async Task ReloadWindowManagementOrdersList()
        {
            ProgressBarManagementOrdersList.Visibility = Visibility.Visible;
            DataGridManagementOrdersList.Visibility = Visibility.Collapsed;

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
        }
        private async void DataGridManagementOrdersListRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ReloadWindowManagementOrdersList();
        }
        private async void ComboBoxManagementOrdersStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                Orders order = grid.Tag as Orders;
                await DBManager.ChangeOrderStatus(order.Id, comboBox.SelectedItem.ToString());
                await ReloadWindowManagementOrdersList();
            }
        }
        private async void ButtonManagementOrdersPageBack_Click(object sender, RoutedEventArgs e)
        {
            _ordersListCurrentPage -= 1;
            await ReloadWindowManagementOrdersList();
        }
        private async void ButtonManagementOrdersPageNext_Click(object sender, RoutedEventArgs e)
        {
            _ordersListCurrentPage += 1;
            await ReloadWindowManagementOrdersList();
        }
        private async void TextBoxManagementOrdersPage_KeyDown(object sender, KeyEventArgs e)
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

                await ReloadWindowManagementOrdersList();
            }
        }
        private async void ButtonManagementOrdersSearch_Click(object sender, RoutedEventArgs e)
        {
            _isSearchOrdersList = true;
            ProgressBarManagementOrdersList.Visibility = Visibility.Visible;
            DataGridManagementOrdersList.Visibility = Visibility.Collapsed;
            string search = TextBoxManagementOrdersSearch.Text;
            if (search.Length == 0)
            {
                _isSearchOrdersList = false;
                await ReloadWindowManagementOrdersList();
                return;
            }

            ButtonManagementOrdersSearch.IsEnabled = false;
            _ordersList.Clear();
            _ordersList = await DBManager.FindOrders(search);
            await ReloadWindowManagementOrdersList();
            ButtonManagementOrdersSearch.IsEnabled = true;
        }
    }
}
