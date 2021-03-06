﻿using System.Windows.Input;
using NCGLib.Extensions;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : CenteredView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ValidateText(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.IsNumeric())
                e.Handled = true;
        }
    }
}
