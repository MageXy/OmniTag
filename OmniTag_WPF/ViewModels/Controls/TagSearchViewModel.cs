using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NCGLib;
using OmniTag.Models;
using OmniTagWPF.Utility;

namespace OmniTagWPF.ViewModels.Controls
{
    public class TagSearchViewModel : SearchViewModel<Tag>
    {
        public TagSearchViewModel(ObservableCollection<Tag> tagList) : base(tagList)
        {
            TagFilterMode = TagFilterMode.All;
            HintText = "Search Tags...";
            EnableEnterFunc = searchText => !String.IsNullOrWhiteSpace(searchText);
        }

        #region Properties

        public List<Tag> FilteredTags { get; set; }

        private TagFilterMode _tagFilterMode;
        public TagFilterMode TagFilterMode
        {
            get { return _tagFilterMode; }
            set { PropNotify.SetProperty(ref _tagFilterMode, value, x => ApplyFilter()); }
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
            get { return EnableEnterFunc(FullSearchText); }
        }

        #endregion

        #region Methods

        protected override void ApplyFilter()
        {
            if (IsUpdatingDisplayText)
                return;

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
            FilteredTags = AllValues.Where(statusFilter).OrderBy(t => t.Name).ToList();

            SearchedValues = FilteredTags.Where(Filter).ToList();
        }

        #endregion

        #region Commands

        private ICommand _enterCommand;
        public ICommand EnterCommand
        {
            get { return _enterCommand; }
            set { PropNotify.SetProperty(ref _enterCommand, value); }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get { return _searchCommand; }
            set { PropNotify.SetProperty(ref _searchCommand, value); }
        }

        private ICommand _clickSelectCommand;
        public ICommand ClickSelectCommand
        {
            get { return _clickSelectCommand; }
            set { PropNotify.SetProperty(ref _clickSelectCommand, value); }
        }

        #endregion
    }
}
