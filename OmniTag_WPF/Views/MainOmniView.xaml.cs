using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for MainOmniView.xaml
    /// </summary>
    public partial class MainOmniView : CenteredView
    {
        public MainOmniView()
        {
            InitializeComponent();
        }

        private void OnWebBrowserLinkClicked(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri == null) // is navigating to string HTML
                return;

            var proc = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = e.Uri.ToString()
            };
            Process.Start(proc);
            e.Cancel = true;
        }

        private void OnClearOmniSearchTextClicked(object sender, RoutedEventArgs e)
        {
            OmniSearchBox.Text = String.Empty;
        }
    }
}
