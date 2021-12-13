﻿using System;
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

namespace CandlesCompany
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        public SignInWindow()
        {
            InitializeComponent();
        }

        private void ButtonSignInRegistered_Click(object sender, RoutedEventArgs e)
        {
            new SignUpWindow().Show();
            Close();
        }

        private void ButtonSignInJoin_Click(object sender, RoutedEventArgs e)
        {
            string email = TextBoxSignInEmail.Text;
            string pass = PasswordBoxSignIn.Password;
            if (!DBManager.Join(email, pass))
            {
                MessageBox.Show("Error");
                return;
            }

            new MainWindow().Show();
            Close();
        }
    }
}