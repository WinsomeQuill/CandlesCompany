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
        private SelectedItemInfo _selectediteminfo { get; set; }
        public ListItem(string name, string description, Candles candle, SelectedItemInfo selectedItemInfo)
        {
            InitializeComponent();
            TextBlockItemName.Text = name;
            TextBlockItemDescription.Text = description;
            _candle = candle;
            _selectediteminfo = selectedItemInfo;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _selectediteminfo.TextBlockCatalogSelectedItemTitle.Text = TextBlockItemName.Text;
            _selectediteminfo.TextBlockCatalogSelectedItemDescription.Text = TextBlockItemDescription.Text;
            _selectediteminfo.TextBlockCatalogSelectedItemPrice.Text = $"Цена в рублях: {_candle.Price}\nКоличество: {_candle.Count}";
            _selectediteminfo.ImageCatalogSelectedItem.Source = Utils.Utils.BinaryToImage(_candle.Image);
            _selectediteminfo.ImageCatalogSelectedItem.Width = _selectediteminfo.ImageCatalogSelectedItem.Height = 300;
        }
    }
}
