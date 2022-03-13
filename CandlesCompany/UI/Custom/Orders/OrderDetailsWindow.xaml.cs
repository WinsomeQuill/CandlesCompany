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

namespace CandlesCompany.UI.Custom.Orders
{
    /// <summary>
    /// Логика взаимодействия для OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow()
        {
            InitializeComponent();
            /*for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.SetValue(Grid.RowProperty, i);
                    orderDetails.SetValue(Grid.ColumnProperty, j);
                    GridOrderDetails.Children.Add(orderDetails);
                }
            }*/
        }
    }
}
