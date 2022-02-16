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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CandlesCompany.UI.Custom
{
    /// <summary>
    /// Логика взаимодействия для BasketItem.xaml
    /// </summary>
    public partial class BasketItem : UserControl
    {
        private Candles _candle { get; set; }
        public BasketItem(Candles candles)
        {
            InitializeComponent();
            _candle = candles;
            TextBlockBasketItemName.Text = _candle.Name;
            TextBlockBasketItemDescription.Text = _candle.Description;
            TextBlockBasketItemPrice.Text = $"Цена в рублях: {_candle.Price}";
            ImageBasketItemImage.Source = Utils.Utils.BinaryToImage(_candle.Image);
            TextBoxBasketItemCount.Text = "1";
        }
        private void ButtonBasketItemCountPlus_Click(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(TextBoxBasketItemCount.Text);
            count += 1;
            TextBoxBasketItemCount.Text = $"{count}";

            if (count >= _candle.Count)
            {
                ButtonBasketItemCountPlus.IsEnabled = false;
            }
            else
            {
                ButtonBasketItemCountMinus.IsEnabled = true;
            }
        }
        private void ButtonBasketItemCountMinus_Click(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(TextBoxBasketItemCount.Text);
            count -= 1;
            TextBoxBasketItemCount.Text = $"{count}";

            if (count <= 1)
            {
                ButtonBasketItemCountMinus.IsEnabled = false;
            }
            else
            {
                ButtonBasketItemCountPlus.IsEnabled = true;
            }
        }

        private void ButtonBasketItemRemove_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate()
            {
                Dispatcher.Invoke(delegate ()
                {
                    DBManager.RemoveCandlesBasket(Cache.UserCache._id, _candle.Id);
                    Cache.UserCache.Basket.Remove(_candle);
                });
            }).Start();
        }
    }
}
