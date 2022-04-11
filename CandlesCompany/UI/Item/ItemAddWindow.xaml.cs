using Microsoft.Win32;
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
    /// Логика взаимодействия для ItemAddWindow.xaml
    /// </summary>
    public partial class ItemAddWindow : Window
    {
        public ItemAddWindow()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            new Thread(delegate ()
            {
                Dispatcher.Invoke(async () =>
                {
                    JObject result = await Api.GetTypeCandles();
                    result["Result"].ToList().ForEach(c =>
                    {
                        ComboBoxItemAddType.Items.Add(new ComboBoxItem { Content = (string)c["Name"], Tag = c });
                    });
                    ComboBoxItemAddType.SelectedIndex = 0;
                });
            }).Start();
        }
        private void ButtonItemAddSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "(*.png, *.jpg)|*.png;*.jpg";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                string Path = openFileDialog1.FileName;
                ImageItemAddPreview.Source = new BitmapImage(new Uri(Path));
            }
        }
        private void ButtonItemAddSave_Click(object sender, RoutedEventArgs e)
        {
            new Thread(async () =>
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    if (!new Regex("^[0-9]+$").IsMatch(TextBoxItemAddCount.Text))
                    {
                        MessageBox.Show("Вы ввели неверный формат в \"Количество\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!new Regex(@"^[0-9]+\,[0-9]+$").IsMatch(TextBoxItemAddPrice.Text))
                    {
                        MessageBox.Show("Вы ввели неверный формат в \"Цена\"!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int count = Convert.ToInt32(TextBoxItemAddCount.Text);
                    double price = Convert.ToDouble(TextBoxItemAddPrice.Text);

                    if (price <= 0)
                    {
                        MessageBox.Show("Цена не может быть меньше или ровна 0!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (ImageItemAddPreview.Source == null)
                    {
                        MessageBox.Show("Вы не загрузили изображение товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    ComboBoxItem item = ComboBoxItemAddType.SelectedItem as ComboBoxItem;
                    JToken type_Candle = item.Tag as JToken;

                    string name = TextBoxItemAddName.Text;
                    string description = TextBoxItemAddDescription.Text;
                    byte[] image = Utils.Utils.ImageToBinary(ImageItemAddPreview);

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


                    await Api.AddItem((int)type_Candle["Id"], name, description, count, price, image);
                    MessageBox.Show($"Вы добавили товар \"{name}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }).Start();
        }
    }
}
