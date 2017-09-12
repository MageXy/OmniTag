using System.Windows;
using System.Windows.Controls;
using OmniTagWPF.ViewModels.Controls;

namespace OmniTagWPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for TagSearchListView.xaml
    /// </summary>
    public partial class TagSearchListView : SearchList
    {
        public TagSearchListView()
        {
            InitializeComponent();
        }

        #region Code-behind Methods

        //private void EnterButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    SearchListView.Focus();
        //}

        private void DoubleClickListItem(object sender, RoutedEventArgs e)
        {
            var list = sender as ListView;
            if (list?.SelectedItem == null)
                return;
            
            var vm = DataContext as TagSearchViewModel;
            vm?.ClickSelectCommand?.Execute(list.SelectedItem);
        }

        #endregion
    }
}
