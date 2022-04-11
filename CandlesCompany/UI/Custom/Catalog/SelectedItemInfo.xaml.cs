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
using Newtonsoft.Json.Linq;

namespace CandlesCompany.UI.Custom.Catalog
{
    /// <summary>
    /// Логика взаимодействия для SelectedItemInfo.xaml
    /// </summary>
    public partial class SelectedItemInfo : UserControl
    {
        public JToken _candle { get; set; }
        public SelectedItemInfo()
        {
            InitializeComponent();
        }
        private async void ButtonCatalogSelectedItemBuy_Click(object sender, RoutedEventArgs e)
        {
            if (_candle == null)
            {
                MessageBox.Show("Произошла ошибка при добавлении товара в корзину!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Utils.Utils.IsInBasket((int)_candle["Id"]))
            {
                MessageBox.Show("Вы уже добавили этот товар в корзину!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await Utils.Utils.AddItemInBasket(_candle);
            ButtonCatalogSelectedItemBuy.IsEnabled = false;
            ButtonCatalogSelectedItemBuy.Content = "В корзине";
        }
    }
}
