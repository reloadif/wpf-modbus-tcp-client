﻿using PLCModbusClient.ViewModels;
using System.Windows;

namespace PLCModbusClient.Views
{
    public partial class MainPage : Window
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}