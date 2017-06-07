using System.Windows;
using NCGLib.WPF.Templates;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for SimpleInputView.xaml
    /// </summary>
    public partial class SimpleInputView : CloseableView
    {
        public SimpleInputView()
        {
            InitializeComponent();
            Loaded += OnViewLoaded;
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            InputBox.CaretIndex = InputBox.Text.Length;
            InputBox.Focus();
        }
    }
}
