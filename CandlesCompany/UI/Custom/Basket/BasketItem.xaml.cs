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
            SetPriceFormat(_count);
            CountCheck();
        }
        private async void ButtonBasketItemCountPlus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = Convert.ToInt32(TextBoxBasketItemCount.Text);
                count += 1;
                TextBoxBasketItemCount.Text = $"{count}";
                _count = count;
                
                SetPriceFormat(_count);
                AddItem();
                await DBManager.UpdateCandlesBasket(Cache.UserCache._id, _candle, _count);
                Cache.UserCache.Basket.Remove(_candle);
                Cache.UserCache.Basket.Add(_candle, count);
                CountCheck();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void ButtonBasketItemCountMinus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = Convert.ToInt32(TextBoxBasketItemCount.Text);
                count -= 1;
                TextBoxBasketItemCount.Text = $"{count}";
                _count = count;
                TakeItem();
                await DBManager.UpdateCandlesBasket(Cache.UserCache._id, _candle, _count);
                Cache.UserCache.Basket.Remove(_candle);
                Cache.UserCache.Basket.Add(_candle, count);
                SetPriceFormat(_count);
                CountCheck();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void ButtonBasketItemRemove_Click(object sender, RoutedEventArgs e)
        {
            await DBManager.RemoveCandlesBasket(Cache.UserCache._id, _candle);
            Cache.UserCache.Basket.Remove(_candle);
            Utils.Utils.ReloadWindowBasket();
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
        private void SetPriceFormat(int count)
        {
            TextBlockBasketItemPrice.Text = $"Цена в рублях: {_candle.Price * count}";
        }
        private async void TextBoxBasketItemCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxBasketItemCount.Text;
                if (!new Regex(@"^[0-9]+$").IsMatch(text))
                {
                    text = "1";
                }

                int count = Convert.ToInt32(text);
                if (count < 1)
                {
                    count = 1;
                }

                if (count > _candle.Count)
                {
                    count = _candle.Count;
                }

                TakeItem(_count);
                _count = count;
                AddItem(_count);
                await DBManager.UpdateCandlesBasket(Cache.UserCache._id, _candle, _count);
                Cache.UserCache.Basket.Remove(_candle);
                Cache.UserCache.Basket.Add(_candle, count);
                SetPriceFormat(_count);
                CountCheck();
            }
        }
        private void AddItem(int count = 1)
        {
            Utils.Utils._summaryInformation.AddCount(count);
            Utils.Utils._summaryInformation.AddPrice((double)_candle.Price * count);
        }
        private void TakeItem(int count = 1)
        {
            Utils.Utils._summaryInformation.TakeCount(count);
            Utils.Utils._summaryInformation.TakePrice((double)_candle.Price * count);
        }
    }
}
