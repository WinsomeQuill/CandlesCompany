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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CandlesCompany.UI.Custom.Orders
{
    /// <summary>
    /// Логика взаимодействия для OrderList.xaml
    /// </summary>
    public partial class OrderList : UserControl
    {
        public int OrderID { get; set; }
        public string FormatOrderID { get; set; }
        public DateTime DateCreated { get; set; }
        public string NameItem { get; set; }
        public double TotalPrice { get; set; }
        public double CountPrice { get; set; }
        public int Count { get; set; }
        public string Status { get; set; }
        public int ProgressBarValue { get; set; }
        public bool ProgressBarAnimation { get; set; }
        public string ProgressBarForeground { get; set; }
        public OrderList(int order_id, DateTime dateCreated, string name, double totalPrice, int count, int orderStatusId)
        {
            InitializeComponent();
            OrderID = order_id;
            DateCreated = dateCreated;
            NameItem = name;
            TotalPrice = totalPrice;
            CountPrice = totalPrice / count;
            Count = count;
            FormatOrderID = $"№{OrderID}";

            switch (orderStatusId)
            {
                case 1:
                    ProgressBarForeground = "#0096ff";
                    Status = "Новый";
                    ProgressBarAnimation = true;
                    break;
                case 2:
                    ProgressBarForeground = "#ffe600";
                    Status = "В работе";
                    ProgressBarAnimation = true;
                    break;
                case 3:
                    ProgressBarForeground = "#ffa000";
                    Status = "Доставлен";
                    ProgressBarAnimation = true;
                    break;
                case 4:
                    ProgressBarForeground = "#00ff55";
                    Status = "Забран";
                    ProgressBarAnimation = false;
                    ProgressBarValue = 100;
                    break;
            }
        }
    }
}
