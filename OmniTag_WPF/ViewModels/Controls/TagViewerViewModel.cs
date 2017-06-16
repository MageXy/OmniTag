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
    public class TagViewerViewModel : SearchViewModel<Tag>
    {
        public TagViewerViewModel(ObservableCollection<Tag> tagList) : base(tagList)
        {
            TagFilterMode = TagFilterMode.All;
            AllValues.CollectionChanged += (sender, args) => { ApplyFilter(); };
            HintText = "Search Tags...";
        }

        #region Properties

        public List<Tag> FilteredTags { get; set; }  

        private TagFilterMode _tagFilterMode;
        public TagFilterMode TagFilterMode
        {
            get { return _tagFilterMode; }
            set { PropNotify.SetProperty(ref _tagFilterMode, value, x => ApplyFilter()); }
        }

        #endregion

        #region Methods

        protected override void ApplyFilter()
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
            FilteredTags = AllValues.Where(statusFilter).OrderBy(t => t.Name).ToList();

            SearchedValues = FilteredTags.Where(t => t.Name.ToUpper().Contains(SearchText.ToUpper())).ToList();
        }

        #endregion
    }
}
