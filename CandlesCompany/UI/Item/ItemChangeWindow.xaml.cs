﻿using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace CandlesCompany.UI.Item
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
        private async void Init()
        {
            if (ComboBoxItemChangeSelectItem.Items.Count != 0)
            {
                ComboBoxItemChangeSelectItem.Items.Clear();
            }

            JObject result = await Api.GetCandles();
            result["Result"].ToList().ForEach(x =>
            {
                ComboBoxItemChangeSelectItem.Items.Add(new ComboBoxItem { Content = (string)x["Name"], Tag = x });
            });

            ComboBoxItemChangeSelectItem.SelectedIndex = 0;
            if (ComboBoxItemChangeSelectItem.Items.Count <= 0)
            {
                ButtonItemChangeSave.IsEnabled = false;
            }
            UpdateInfo();

            if (ComboBoxItemChangeType.Items.Count != 0)
            {
                ComboBoxItemChangeType.Items.Clear();
            }

            JObject types = await Api.GetTypeCandles();
            types["Result"].ToList().ForEach(c =>
            {
                ComboBoxItemChangeType.Items.Add(new ComboBoxItem { Content = (string)c["Name"], Tag = c });
            });

            ComboBoxItemChangeType.SelectedIndex = 0;
            if (ComboBoxItemChangeType.Items.Count <= 0)
            {
                ButtonItemChangeSave.IsEnabled = false;
            }
        }
        private void UpdateInfo()
        {
            ComboBoxItem item = ComboBoxItemChangeSelectItem.SelectedItem as ComboBoxItem;
            if (item == null)
            {
                return;
            }
            JToken candle = item.Tag as JToken;

            TextBoxItemChangeCount.Text = candle["Count"].ToString();
            TextBoxItemChangeName.Text = candle["Name"].ToString();
            TextBoxItemChangePrice.Text = candle["Price"].ToString();
            TextBoxItemChangeDescription.Text = candle["Description"].ToString();

            if (candle["Image"] != null)
            {
                ImageItemChangePreview.Source = Utils.Utils.BinaryToImage((byte[])candle["Image"]);
                return;
            }

            ImageItemChangePreview.Source = Utils.Utils._defaultImage;
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
        private async void ButtonItemChangeSave_Click(object sender, RoutedEventArgs e)
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

            if (ImageItemChangePreview.Source == null)
            {
                MessageBox.Show("Вы не загрузили изображение товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ComboBoxItem item = ComboBoxItemChangeSelectItem.SelectedItem as ComboBoxItem;
            JToken candle = item.Tag as JToken;

            item = ComboBoxItemChangeType.SelectedItem as ComboBoxItem;
            JToken type_Candle = item.Tag as JToken;

            int id = (int)candle["Id"];
            int count = Convert.ToInt32(TextBoxItemChangeCount.Text);
            double price = Convert.ToDouble(TextBoxItemChangePrice.Text);

            if (price <= 0)
            {
                MessageBox.Show("Цена не может быть меньше или ровна 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = TextBoxItemChangeName.Text;
            string description = TextBoxItemChangeDescription.Text;
            byte[] image = Utils.Utils.ImageToBinary(ImageItemChangePreview);

            if (name.Length < 3)
            {
                MessageBox.Show("Слишком короткое название товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (description.Length < 3)
            {
                MessageBox.Show("Слишком короткое описание товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (name.Length > 30)
            {
                MessageBox.Show("Слишком длинное название товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (description.Length > 300)
            {
                MessageBox.Show("Слишком длинное описание товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await Api.UpdateItem(id, (int)type_Candle["Id"], name, description, count, price, image);
            MessageBox.Show($"Вы обновили товар \"{candle["Name"]}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            Init();
        }
    }
}
