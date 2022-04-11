using Newtonsoft.Json.Linq;
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
        public JToken _candle { get; set; }
        public int _count { get; set; }
        public BasketItem(JToken candle, int count = 1)
        {
            InitializeComponent();
            _candle = candle;
            _count = count;
            TextBlockBasketItemName.Text = (string)_candle["Name"];
            TextBlockBasketItemDescription.Text = (string)_candle["Description"];
            ImageBasketItemImage.Source = Utils.Utils.BinaryToImage((byte[])_candle["Image"]);
            TextBoxBasketItemCount.Text = $"{_count}";
            SetPriceFormat();
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
                
                SetPriceFormat();
                AddItem();
                await Api.UpdateCandlesBasket(Cache.UserCache._id, (int)_candle["Id"], _count);
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
                await Api.UpdateCandlesBasket(Cache.UserCache._id, (int)_candle["Id"], _count);
                Cache.UserCache.Basket.Remove(_candle);
                Cache.UserCache.Basket.Add(_candle, count);
                SetPriceFormat();
                CountCheck();
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void ButtonBasketItemRemove_Click(object sender, RoutedEventArgs e)
        {
            await Api.RemoveCandlesBasket(Cache.UserCache._id, (int)_candle["Id"]);
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
                if (count >= (int)_candle["Count"])
                {
                    ButtonBasketItemCountPlus.IsEnabled = false;
                    TextBoxBasketItemCount.Text = _candle["Count"].ToString();
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
        private void SetPriceFormat()
        {            
            TextBlockBasketItemPrice.Text = $"Цена в рублях: {(double)_candle["Price"] * _count}";
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

                if (count > (int)_candle["Count"])
                {
                    count = (int)_candle["Count"];
                }

                TakeItem(_count);
                _count = count;
                AddItem(_count);
                await Api.UpdateCandlesBasket(Cache.UserCache._id, (int)_candle["Id"], _count);
                Cache.UserCache.Basket.Remove(_candle);
                Cache.UserCache.Basket.Add(_candle, count);
                SetPriceFormat();
                CountCheck();
            }
        }
        private void AddItem(int count = 1)
        {
            Utils.Utils._summaryInformation.AddCount(count);
            Utils.Utils._summaryInformation.AddPrice((double)_candle["Price"] * count);
        }
        private void TakeItem(int count = 1)
        {
            Utils.Utils._summaryInformation.TakeCount(count);
            Utils.Utils._summaryInformation.TakePrice((double)_candle["Price"] * count);
        }
    }
}
