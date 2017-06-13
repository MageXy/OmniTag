using System.Windows.Input;
using NCGLib.Extensions;
using NCGLib.WPF.Templates.Views;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for TableMarkdownSizeView.xaml
    /// </summary>
    public partial class TableMarkdownSizeView : CloseableView
    {
        public TableMarkdownSizeView()
        {
            InitializeComponent();
            ColumnTextBox.Focus();
        }

        private void ValidateText(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.IsNumeric())
                e.Handled = true;
        }
    }
}
