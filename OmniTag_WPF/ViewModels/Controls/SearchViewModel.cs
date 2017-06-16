using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NCGLib;
using NCGLib.WPF.Templates.ViewModels;

namespace OmniTagWPF.ViewModels.Controls
{
    public class SearchViewModel<T> : SimpleBaseViewModel
    {
        public SearchViewModel(ObservableCollection<T> values)
        {
            SearchText = String.Empty;
            AllValues = values;
            HintText = "Search...";
            ApplyFilterOnSearchTextChanged = true;
            EnableEnterFunc = searchText => !String.IsNullOrWhiteSpace(searchText);
        }

        #region Properties

        private ObservableCollection<T> _allValues;
        public ObservableCollection<T> AllValues
        {
            get { return _allValues; }
            set { PropNotify.SetProperty(ref _allValues, value, x => ApplyFilter()); }
        }

        private string _hintText;
        public string HintText
        {
            get { return _hintText; }
            set { PropNotify.SetProperty(ref _hintText, value); }
        }

        private string _enterText;
        public string EnterText
        {
            get { return _enterText; }
            set { PropNotify.SetProperty(ref _enterText, value); }
        }

        private List<T> _searchedValues;
        public List<T> SearchedValues
        {
            get { return _searchedValues; }
            set { PropNotify.SetProperty(ref _searchedValues, value); }
        }

        private string _fullSearchText;
        public string FullSearchText
        {
            get { return _fullSearchText; }
            set { PropNotify.SetProperty(ref _fullSearchText, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                PropNotify.SetProperty(ref _searchText, value, x =>
                {
                    if (ApplyFilterOnSearchTextChanged)
                        ApplyFilter();
                });
            }
        }

        private T _selectedValue;
        public T SelectedValue
        {
            get { return _selectedValue; }
            set { PropNotify.SetProperty(ref _selectedValue, value); }
        }

        private bool _showEnterButton;
        public bool ShowEnterButton
        {
            get { return _showEnterButton; }
            set { PropNotify.SetProperty(ref _showEnterButton, value); }
        }

        private bool _showStatusFilter;
        public bool ShowStatusFilter
        {
            get { return _showStatusFilter; }
            set { PropNotify.SetProperty(ref _showStatusFilter, value); }
        }

        private bool _applyFilterOnSearchTextChanged;
        public bool ApplyFilterOnSearchTextChanged
        {
            get { return _applyFilterOnSearchTextChanged; }
            set { PropNotify.SetProperty(ref _applyFilterOnSearchTextChanged, value); }
        }

        private Func<string, bool> _enableEnterFunc;
        public Func<string, bool> EnableEnterFunc
        {
            get { return _enableEnterFunc; }
            set { PropNotify.SetProperty(ref _enableEnterFunc, value); }
        }

        [DependsOnProperty(nameof(FullSearchText))]
        public virtual bool CanNewValueBeEntered
        {
            get
            {
                return EnableEnterFunc(FullSearchText);
            }
        }

        #endregion

        #region Methods

        protected virtual void ApplyFilter()
        {
            SearchedValues = AllValues.Where(x => x.ToString().ToUpper().Contains(SearchText.ToUpper())).ToList();
        }

        #endregion

        #region Commands

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
            set { PropNotify.SetProperty(ref _searchCommand, value); }
        }

        private ICommand _enterCommand;
        public ICommand EnterCommand
        {
            get { return _enterCommand; }
            set { PropNotify.SetProperty(ref _enterCommand, value); }
        }

        #endregion
    }
}
