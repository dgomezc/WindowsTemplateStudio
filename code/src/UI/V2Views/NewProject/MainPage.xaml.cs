﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Templates.UI.V2ViewModels.Common;
using Microsoft.Templates.UI.V2ViewModels.NewProject;

namespace Microsoft.Templates.UI.V2Views.NewProject
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            DataContext = MainViewModel.Instance;
            InitializeComponent();
            V2Services.NavigationService.InitializeSecondaryFrame(stepFrame, new ProjectTypePage());
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                V2Services.OrderingService.Initialize(listView);
            }
        }

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && !comboBox.IsDropDownOpen)
            {
                if (e.Key == Key.Left
                    || e.Key == Key.Up
                    || e.Key == Key.Right
                    || e.Key == Key.Down)
                {
                    e.Handled = true;
                }
            }
        }
    }
}