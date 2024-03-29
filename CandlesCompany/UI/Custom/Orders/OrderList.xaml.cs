﻿using Newtonsoft.Json.Linq;
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
        public string Address { get; set; } //адрес доставки
        public string UserName { get; set; } //ФИО заказчика
        public string UserEmail { get; set; } //Email заказчика
        public BitmapImage UserAvatar { get; set; } //аватарка заказчика
        public int UserID { get; set; } //ID заказчика
        public List<string> OrderStatuses { get; set; }
        public bool ButtonCancelEnabled { get; set; }
        public JToken Order { get; set; }
        public OrderList(JToken order)
        {
            InitializeComponent();
            Order = order;
            OrderID = (int)order["Id"];
            DateCreated = (DateTime)order["Date"];
            NameItem = (string)order["Candle"]["Name"];
            TotalPrice = (double)order["Price"];
            Count = (int)order["Count"];
            CountPrice = TotalPrice / Count;
            FormatOrderID = $"№{OrderID}";
            Address = (string)order["Address"] ?? "Адрес удален";
            Status = (string)order["Status"]["Name"] ?? "Новый";
            ButtonCancelEnabled = true;

            switch (string.IsNullOrEmpty((string)order["Status"]["Name"]) ? 1 : (int)order["Status"]["Id"])
            {
                case 1: //Новый заказ
                    ProgressBarForeground = "#0096ff";
                    ProgressBarAnimation = true;
                    break;
                case 2: //Заказ обрабатывается
                    ProgressBarForeground = "#ffe600";
                    ProgressBarAnimation = true;
                    break;
                case 3: //Заказ в пути
                    ProgressBarForeground = "#ffa000";
                    ProgressBarAnimation = true;
                    break;
                case 4: //Заказ ожидает в пункте выдачи
                    ProgressBarForeground = "#00bd87";
                    ProgressBarAnimation = true;
                    break;
                case 5: //Заказ забран
                    ProgressBarForeground = "#5bc746";
                    ProgressBarAnimation = ButtonCancelEnabled = false;
                    ProgressBarValue = 100;
                    break;
                case 6: //Заказ отменен
                    ProgressBarForeground = "#c43f3f";
                    ProgressBarAnimation = ButtonCancelEnabled = false;
                    ProgressBarValue = 100;
                    break;
            }
        }
        public OrderList(JToken order, JToken user, List<string> order_Statuses)
        {
            InitializeComponent();
            Order = order;
            OrderID = (int)order["Id"];
            DateCreated = (DateTime)order["Date"];
            NameItem = (string)order["Candle"]["Name"];
            TotalPrice = (double)order["Price"];
            Count = (int)order["Count"];
            CountPrice = TotalPrice / Count;
            FormatOrderID = $"№{OrderID}";
            Address = (string)order["Address"] ?? "Адрес удален";
            Status = (string)order["Status"]["Name"] ?? "Новый";
            UserID = (int)user["Id"];
            OrderStatuses = order_Statuses;
            UserEmail = (string)user["Email"];
            UserName = $"{user["Last_Name"]} {user["First_Name"]} {user["Middle_Name"]}";
            _ = string.IsNullOrEmpty((string)user["Avatar"]) ? UserAvatar = Utils.Utils._defaultAvatar : UserAvatar = Utils.Utils.BinaryToImage((byte[])user["Avatar"]);
            ButtonCancelEnabled = true;


            switch (string.IsNullOrEmpty((string)order["Status"]["Name"]) ? 1 : (int)order["Status"]["Id"])
            {
                case 1: //Новый заказ
                    ProgressBarForeground = "#0096ff";
                    ProgressBarAnimation = true;
                    break;
                case 2: //Заказ обрабатывается
                    ProgressBarForeground = "#ffe600";
                    ProgressBarAnimation = true;
                    break;
                case 3: //Заказ в пути
                    ProgressBarForeground = "#ffa000";
                    ProgressBarAnimation = true;
                    break;
                case 4: //Заказ ожидает в пункте выдачи
                    ProgressBarForeground = "#00bd87";
                    ProgressBarAnimation = true;
                    break;
                case 5: //Заказ забран
                    ProgressBarForeground = "#5bc746";
                    ProgressBarAnimation = false;
                    ProgressBarValue = 100;
                    break;
                case 6: //Заказ отменен
                    ProgressBarForeground = "#c43f3f";
                    ProgressBarAnimation = ButtonCancelEnabled = false;
                    ProgressBarValue = 100;
                    break;
            }
        }
    }
}
