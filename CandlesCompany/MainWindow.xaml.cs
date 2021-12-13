using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                CatalogInit();
            });
        }

        private async void CatalogInit()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DBManager.db.Candles.ToList().ForEach(candle =>
                {
                    ListViewCatalog.Items.Add(new ListItem(candle.Name, candle.Description, $"Price: {candle.Price} руб. | Count: {candle.Count}"));
                });
            });
        }
    }
}
