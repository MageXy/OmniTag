using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using NCGLib.WPF.Templates;
using OmniTag.Models;
using OmniTagWPF.ViewModels;
using OmniTagWPF.Views.Controls;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for EditTagView.xaml
    /// </summary>
    public partial class EditTagView : NCGLibView
    {
        public EditTagView()
        {
            InitializeComponent();
        }

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
            var vm = DataContext as EditTagViewModel;
            if (vm == null)
                return;

            //var textBox = sender as TextBox;
            //if (textBox == null)
            //    return;

            var comboBox = sender as ComboBox;
            if (comboBox == null)
                return;
            var textBox = (TextBox)comboBox.Template.FindName("PART_EditableTextBox", comboBox);

            string searchText;
            if (textBox.SelectionLength == 0)
                searchText = textBox.Text;
            else
                searchText = textBox.Text.Substring(0, textBox.SelectionStart);

            //vm.SearchText = searchText;
            SearchTextAttachedProperty.SetSearchText(comboBox, searchText);
        }

        private void OnSearchTextKeyPressed(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Up || e.Key == Key.Down)
            //    e.Handled = true;
        }
    }

    class VerifiedLabelTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orig = value as bool?;
            if ((orig ?? false) == false)
                return Boolean.FalseString;
            return Boolean.TrueString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class TagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orig = value as IEnumerable<Tag>;

            return orig?.Select(t => t.Name).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
