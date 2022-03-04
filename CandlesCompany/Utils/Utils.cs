using Microsoft.Win32;
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
        public static List<Order_Address> _addresses { get; set; }
        public static List<string> _roles { get; set; }
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
            _mainWindow.ListViewBasket.Items.Clear();
            _summaryInformation.Reset();
            foreach (var item in Cache.UserCache.Basket)
            {
                _mainWindow.ListViewBasket.Items.Add(new UI.Custom.Basket.BasketItem(item.Key, item.Value));
                _summaryInformation.AddCount(item.Value);
                _summaryInformation.AddPrice((double)item.Key.Price);
            }
        }
        public async static Task<bool> BasketToOrders()
        {
            if (_summaryInformation._address == null)
            {
                return false;
            }

            foreach (UI.Custom.Basket.BasketItem item in _listViewBasket.Items)
            {
                Orders order = await DBManager.AddOrder(Cache.UserCache._id, item._candle.Id, item._count, (double)item._candle.Price * item._count, _summaryInformation._address);
                _dataGridOrdersList.Items.Add(new UI.Custom.Orders.OrderList(order));
                DBManager.RemoveCandlesBasket(Cache.UserCache._id, item._candle);
            }
            _listViewBasket.Items.Clear();

            return true;
        }
        public static bool IsInBasket(Candles candle)
        {
            return Cache.UserCache.Basket.ContainsKey(candle);
        }
        public static void AddItemInBasket(Candles candle)
        {
            _mainWindow.ListViewBasket.Items.Add(new UI.Custom.Basket.BasketItem(candle));
            DBManager.AddCandlesBasket(Cache.UserCache._id, candle);
            _summaryInformation.AddCount(1);
            _summaryInformation.AddPrice((double)candle.Price);
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
