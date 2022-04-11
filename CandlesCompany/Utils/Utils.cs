using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CandlesCompany.Utils
{
    public static class Utils
    {
        public static UI.MainWindow _mainWindow { get; set; }
        public static UI.Custom.Catalog.SelectedItemInfo _selectediteminfo { get; set; }
        public static UI.Custom.Basket.SummaryInformation _summaryInformation { get; set; }
        public static ListView _listViewBasket { get; set; }
        public static DataGrid _dataGridOrdersList { get; set; }
        public static BitmapImage _defaultImage { get; set; }
        public static BitmapImage _defaultAvatar { get; set; }
        public static List<JToken> _addresses { get; set; } = new List<JToken>();
        public static List<string> _roles { get; set; } = new List<string>();
        public static byte[] ImageToBinary(Image image)
        {
            MemoryStream stream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image.Source));
            encoder.Save(stream);
            return stream.ToArray();
        }
        public static byte[] ImageToBinary(BitmapImage image)
        {
            MemoryStream stream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
            return stream.ToArray();
        }
        public static BitmapImage BinaryToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        public static void ReloadWindowBasket()
        {
            if (Cache.UserCache.Basket.Count() == 0)
            {
                _mainWindow.ListViewBasket.Visibility = System.Windows.Visibility.Collapsed;
                _mainWindow.TextBlockBasket.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _mainWindow.ListViewBasket.Visibility = System.Windows.Visibility.Visible;
                _mainWindow.TextBlockBasket.Visibility = System.Windows.Visibility.Collapsed;
            }

            _mainWindow.ListViewBasket.Items.Clear();
            _summaryInformation.Reset();
            foreach (var item in Cache.UserCache.Basket)
            {
                _mainWindow.ListViewBasket.Items.Add(new UI.Custom.Basket.BasketItem(item.Key, item.Value));
                _summaryInformation.AddCount(item.Value);
                _summaryInformation.AddPrice((double)item.Key["Candle"]["Price"]);
            }
        }
        public async static Task<bool> BasketToOrders()
        {
            if (_summaryInformation._address == null)
            {
                return false;
            }

            _mainWindow.ListViewBasket.Visibility = System.Windows.Visibility.Collapsed;
            _mainWindow.TextBlockBasket.Visibility = System.Windows.Visibility.Visible;
            foreach (UI.Custom.Basket.BasketItem item in _listViewBasket.Items)
            {
                JObject result = await Api.AddOrder(Cache.UserCache._id, (int)item._candle["Id"], item._count, 
                    (double)item._candle["Price"] * item._count, (int)_summaryInformation._address["Id"]);
                _dataGridOrdersList.Items.Add(new UI.Custom.Orders.OrderList(result["Result"]));
                Cache.UserCache.Basket.Remove(item._candle);
                await Api.RemoveCandlesBasket(Cache.UserCache._id, (int)item._candle["Id"]);
            }
            _listViewBasket.Items.Clear();

            return true;
        }
        public static bool IsInBasket(int id_candle)
        {
            foreach (var item in Cache.UserCache.Basket)
            {
                if ((int)item.Key["Id"] == id_candle)
                {
                    return true;
                }
            }
            return false;
        }
        public async static Task AddItemInBasket(JToken candle)
        {
            _mainWindow.ListViewBasket.Visibility = System.Windows.Visibility.Visible;
            _mainWindow.TextBlockBasket.Visibility = System.Windows.Visibility.Collapsed;
            _mainWindow.ListViewBasket.Items.Add(new UI.Custom.Basket.BasketItem(candle));
            await Api.AddCandlesBasket(Cache.UserCache._id, (int)candle["Id"]);
            Cache.UserCache.Basket.Add(candle, 1);
            _summaryInformation.AddCount(1);
            _summaryInformation.AddPrice((double)candle["Price"]);
        }
        public static BitmapImage GetImageWindowsDialog()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "(*.png, *.jpg)|*.png;*.jpg";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                string Path = openFileDialog1.FileName;
                return new BitmapImage(new Uri(Path));
            }

            return null;
        }  
    }
}
