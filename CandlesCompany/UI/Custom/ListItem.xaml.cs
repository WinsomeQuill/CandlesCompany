using CandlesCompany.UI.Custom;
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

namespace CandlesCompany.UI.Custom
{
    /// <summary>
    /// Логика взаимодействия для ListItem.xaml
    /// </summary>
    public partial class ListItem : UserControl
    {
        public Candles _candle { get; set; }
        public ListItem(string name, string description, Candles candle)
        {
            InitializeComponent();
            TextBlockItemName.Text = name;
            TextBlockItemDescription.Text = description;
            _candle = candle;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Utils.Utils._selectediteminfo.TextBlockCatalogSelectedItemTitle.Text = TextBlockItemName.Text;
            Utils.Utils._selectediteminfo.TextBlockCatalogSelectedItemDescription.Text = TextBlockItemDescription.Text;
            Utils.Utils._selectediteminfo.TextBlockCatalogSelectedItemPrice.Text = $"Цена в рублях: {_candle.Price}\nКоличество: {_candle.Count}";
            Utils.Utils._selectediteminfo.ImageCatalogSelectedItem.Source = Utils.Utils.BinaryToImage(_candle.Image);
            Utils.Utils._selectediteminfo.ImageCatalogSelectedItem.Width = Utils.Utils._selectediteminfo.ImageCatalogSelectedItem.Height = 300;
            Utils.Utils._selectediteminfo._candle = _candle;
            Utils.Utils._selectediteminfo.Visibility = Visibility.Visible;

            if (IsInBasket())
            {
                Utils.Utils._selectediteminfo.ButtonCatalogSelectedItemBuy.IsEnabled = false;
                Utils.Utils._selectediteminfo.ButtonCatalogSelectedItemBuy.Content = "В корзине";
            }
            else
            {
                Utils.Utils._selectediteminfo.ButtonCatalogSelectedItemBuy.IsEnabled = true;
                Utils.Utils._selectediteminfo.ButtonCatalogSelectedItemBuy.Content = "В корзину";
            }
        }
        private bool IsInBasket()
        {
            return Cache.UserCache.Basket.ContainsKey(_candle);
        }
    }
}
