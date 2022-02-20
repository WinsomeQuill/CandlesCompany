using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CandlesCompany.UI.Custom.Basket
{
    /// <summary>
    /// Логика взаимодействия для BasketItem.xaml
    /// </summary>
    public partial class BasketItem : UserControl
    {
        public Candles _candle { get; set; }
        public int _count { get; set; }
        public BasketItem(Candles candles, int count = 1)
        {
            InitializeComponent();
            _candle = candles;
            TextBlockBasketItemName.Text = _candle.Name;
            TextBlockBasketItemDescription.Text = _candle.Description;
            ImageBasketItemImage.Source = Utils.Utils.BinaryToImage(_candle.Image);
            TextBoxBasketItemCount.Text = $"{count}";
            _count = count;
            SetPrice(_count);
            CountCheck();
        }
        private void ButtonBasketItemCountPlus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = Convert.ToInt32(TextBoxBasketItemCount.Text);
                count += 1;
                TextBoxBasketItemCount.Text = $"{count}";
                _count = count;
                Utils.Utils._summaryInformation.AddCount(1);
                Utils.Utils._summaryInformation.AddPrice((double)_candle.Price);
                SetPrice(_count);
                DBManager.UpdateCandlesBasket(Cache.UserCache._id, _candle, _count);
                CountCheck();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ButtonBasketItemCountMinus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = Convert.ToInt32(TextBoxBasketItemCount.Text);
                count -= 1;
                TextBoxBasketItemCount.Text = $"{count}";
                _count = count;
                Utils.Utils._summaryInformation.TakeCount(1);
                Utils.Utils._summaryInformation.TakePrice((double)_candle.Price);
                DBManager.UpdateCandlesBasket(Cache.UserCache._id, _candle, _count);
                SetPrice(_count);
                CountCheck();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ButtonBasketItemRemove_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    DBManager.RemoveCandlesBasket(Cache.UserCache._id, _candle);
                    Utils.Utils.ReloadWindowBasket();
                });
            }).Start();
        }
        private void CountCheck()
        {
            int count = 1;
            try
            {
                count = Convert.ToInt32(TextBoxBasketItemCount.Text);
            }
            catch (FormatException)
            {
                TextBoxBasketItemCount.Text = "1";
                count = 1;
            }
            finally
            {
                if (count >= _candle.Count)
                {
                    ButtonBasketItemCountPlus.IsEnabled = false;
                    TextBoxBasketItemCount.Text = _candle.Count.ToString();
                }
                else
                {
                    ButtonBasketItemCountPlus.IsEnabled = true;
                }

                if (count <= 1)
                {
                    ButtonBasketItemCountMinus.IsEnabled = false;
                    TextBoxBasketItemCount.Text = "1";
                }
                else
                {
                    ButtonBasketItemCountMinus.IsEnabled = true;
                }
            }
        }
        private void TextBoxBasketItemCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CountCheck();
        }
        private void SetPrice(int count)
        {
            TextBlockBasketItemPrice.Text = $"Цена в рублях: {_candle.Price * count}";
        }
    }
}
