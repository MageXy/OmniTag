using System.Windows;
using System.Windows.Controls;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for TableMarkdownView.xaml
    /// </summary>
    public partial class TableMarkdownView : CenteredView
    {
        public TableMarkdownView()
        {
            InitializeComponent();
        }

        protected override void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            base.OnWindowLoaded(sender, e);

            var cellInfo = new DataGridCellInfo(TableData.Items[0], TableData.Columns[0]);
            TableData.SelectedCells.Clear();
            TableData.SelectedCells.Add(cellInfo);
            TableData.CurrentCell = cellInfo;
            TableData.BeginEdit();
        }
    }
}
