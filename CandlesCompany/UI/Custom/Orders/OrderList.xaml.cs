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
        public int OrderID { get; set; } //номер заказа
        public string FormatOrderID { get; set; } //форматированный номер заказа в DataGrid
        public DateTime DateCreated { get; set; } //дата создания в DataGrid
        public string NameItem { get; set; } //название товара в DataGrid
        public double TotalPrice { get; set; } //общая цена товара если много штук в DataGrid
        public double CountPrice { get; set; } //по штучная цена товара, т.е. сколько стоит одна штука в DataGrid
        public int Count { get; set; } //количество в DataGrid
        public string Status { get; set; } //статус заказа в DataGrid
        public int ProgressBarValue { get; set; } //значение progressbar в DataGrid
        public bool ProgressBarAnimation { get; set; } //вкл/выкл анимацию progressbar в DataGrid
        public string ProgressBarForeground { get; set; } //цвет бегущей полосы в progressbar в DataGrid
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
                case 1: //Новый заказ
                    ProgressBarForeground = "#0096ff";
                    Status = "Новый";
                    ProgressBarAnimation = true;
                    break;
                case 2: //Заказ обрабатывается
                    ProgressBarForeground = "#ffe600";
                    Status = "В работе";
                    ProgressBarAnimation = true;
                    break;
                case 3: //Заказ доставлен
                    ProgressBarForeground = "#ffa000";
                    Status = "Доставлен";
                    ProgressBarAnimation = true;
                    break;
                case 4: //Клиент забрал заказ
                    ProgressBarForeground = "#00ff55";
                    Status = "Забран";
                    ProgressBarAnimation = false;
                    ProgressBarValue = 100;
                    break;
            }
        }
    }
}
