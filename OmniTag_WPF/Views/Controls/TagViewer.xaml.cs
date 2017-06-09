using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OmniTagWPF.ViewModels.Controls;

namespace OmniTagWPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for TagViewer.xaml
    /// </summary>
    public partial class TagViewer : UserControl
    {
        public TagViewer()
        {
            InitializeComponent();
        }

        #region Code-behind Methods

        private void AddTagButton_OnClick(object sender, RoutedEventArgs e)
        {
            TagListView.Focus();
        }

        private void TagListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TagListView.SelectedItem != null)
                TagListView.ScrollIntoView(e.AddedItems[0]);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null)
                return;
            var textBox = (TextBox)comboBox.Template.FindName("PART_EditableTextBox", comboBox);

            string searchText;
            if (textBox.SelectionLength == 0)
                searchText = textBox.Text;
            else
                searchText = textBox.Text.Substring(0, textBox.SelectionStart);

            SearchTextAttachedProperty.SetSearchText(comboBox, searchText);
        }

        #endregion
    }
}
