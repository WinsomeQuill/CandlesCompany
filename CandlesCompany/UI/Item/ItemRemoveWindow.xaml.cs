﻿using Newtonsoft.Json.Linq;
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
using System.Windows.Shapes;

namespace CandlesCompany.UI.Item
{
    /// <summary>
    /// Логика взаимодействия для ItemRemoveWindow.xaml
    /// </summary>
    public partial class ItemRemoveWindow : Window
    {
        public ItemRemoveWindow()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            new Task(async () =>
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    if (ComboBoxItemRemoveSelectItem.Items.Count != 0)
                    {
                        ComboBoxItemRemoveSelectItem.Items.Clear();
                    }

                    JObject candles = await Api.GetCandles();
                    candles["Result"].ToList().ForEach(c =>
                    {
                        ComboBoxItemRemoveSelectItem.Items.Add(new ComboBoxItem { Content = (string)c["Name"], Tag = c });
                    });

                    ComboBoxItemRemoveSelectItem.SelectedIndex = 0;
                    if (ComboBoxItemRemoveSelectItem.Items.Count <= 0)
                    {
                        ButtonItemRemoveSave.IsEnabled = false;
                    }
                });
            }).Start();
        }
        private async void ButtonItemRemoveSave_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = ComboBoxItemRemoveSelectItem.SelectedItem as ComboBoxItem;
            JToken candle = item.Tag as JToken;
            MessageBoxResult result =  MessageBox.Show($"Вы действительно хотите удалить товар \"{candle["Name"]}\"", "Подтверждени",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No) { return; }

            MessageBox.Show($"Вы удалили товар \"{candle["Name"]}\"!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            await Api.RemoveItem((int)candle["Id"]);
            Init();
        }
    }
}
