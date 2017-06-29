using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using OmniTagWPF.ViewModels;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

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

        private void OnShortcutEnteredInWebBrowser(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                var vm = DataContext as MainOmniViewModel;
                if (vm == null)
                    return;

                if (e.Key == Key.N)
                    vm.EditOmniCommand.Execute(null);
                else if (e.Key == Key.E)
                    vm.EditOmniCommand.Execute(vm.SelectedOmni);
                else if (e.Key == Key.Delete)
                    vm.DeleteOmniCommand.Execute(null);
                else if (e.Key == Key.T)
                    vm.EditTagsCommand.Execute(null);
            }

            e.Handled = true;
        }
    }
}
