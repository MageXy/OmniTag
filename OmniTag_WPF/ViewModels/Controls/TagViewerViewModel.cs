using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Windows.Input;
using NCGLib;
using NCGLib.WPF.Commands;
using NCGLib.WPF.Templates;
using OmniTag.Models;
using OmniTagWPF.Utility;

namespace OmniTagWPF.ViewModels.Controls
{
    public class TagViewerViewModel : SimpleBaseViewModel
    {
        public TagViewerViewModel(ObservableCollection<Tag> tagList)
        {
            _searchText = String.Empty;
            AllTags = tagList;
            TagFilterMode = TagFilterMode.All;
            AllTags.CollectionChanged += (sender, args) => { ApplyFilter(); };
        }

        #region Properties

        private ObservableCollection<Tag> _allTags;
        public ObservableCollection<Tag> AllTags
        {
            get { return _allTags; }
            set { PropNotify.SetProperty(ref _allTags, value, x => ApplyFilter()); }
        }

        public List<Tag> FilteredTags { get; set; }  

        private List<Tag> _searchedFilteredTags;
        public List<Tag> SearchedFilteredTags
        {
            get { return _searchedFilteredTags; }
            set { PropNotify.SetProperty(ref _searchedFilteredTags, value); }
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
            set { PropNotify.SetProperty(ref _searchText, value, x => ApplyFilter()); }
        }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get { return _selectedTag; }
            set { PropNotify.SetProperty(ref _selectedTag, value); }
        }

        private TagFilterMode _tagFilterMode;
        public TagFilterMode TagFilterMode
        {
            get { return _tagFilterMode; }
            set { PropNotify.SetProperty(ref _tagFilterMode, value, x => ApplyFilter()); }
        }

        private bool _showAddButton;
        public bool ShowAddButton
        {
            get { return _showAddButton; }
            set { PropNotify.SetProperty(ref _showAddButton, value); }
        }

        private bool _showStatusFilter;
        public bool ShowStatusFilter
        {
            get { return _showStatusFilter; }
            set { PropNotify.SetProperty(ref _showStatusFilter, value); }
        }

        [DependsOnProperty(nameof(FullSearchText))]
        public bool CanNewTagBeAdded
        {
            get
            {
                if (String.IsNullOrWhiteSpace(FullSearchText))
                    return false;
                if (AllTags.Any(t => String.Equals(t.Name, FullSearchText, StringComparison.InvariantCultureIgnoreCase)))
                    return false;
                return true;
            }
        }

        #endregion

        #region Methods

        private void ApplyFilter()
        {
            Func<Tag, bool> statusFilter;
            switch (TagFilterMode)
            {
                case TagFilterMode.Verified:
                    statusFilter = t => t.IsVerified && t.ManuallyVerified;
                    break;
                case TagFilterMode.AutoVerified:
                    statusFilter = t => t.IsVerified && !t.ManuallyVerified;
                    break;
                case TagFilterMode.Unverified:
                    statusFilter = t => !t.IsVerified || !t.ManuallyVerified;
                    break;
                case TagFilterMode.All:
                default:
                    statusFilter = t => true;
                    break;
            }
            FilteredTags = AllTags.Where(statusFilter).OrderBy(t => t.Name).ToList();

            SearchedFilteredTags = FilteredTags.Where(t => t.Name.ToUpper().Contains(SearchText.ToUpper())).ToList();
        }

        #endregion

        #region Commands

        private ICommand _searchTagCommand;
        public ICommand SearchTagCommand
        {
            get { return _searchTagCommand; }
            set { PropNotify.SetProperty(ref _searchTagCommand, value); }
        }

        private ICommand _addTagCommand;
        public ICommand AddTagCommand
        {
            get { return _addTagCommand; }
            set { PropNotify.SetProperty(ref _addTagCommand, value); }
        }

        #endregion
    }
}
