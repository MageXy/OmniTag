namespace OmniTagWPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SearchListView : SearchList
    {
        public SearchListView()
        {
            InitializeComponent();
        }

        //protected void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var textBox = (TextBox)SearchBox.Template.FindName("PART_EditableTextBox", SearchBox);

        //    string searchText = String.Empty;
        //    if (textBox.SelectionLength == 0)
        //        searchText = textBox.Text;
        //    else
        //    if (textBox.SelectionLength > 0)
        //    {
        //        searchText = textBox.Text.Substring(0, textBox.SelectionStart);
        //    }
        //    SearchTextAttachedProperty.SetSearchText(SearchBox, searchText);
        //}

        //protected void OnClearSearchText(object sender, RoutedEventArgs e)
        //{
        //    var textBox = (TextBox)SearchBox.Template.FindName("PART_EditableTextBox", SearchBox);
        //    textBox.Text = String.Empty;
        //}

        //protected void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (SearchView.SelectedItem != null)
        //        SearchView.ScrollIntoView(e.AddedItems[0]);
        //}
    }
}
