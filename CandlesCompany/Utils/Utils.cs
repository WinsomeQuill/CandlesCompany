using CandlesCompany.UI;
using CandlesCompany.UI.Custom.Basket;
using CandlesCompany.UI.Custom.Catalog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CandlesCompany.Utils
{
    public static class Utils
    {
        public static MainWindow _mainWindow { get; set; }
        public static SelectedItemInfo _selectediteminfo { get; set; }
        public static SummaryInformation _summaryInformation { get; set; }
        public static ListView _listViewBasket { get; set; }
        public static DataGrid _dataGridOrdersList { get; set; }
        public enum OrderStatus
        {
            New,
            InWork,
            InDelivery,
            Completed
        }
        public static byte[] ImageToBinary(Image image)
        {
            MemoryStream stream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)image.Source));
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
                _mainWindow.ListViewBasket.Items.Add(new BasketItem(item.Key, item.Value));
                _summaryInformation.AddCount(item.Value);
                _summaryInformation.AddPrice((double)item.Key.Price);
            }
        }
    }
}
