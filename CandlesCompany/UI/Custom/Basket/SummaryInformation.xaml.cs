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

namespace CandlesCompany.UI.Custom.Basket
{
    /// <summary>
    /// Логика взаимодействия для SummaryInformation.xaml
    /// </summary>
    public partial class SummaryInformation : UserControl
    {
        public double _price { get; set; }
        public int _salePercent { get; set; }
        public double _amount { get; set; }
        public SummaryInformation()
        {
            InitializeComponent();
            TextBlockSummaryInformationSale.Text = "Скидка: 0%";
        }
        public void AddPrice(double price)
        {
            _price += price;
            TextBlockSummaryInformationPrice.Text = $"Итоговая цена: {_price} руб.";
        }
        public void TakePrice(double price)
        {
            if (_price < 0)
            {
                _price = 0;
                TextBlockSummaryInformationPrice.Text = $"Итоговая цена: {_price} руб.";
                return;
            }
            _price -= price;
            TextBlockSummaryInformationPrice.Text = $"Итоговая цена: {_price} руб.";
        }
        public void AddCount(int count)
        {
            _amount += count;
            TextBlockSummaryInformationCount.Text = $"Общее количество: {_amount} шт.";
        }
        public void TakeCount(int count)
        {
            if (count < 0)
            {
                _amount = 0;
                TextBlockSummaryInformationCount.Text = $"Общее количество: {_amount} шт.";
                return;
            }
            _amount -= count;
            TextBlockSummaryInformationCount.Text = $"Общее количество: {_amount} шт.";
        }
        public void Reset()
        {
            _price = _amount = 0;
            TextBlockSummaryInformationPrice.Text = $"Итоговая цена: {_price} руб.";
            TextBlockSummaryInformationCount.Text = $"Общее количество: {_amount} шт.";
        }
        private void ButtonSummaryInformationBuy_Click(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    Utils.Utils.BasketToOrders();
                });
            }).Start();
            MessageBox.Show("Ваш заказ упешно создан! Подробнее смотрите в раделе \"Заказы\"!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
