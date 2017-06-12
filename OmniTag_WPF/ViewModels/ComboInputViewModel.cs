using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NCGLib.WPF.Commands;
using NCGLib.WPF.Templates;

namespace OmniTagWPF.ViewModels
{
    class ComboInputViewModel<T> : CloseableViewModel
    {
        public ComboInputViewModel(IEnumerable<T> items, string labelText = "Please select one:", string displayPropertyName = "")
        {
            AvailableItems = items;
            SelectedItem = AvailableItems.FirstOrDefault();
            DisplayPropertyName = displayPropertyName;
            LabelText = labelText;
            UserConfirmed = false;
        }

        #region Properties

        private IEnumerable<T> _availableItems;
        public IEnumerable<T> AvailableItems
        {
            get { return _availableItems; }
            set { PropNotify.SetProperty(ref _availableItems, value); }
        }

        private T _selectedItem;
        public T SelectedItem
        {
            get { return _selectedItem; }
            set { PropNotify.SetProperty(ref _selectedItem, value); }
        }

        private string _displayPropertyName;
        public string DisplayPropertyName
        {
            get { return _displayPropertyName; }
            set { PropNotify.SetProperty(ref _displayPropertyName, value); }
        }

        private string _labelText;
        public string LabelText
        {
            get { return _labelText; }
            set { PropNotify.SetProperty(ref _labelText, value); }
        }

        private bool _userConfirmed;
        public bool UserConfirmed
        {
            get { return _userConfirmed; }
            set { PropNotify.SetProperty(ref _userConfirmed, value); }
        }

        #endregion

        #region Methods

        private void Confirm()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a value.", "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                return;
            }

            UserConfirmed = true;
            RequestCloseView();
        }

        #endregion

        #region Commands

        private ICommand _confirmCommand;
        public ICommand ConfirmCommand
        {
            get { return _confirmCommand ?? (_confirmCommand = new SimpleCommand(Confirm)); }
        }

        #endregion
    }
}
