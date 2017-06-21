using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OmniTagWPF.Views.Controls
{
    public abstract class SearchList : UserControl
    {
        protected KeylessComboBox SearchBox;
        
        protected void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var comboBox = sender as KeylessComboBox;
            if (comboBox == null)
                return;
            var textBox = (TextBox)comboBox.Template.FindName("PART_EditableTextBox", comboBox);

            string searchText = String.Empty;
            if (textBox.SelectionLength == 0)
                searchText = textBox.Text;
            else
            if (textBox.SelectionLength > 0)
            {
                searchText = textBox.Text.Substring(0, textBox.SelectionStart);
            }
            SearchTextAttachedProperty.SetSearchText(comboBox, searchText);

            SearchBox = comboBox;
        }

        protected void OnClearSearchText(object sender, RoutedEventArgs e)
        {
            if (SearchBox == null)
                return;

            var textBox = (TextBox)SearchBox.Template.FindName("PART_EditableTextBox", SearchBox);
            textBox.Text = String.Empty;
        }

        protected void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null)
                return;

            if (listView.SelectedItem != null)
                listView.ScrollIntoView(e.AddedItems[0]);
        }
    }
}
