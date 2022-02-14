using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace CandlesCompany
{
    /// <summary>
    /// Логика взаимодействия для ItemChangeWindow.xaml
    /// </summary>
    public partial class ItemChangeWindow : Window
    {
        public ItemChangeWindow()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    DBManager.GetCandles().ForEach(c =>
                    {
                        ComboBoxItemChangeSelectItem.Items.Add(new ComboBoxItem { Content = c.Name, Tag = c });
                    });
                    ComboBoxItemChangeSelectItem.SelectedIndex = 0;
                    UpdateInfo();
                });
            }).Start();

            new Thread(delegate ()
            {
                Dispatcher.Invoke(delegate ()
                {
                    DBManager.GetTypeCandles().ForEach(c =>
                    {
                        ComboBoxItemChangeType.Items.Add(new ComboBoxItem { Content = c.Name, Tag = c });
                    });
                    ComboBoxItemChangeType.SelectedIndex = 0;
                });
            }).Start();
        }
        private void UpdateInfo()
        {
            ComboBoxItem item = ComboBoxItemChangeSelectItem.SelectedItem as ComboBoxItem;
            Candles candle = item.Tag as Candles;

            TextBoxItemChangeCount.Text = candle.Count.ToString();
            TextBoxItemChangeName.Text = candle.Name.ToString();
            TextBoxItemChangePrice.Text = candle.Price.ToString();
            TextBoxItemDescription.Text = candle.Description.ToString();

            if (candle.Image != null)
            {
                ImageItemChangePreview.Source = Utils.Utils.BinaryToImage(candle.Image);
                return;
            }

            ImageItemChangePreview.Source = new BitmapImage(new Uri(@"pack://application:,,,/CandlesCompany;component/Resources/Images/Items/notfound.png"));
        }
        private void ButtonItemChangeSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "(*.png, *.jpg)|*.png;*.jpg";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                string Path = openFileDialog1.FileName;
                ImageItemChangePreview.Source = new BitmapImage(new Uri(Path));
            }
        }
        private void ComboBoxItemChangeSelectItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateInfo();
        }
        private void ButtonItemChangeSave_Click(object sender, RoutedEventArgs e)
        {
            if (!new Regex("^[0-9]+$").IsMatch(TextBoxItemChangeCount.Text))
            {
                MessageBox.Show("Вы ввели неверный формат в \"Количество\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!new Regex(@"^[0-9]+\,[0-9]+$").IsMatch(TextBoxItemChangePrice.Text))
            {
                MessageBox.Show("Вы ввели неверный формат в \"Цена\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ComboBoxItem item = ComboBoxItemChangeSelectItem.SelectedItem as ComboBoxItem;
            Candles candle = item.Tag as Candles;

            item = ComboBoxItemChangeType.SelectedItem as ComboBoxItem;
            Type_Candle type_Candle = item.Tag as Type_Candle;

            int id = candle.Id;
            int count = Convert.ToInt32(TextBoxItemChangeCount.Text);
            double price = Convert.ToDouble(TextBoxItemChangePrice.Text);

            if(price <= 0)
            {
                MessageBox.Show("Цена не может быть меньше или ровна 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = TextBoxItemChangeName.Text;
            string description = TextBoxItemDescription.Text;
            byte[] image = Utils.Utils.ImageToBinary(ImageItemChangePreview);

            DBManager.UpdateItem(id, type_Candle.Id, name, description, count, price, image);
            MessageBox.Show($"Вы обновили товар \"{candle.Name}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
