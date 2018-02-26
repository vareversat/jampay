﻿using ApplicationJampay.ViewModel.ViewModel;
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

namespace ApplicationJampay.View.Caissier.PopUp
{
    /// <summary>
    /// Logique d'interaction pour AjouterPlat.xaml
    /// </summary>
    public partial class AjouterPlat : Window
    {
        public AjouterPlat()
        {
            InitializeComponent();
            DataContext = new AjouterPlatViewModel();
        }
    }
}