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
using Newtonsoft.Json.Linq;

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

        private List<JToken> _usersList { get; set; } = new List<JToken>();
        private List<JToken> _employeesList { get; set; } = new List<JToken>();
        private List<JToken> _ordersList { get; set; } = new List<JToken>();

        private bool _isSearchUsersList = false;
        private bool _isSearchEmployeesList = false;
        private bool _isSearchAddressesList = false;
        private bool _isSearchOrdersList = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task Init()
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                Utils.Utils._mainWindow = this;
                Utils.Utils._defaultImage = new BitmapImage(new Uri(@"pack://application:,,,/CandlesCompany;component/Resources/Images/Items/notfound.png"));
                Utils.Utils._listViewBasket = ListViewBasket;
                Utils.Utils._dataGridOrdersList = DataGridOrdersList;
                ProfileInit();
                SelectedItemInit();

                await ReloadOrdersList();
                await CatalogInit();
                await SummaryInformationInit();
                await BasketInit();

                if (UserCache._role != 1 && UserCache._role != 2) // if user not admin/manager
                {
                    TabItemAdmin.Visibility = Visibility.Collapsed;
                }
                else // if user is admin/manager
                {
                    await ReloadWindowManagementEmployeesList();
                    await ReloadWindowManagementUsersList();
                    await ReloadWindowManagementOrdersList();
                    await ReloadWindowManagementAddressesList();
                }
            });
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
            TextBlockProfilePhone.Text = UserCache._phone;
            TextBlockProfileEmail.Text = $"Эл. почта: {UserCache._email}";
            TextBlockProfileRole.Text = $"Должность: {UserCache._roleName}";
            ImageBrushProfileAvatar.ImageSource = UserCache._avatar;
        }
        private async Task BasketInit()
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                JObject result = await Api.GetCandlesBasket(UserCache._id);

                if ((int)result["Count"] == 0)
                {
                    TextBlockBasket.Visibility = Visibility.Visible;
                    ListViewBasket.Visibility = Visibility.Collapsed;
                    return;
                }

                TextBlockBasket.Visibility = Visibility.Collapsed;
                ListViewBasket.Visibility = Visibility.Visible;

                result["Result"].ToList().ForEach(item =>
                {
                    int count = (int)item["Count"];
                    UserCache.Basket.Add(item["Candle"], count);
                    ListViewBasket.Items.Add(new BasketItem(item["Candle"], count));
                    Utils.Utils._summaryInformation.AddCount(count);
                    Utils.Utils._summaryInformation.AddPrice((double)item["Candle"]["Price"] * count);
                });
            });            
        }
        private async Task CatalogInit()
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                JObject result = await Api.GetCandles();
                result["Result"].ToList().ForEach(x =>
                {
                    ListViewCatalog.Items.Add(new Custom.Catalog.ListItem(x));
                });
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
            UserCache._avatar = image;
            await Api.SetAvatarUser(UserCache._id, Utils.Utils.ImageToBinary(image));
        }
        private async void ButtonProfileRemoveAvatar_Click(object sender, RoutedEventArgs e)
        {
            if (ImageBrushProfileAvatar.ImageSource == Utils.Utils._defaultAvatar)
            {
                MessageBox.Show("У вас нету фотографии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ImageBrushProfileAvatar.ImageSource = Utils.Utils._defaultAvatar;
            UserCache._avatar = null;
            await Api.RemoveAvatarUser(UserCache._id);
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
            await App.Current.Dispatcher.Invoke(async () =>
            {
                ProgressBarManagementEmployeesList.Visibility = Visibility.Visible;
                DataGridManagementEmployeesList.Visibility = Visibility.Collapsed;

                DataGridManagementEmployeesList.Items.Clear();
                if (!_isSearchEmployeesList)
                {
                    JObject employees = await Api.GetEmployeesForPage(_employeesListCurrentPage);

                    if ((int)employees["Count"] == 0) return;

                    employees["Result"].ToList().ForEach(x =>
                    {
                        _employeesList.Add(x);
                    });

                    _employeesList.ForEach(user =>
                    {
                        BitmapImage avatar = null;
                        if (string.IsNullOrEmpty((string)user["Avatar"]))
                        {
                            avatar = Utils.Utils._defaultAvatar;
                        }
                        else
                        {
                            avatar = Utils.Utils.BinaryToImage((byte[])user["Avatar"]);
                        }

                        DataGridManagementEmployeesList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                    });
                    JObject result = await Api.GetEmployeesCount();
                    SetPagesEmployeesList((int)result["Result"]);
                }
                else
                {
                    _employeesList.Skip(_listPageSize * (_employeesListCurrentPage - 1)).Take(_listPageSize).ToList().ForEach(user =>
                    {
                        BitmapImage avatar = null;
                        if (string.IsNullOrEmpty((string)user["Avatar"]))
                        {
                            avatar = Utils.Utils._defaultAvatar;
                        }
                        else
                        {
                            avatar = Utils.Utils.BinaryToImage((byte[])user["Avatar"]);
                        }

                        DataGridManagementEmployeesList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                    });
                    SetPagesEmployeesList(_employeesList.Count() - 1);
                }

                ProgressBarManagementEmployeesList.Visibility = Visibility.Collapsed;
                DataGridManagementEmployeesList.Visibility = Visibility.Visible;
            });
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
                JToken user = grid.Tag as JToken;

                MessageBoxResult result = MessageBox.Show($"Вы действительно хотите снять с должности \"{user["Roles"]["Name"]}\" " +
                    $"сотрудника \"{user["Last_Name"]} {user["First_Name"]}\"", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No) { return; }

                MessageBox.Show($"Вы сняли с должности \"{user["Roles"]["Name"]}\" сотрудника \"{user["Last_Name"]} {user["First_Name"]}\"!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await Api.ChangeRoleById((int)user["Id"], "Пользователь");

                DataGridManagementEmployeesList.Items.Clear();

                JObject employees = await Api.GetEmployeesForPage(_employeesListCurrentPage);
                employees["Result"].ToList().ForEach(x =>
                {
                    _employeesList.Add(x);
                });

                _employeesList.ForEach(employee =>
                {
                    BitmapImage avatar = null;
                    if (employee["Avatar"] == null)
                    {
                        avatar = Utils.Utils._defaultAvatar;
                    }
                    else
                    {
                        avatar = Utils.Utils.BinaryToImage((byte[])employee["Avatar"]);
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
            JObject employees = await Api.FindEmployees(search);

            if ((int)employees["Count"] == 0) return;

            employees["Result"].ToList().ForEach(x =>
            {
                _employeesList.Add(x);
            });
            await ReloadWindowManagementEmployeesList();
            ButtonManagementEmployeesSearch.IsEnabled = true;
        }
        private async void ComboBoxManagementEmployeesRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                JToken user = grid.Tag as JToken;
                await Api.ChangeRoleById((int)user["Id"], comboBox.SelectedItem.ToString());
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
                JToken user = button.Tag as JToken;
                string format_name = $"{user["Last_Name"]} {user["First_Name"]} {user["Middle_Name"]}";
                new UI.Custom.Users.ManagementUsersBlockWindow(format_name, (int)user["Id"]).Show();
            }
        }

        // Users Management
        public async Task ReloadWindowManagementUsersList()
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                ProgressBarManagementUsersList.Visibility = Visibility.Visible;
                DataGridManagementUsersList.Visibility = Visibility.Collapsed;

                DataGridManagementUsersList.Items.Clear();
                if (!_isSearchUsersList)
                {
                    JObject users = await Api.GetUsersForPage(_usersListCurrentPage);
                    if ((int)users["Count"] == 0) return;
                    users["Result"].ToList().ForEach(x =>
                    {
                        _usersList.Add(x);
                    });
                    _usersList.ForEach(user =>
                    {
                        BitmapImage avatar = null;
                        if (string.IsNullOrEmpty((string)user["Avatar"]))
                        {
                            avatar = Utils.Utils._defaultAvatar;
                        }
                        else
                        {
                            avatar = Utils.Utils.BinaryToImage((byte[])user["Avatar"]);
                        }

                        DataGridManagementUsersList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                    });
                    JObject result = await Api.GetUsersCount();
                    SetPagesUsersList((int)result["Result"]);
                }
                else
                {
                    _usersList.Skip(_listPageSize * (_usersListCurrentPage - 1)).Take(_listPageSize).ToList().ForEach(user =>
                    {
                        BitmapImage avatar = null;
                        if (string.IsNullOrEmpty((string)user["Avatar"]))
                        {
                            avatar = Utils.Utils._defaultAvatar;
                        }
                        else
                        {
                            avatar = Utils.Utils.BinaryToImage((byte[])user["Avatar"]);
                        }

                        DataGridManagementUsersList.Items.Add(new UI.UsersList(user, avatar, Utils.Utils._roles));
                    });
                    SetPagesUsersList(_usersList.Count() - 1);
                }

                ProgressBarManagementUsersList.Visibility = Visibility.Collapsed;
                DataGridManagementUsersList.Visibility = Visibility.Visible;
            });
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
            JObject result = await Api.FindUsers(search);
            result["Result"].ToList().ForEach(x =>
            {
                _usersList.Add(x);
            });
            await ReloadWindowManagementUsersList();
            ButtonManagementUsersSearch.IsEnabled = true;
        }
        private async void ComboBoxManagementUsersListSetRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                Grid grid = comboBox.Parent as Grid;
                JToken user = grid.Tag as JToken;
                await Api.ChangeRoleById((int)user["Id"], comboBox.SelectedItem.ToString());
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
            await App.Current.Dispatcher.Invoke(async () =>
            {
                ProgressBarManagementAddressesList.Visibility = Visibility.Visible;
                DataGridManagementAddressesList.Visibility = Visibility.Collapsed;
                DataGridManagementAddressesList.Items.Clear();

                if (!_isSearchAddressesList)
                {
                    JObject result = await Api.GetAddresses();
                    result["Result"].ToList().ForEach(x =>
                    {
                        Utils.Utils._addresses.Add(x);
                        DataGridManagementAddressesList.Items.Add(new UI.Custom.Address.AddressList(x));
                    });
                }
                else
                {
                    Utils.Utils._addresses.ForEach(address =>
                    {
                        if (address["Address"].Contains(TextBoxManagementAddressesListSearch.Text))
                        {
                            DataGridManagementAddressesList.Items.Add(new UI.Custom.Address.AddressList(address));
                        }
                    });
                }

                ProgressBarManagementAddressesList.Visibility = Visibility.Collapsed;
                DataGridManagementAddressesList.Visibility = Visibility.Visible;
            });
        }
        private async Task ReloadComboBoxSummaryInformationAddress()
        {
            await App.Current.Dispatcher.Invoke(async () =>
            {
                if (Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Count != 0)
                {
                    Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress.Items.Clear();
                }

                JObject result = await Api.GetAddresses();
                result["Result"].ToList().ForEach(x =>
                {
                    Utils.Utils._addresses.Add(x);
                    Utils.Utils._summaryInformation.ComboBoxSummaryInformationAddress
                    .Items.Add(new ComboBoxItem
                    {
                        Content = (string)x["Address"],
                        Tag = x
                    });
                });
            });
        }
        private async void ButtonManagementAddressesListRemove_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            JToken address = button.Tag as JToken;
            await Api.RemoveAddress((int)address["Id"]);
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

            JObject result = await Api.AddAddresses(address);
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
            await App.Current.Dispatcher.Invoke(async () =>
            {
                if (DataGridOrdersList.Items.Count != 0)
                {
                    DataGridOrdersList.Items.Clear();
                }

                JObject result = await Api.GetOrders(UserCache._id);
                result["Result"].ToList().ForEach(o =>
                {
                    DataGridOrdersList.Items.Add(new Custom.Orders.OrderList(o));
                });
            });
        }
        private async void DatGridOrdersListRefresh_Click(object sender, RoutedEventArgs e)
        {
            await ReloadOrdersList();
        }
        private async void ButtonOrderRemove_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag is JToken order)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите отменить заказа?", "Подтверждение",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }


                await Api.ChangeOrderStatus((int)order["Id"], "Заказ отменён");
                await ReloadOrdersList();
                MessageBox.Show($"Вы отменили заказ товара \"{order["Candle"]["Name"]}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
            await App.Current.Dispatcher.Invoke(async () =>
            {
                ProgressBarManagementOrdersList.Visibility = Visibility.Visible;
                DataGridManagementOrdersList.Visibility = Visibility.Collapsed;

                DataGridManagementOrdersList.Items.Clear();
                List<string> orders_statutes = new List<string>();
                JObject status = await Api.GetStatusList();
                status["Result"].ToList().ForEach(x =>
                {
                    orders_statutes.Add((string)x);
                });
                if (!_isSearchOrdersList)
                {

                    _ordersList.Clear();
                    JObject result = await Api.GetOrdersForPage(_ordersListCurrentPage);
                    _ordersList = result["Result"].ToList();
                    _ordersList.ForEach(o =>
                    {
                        DataGridManagementOrdersList.Items.Add(new UI.Custom.Orders.OrderList(o, o["User"], orders_statutes));
                    });
                    JObject count = await Api.GetOrdersCount();
                    SetPagesOrdersList((int)count["Result"]);
                }
                else
                {
                    _ordersList.Skip(_listPageSize * (_ordersListCurrentPage - 1)).Take(_listPageSize).ToList().ForEach(o =>
                    {
                        DataGridManagementOrdersList.Items.Add(new UI.Custom.Orders.OrderList(o, o["User"], orders_statutes));
                    });
                    SetPagesOrdersList(_ordersList.Count() - 1);
                }

                ProgressBarManagementOrdersList.Visibility = Visibility.Collapsed;
                DataGridManagementOrdersList.Visibility = Visibility.Visible;
                GC.Collect();
            });
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
                JToken order = grid.Tag as JToken;
                await Api.ChangeOrderStatus((int)order["Id"], comboBox.SelectedItem.ToString());
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
            JObject result = await Api.FindOrders(search);
            _ordersList = result["Result"].ToList();
            await ReloadWindowManagementOrdersList();
            ButtonManagementOrdersSearch.IsEnabled = true;
        }
    }
}
