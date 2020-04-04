using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NCGLib.Extensions;
using NCGLib.WPF.Templates.ViewModels;

namespace OmniTagWPF.ViewModels.Controls
{
    public class SearchViewModel<T> : SimpleBaseViewModel
    {
        public SearchViewModel(ObservableCollection<T> values)
        {
            SearchText = String.Empty;
            Filter = x => true;
            AllValues = values;
            HintText = "Search...";
            ApplyFilterOnSearchTextChanged = true;
            UpdateSearchTextWhenSelectionChanges = false;

            AllValues.CollectionChanged += (s, e) => ApplyFilter();
        }

        #region Properties

        protected bool IsUpdatingDisplayText { get; set; }

        private ObservableCollection<T> _allValues;
        public ObservableCollection<T> AllValues
        {
            get { return _allValues; }
            set { SetProperty(ref _allValues, value, onChangedAction:x => ApplyFilter()); }
        }

        private Func<T, bool> _filter;
        public Func<T, bool> Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); }
        }

        private string _hintText;
        public string HintText
        {
            get { return _hintText; }
            set { SetProperty(ref _hintText, value); }
        }

        private string _displayMember;
        public string DisplayMember
        {
            get { return _displayMember; }
            set { SetProperty(ref _displayMember, value); }
        }

        private string _enterText;
        public string EnterText
        {
            get { return _enterText; }
            set { SetProperty(ref _enterText, value); }
        }

        private List<T> _searchedValues;
        public List<T> SearchedValues
        {
            get { return _searchedValues; }
            set { SetProperty(ref _searchedValues, value); }
        }

        private string _fullSearchText;
        public string FullSearchText
        {
            get { return _fullSearchText; }
            set { SetProperty(ref _fullSearchText, value, forceUpdate:true); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value, onChangedAction: x =>
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
            set
            {
                SetProperty(ref _selectedValue, value, onChangedAction: x =>
                {
                    if (UpdateSearchTextWhenSelectionChanges)
                    {
                        if (SelectedValue == null)
                            return;

                        IsUpdatingDisplayText = true;
                        if (DisplayMember.IsEmpty())
                            FullSearchText = SelectedValue.ToString() ?? String.Empty;
                        else
                        {
                            var prop = SelectedValue.GetType().GetProperty(DisplayMember);
                            FullSearchText = prop.GetValue(SelectedValue).ToString();
                        }
                        
                        IsUpdatingDisplayText = false;
                    }
                });
            }
        }

        private bool _showEnterButton;
        public bool ShowEnterButton
        {
            get { return _showEnterButton; }
            set { SetProperty(ref _showEnterButton, value); }
        }

        private bool _showStatusFilter;
        public bool ShowStatusFilter
        {
            get { return _showStatusFilter; }
            set { SetProperty(ref _showStatusFilter, value); }
        }

        private bool _applyFilterOnSearchTextChanged;
        public bool ApplyFilterOnSearchTextChanged
        {
            get { return _applyFilterOnSearchTextChanged; }
            set { SetProperty(ref _applyFilterOnSearchTextChanged, value); }
        }

        private bool _updateSearchTextWhenSelectionChanges;
        public bool UpdateSearchTextWhenSelectionChanges
        {
            get { return _updateSearchTextWhenSelectionChanges; }
            set { SetProperty(ref _updateSearchTextWhenSelectionChanges, value); }
        }

        #endregion

        #region Methods

        protected virtual void ApplyFilter()
        {
            if (!IsUpdatingDisplayText)
                SearchedValues = AllValues.Where(Filter).ToList();
        }



        #endregion

        #region Commands

        

        #endregion
    }
}
