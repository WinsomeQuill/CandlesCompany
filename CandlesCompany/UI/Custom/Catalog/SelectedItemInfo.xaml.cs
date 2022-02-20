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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CandlesCompany.UI.Custom.Basket;

namespace CandlesCompany.UI.Custom.Catalog
{
    /// <summary>
    /// Логика взаимодействия для SelectedItemInfo.xaml
    /// </summary>
    public partial class SelectedItemInfo : UserControl
    {
        private MainWindow _mainWindow { get; set; }
        public Candles _candle { get; set; }
        public SelectedItemInfo(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        private void ButtonCatalogSelectedItemBuy_Click(object sender, RoutedEventArgs e)
        {
            if (_candle == null)
            {
                MessageBox.Show("Произошла ошибка при добавлении товара в корзину!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Cache.UserCache.Basket.ContainsKey(_candle))
            {
                MessageBox.Show("Вы уже добавили этот товар в корзину!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _mainWindow.ListViewBasket.Items.Add(new BasketItem(_candle));
            DBManager.AddCandlesBasket(Cache.UserCache._id, _candle);
            Utils.Utils._summaryInformation.AddCount(1);
            Utils.Utils._summaryInformation.AddPrice((double)_candle.Price);
            ButtonCatalogSelectedItemBuy.IsEnabled = false;
            ButtonCatalogSelectedItemBuy.Content = "В корзине";
        }
    }
}
