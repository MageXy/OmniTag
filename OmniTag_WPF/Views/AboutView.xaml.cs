﻿using System.Reflection;
using System.Windows;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
	{
		public AboutView()
		{
			InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
			VersionLabel.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
	}
}
