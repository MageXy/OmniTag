using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using NCGLib;
using NCGLib.WPF.Commands;
using OmniTag.Models;
using OmniTagWPF.Utility;
using OmniTagWPF.ViewModels.Base;

namespace OmniTagWPF.ViewModels
{
    public class EditTagViewModel : BaseViewModel
    {
        public EditTagViewModel()
        {
            AddedTags = new List<Tag>();
            DeletedTags = new List<Tag>();
            TagFilterMode = TagFilterMode.All;
        }

        #region Properties

        #region Main ViewModel Properties
        private List<Tag> AllTags { get; set; }
        private List<Tag> AddedTags { get; set; }
        private List<Tag> DeletedTags { get; set; }

        private List<Tag> _allFilteredTags;
        public List<Tag> AllFilteredTags
        {
            get { return _allFilteredTags; }
            set { PropNotify.SetProperty(ref _allFilteredTags, value); }
        }

        private List<Tag> _searchedFilteredTags;
        public List<Tag> SearchedFilteredTags
        {
            get { return _searchedFilteredTags; }
            set { PropNotify.SetProperty(ref _searchedFilteredTags, value); }
        }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get { return _selectedTag; }
            set { PropNotify.SetProperty(ref _selectedTag, value, OnSelectedTagChanging, OnSelectedTagChanged); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { PropNotify.SetProperty(ref _searchText, value, OnSearchTextChanged); }
        }

        private string _suggestedText;
        public string SuggestedText
        {
            get { return _suggestedText; }
            set { PropNotify.SetProperty(ref _suggestedText, value); }
        }

        private TagFilterMode _tagFilterMode;
        public TagFilterMode TagFilterMode
        {
            get { return _tagFilterMode; }
            set { PropNotify.SetProperty(ref _tagFilterMode, value, x => ApplyFilter()); }
        }

        private bool _changesMade;
        public bool ChangesMade
        {
            get { return _changesMade; }
            set { PropNotify.SetProperty(ref _changesMade, value); }
        }
        #endregion

        #region Cached Tag Properties
        private string _selectedTagName;
        public string SelectedTagName
        {
            get { return _selectedTagName; }
            set
            {
                PropNotify.SetProperty(ref _selectedTagName, value,
                    s =>
                    {
                        if ((SelectedTag != null) && (s != SelectedTag.Name))
                            ChangesMade = true;
                    });
            }
        }

        private string _selectedTagDescription;
        public string SelectedTagDescription
        {
            get { return _selectedTagDescription; }
            set
            {
                PropNotify.SetProperty(ref _selectedTagDescription, value,
                s =>
                {
                    if ((SelectedTag != null) && (s != SelectedTag.Description))
                        ChangesMade = true;
                });
            }
        }

        private bool? _isSelectedTagVerified;
        public bool? IsSelectedTagVerified
        {
            get { return _isSelectedTagVerified; }
            set
            {
                PropNotify.SetProperty(ref _isSelectedTagVerified, value,
                s =>
                {
                    if ((SelectedTag != null) && (s != SelectedTag.IsVerified))
                        ChangesMade = true;
                });
            }
        }

        private bool? _isSelectedTagManuallyVerified;
        public bool? IsSelectedTagManuallyVerified
        {
            get { return _isSelectedTagManuallyVerified; }
            set { PropNotify.SetProperty(ref _isSelectedTagManuallyVerified, value); }
        }

        private DateTime? _selectedTagLastUpdatedTime;
        public DateTime? SelectedTagLastUpdatedTime
        {
            get { return _selectedTagLastUpdatedTime; }
            set { PropNotify.SetProperty(ref _selectedTagLastUpdatedTime, value); }
        }

        [DependsOnProperty("SelectedTagLastUpdatedTime")]
        public string SelectedTagLastUpdated
        {
            get
            {
                return SelectedTagLastUpdatedTime == null
                    ? ""
                    : ((DateTime)SelectedTagLastUpdatedTime).ToString("MMMM d, yyyy - hh:mm:ss tt");
            }
        }
        #endregion

        #region Calculated Properties

        private string _fullSearchText;
        public string FullSearchText
        {
            get { return _fullSearchText; }
            set { PropNotify.SetProperty(ref _fullSearchText, value); }
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

        [DependsOnProperty(nameof(SelectedTag))]
        public bool CanSelectedTagBeDeleted
        {
            get { return (SelectedTag != null); }
        }

        [DependsOnProperty(nameof(IsSelectedTagVerified))]
        [DependsOnProperty(nameof(IsSelectedTagManuallyVerified))]
        public string VerifiedImage
        {
            get
            {
                if (IsSelectedTagVerified == null)
                    return null;

                if (IsSelectedTagVerified == false)
                    return Images.Images.ExclamationMark;
                else
                {
                    if (IsSelectedTagManuallyVerified == true)
                        return Images.Images.CheckMark;
                    else
                        return Images.Images.ExclamationMark;
                }
            }
        }

        [DependsOnProperty(nameof(IsSelectedTagVerified))]
        [DependsOnProperty(nameof(IsSelectedTagManuallyVerified))]
        public bool IsVerifiedButtonEnabled
        {
            get
            {
                if (IsSelectedTagVerified == null)
                    return false;
                else if (IsSelectedTagVerified == false)
                    return true;
                else
                {
                    if (IsSelectedTagManuallyVerified == true)
                        return false;
                    else return true;
                }
            }
        }

        [DependsOnProperty(nameof(IsSelectedTagVerified))]
        [DependsOnProperty(nameof(IsSelectedTagManuallyVerified))]
        public bool IsVerifiedLabelVisible
        {
            get
            {
                if (IsSelectedTagManuallyVerified == null)
                    return false;
                else if (IsSelectedTagManuallyVerified == true)
                    return false;
                else
                {
                    return true;
                    //if (IsSelectedTagVerified == false)
                    //    return true;
                    //else
                    //    return false;
                }
            }
        }
        #endregion

        #endregion

        #region Methods

        public override void LoadData()
        {
            AllTags = Context.Tags.Where(t => t.DateDeleted == null).OrderBy(t => t.Name).ToList();
            SearchedFilteredTags = AllTags;
            AllFilteredTags = AllTags;
            SelectedTag = null;
            SearchText = String.Empty;
        }

        //public override void LoadData()
        //{
        //    All = Context.Tags.Where(t => t.DateDeleted == null).OrderBy(t => t.Name).ToList();
        //    SearchedFilteredTags = All;
        //    AllFilteredTags = All;
        //    SelectedTag = null;
        //    SearchText = String.Empty;
        //}

        private void VerifyTag()
        {
            IsSelectedTagVerified = true;
            IsSelectedTagManuallyVerified = true;
            ChangesMade = true;
        }

        private void SearchTag()
        {
            if (CanNewTagBeAdded)
                AddNewTag();
            else
            {
                SelectedTag = AllTags.Single(t => t.Name == FullSearchText);
                FullSearchText = String.Empty;
            }
        }

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
            AllFilteredTags = AllTags.Where(statusFilter).OrderBy(t => t.Name).ToList();

            SearchedFilteredTags = AllFilteredTags.Where(t => t.Name.ToLowerInvariant().Contains(SearchText.ToLowerInvariant())).ToList();
        }

        #region Add/Delete/Save Tag Methods
        private void AddNewTag()
        {
            if (SearchText.Length > 20)
            {
                MessageBox.Show("Tag name cannot be longer than 20 characters (including space).", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            var newTag = new Tag
            {
                DateCreated = DateTime.Now,
                DateDeleted = null,
                LastModifiedDate = DateTime.Now,
                Name = SearchText,
                IsVerified = true,
                ManuallyVerified = true
            };
            AllTags.Add(newTag);
            AddedTags.Add(newTag);
            SearchText = String.Empty;
            SelectedTag = newTag;
            ChangesMade = true;
        }

        private void DeleteSelectedTag()
        {
            var result = MessageBox.Show(String.Format("Are you sure you want to delete the [{0}] tag?", SelectedTag.Name),
                                         String.Format("Delete Tag [{0}]", SelectedTag.Name),
                                         MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            if (!AddedTags.Remove(SelectedTag))
                DeletedTags.Add(SelectedTag);
            SelectedTag.DateDeleted = DateTime.Now;
            AllTags.Remove(SelectedTag);
            ApplyFilter();
            OnPropertyChanged(nameof(SearchText));
            ChangesMade = true;
        }

        private void SaveTagInfo()
        {
            if (!ChangesMade)
                return;

            if ((SelectedTag != null) && (SelectedTag.Name != SelectedTagName))
            {
                SelectedTag.Name = SelectedTagName;
                SelectedTag.LastModifiedDate = DateTime.Now;
            }
            if ((SelectedTag != null) && (SelectedTag.Description != SelectedTagDescription))
            {
                SelectedTag.Description = SelectedTagDescription;
                SelectedTag.LastModifiedDate = DateTime.Now;
            }
            if ((SelectedTag != null) && 
                ((SelectedTag.IsVerified != IsSelectedTagVerified) || (SelectedTag.ManuallyVerified != IsSelectedTagManuallyVerified)))
            {
                SelectedTag.IsVerified = IsSelectedTagVerified ?? false;
                SelectedTag.ManuallyVerified = IsSelectedTagManuallyVerified ?? false;
                SelectedTag.LastModifiedDate = DateTime.Now;
            }

            foreach (var tag in AddedTags)
                tag.LastModifiedDate = DateTime.Now;
            foreach (var tag in DeletedTags)
                tag.LastModifiedDate = DateTime.Now;

            // "Hard save" all changes made to all tags into the database. 
            Context.Tags.AddRange(AddedTags);
            Context.SaveChanges();
            SelectedTagLastUpdatedTime = DateTime.Now;
            AddedTags.Clear();
            DeletedTags.Clear();
            ChangesMade = false;
        }
        #endregion

        #region OnChanging/OnChanged Methods

        private void OnSelectedTagChanging(Tag newTag)
        {
            if (SelectedTag == null)
                return;

            if (SelectedTagName.Length > 20)
            {
                MessageBox.Show("Tag name cannot be longer than 20 characters (including space). Tag name will not be changed.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            // "Soft save" the current tag (save the properties but don't add to DB)
            SelectedTag.Name = SelectedTagName;
            SelectedTag.Description = SelectedTagDescription;
            SelectedTag.IsVerified = IsSelectedTagVerified ?? false;
            SelectedTag.ManuallyVerified = IsSelectedTagManuallyVerified ?? false;
            SelectedTag.LastModifiedDate = SelectedTagLastUpdatedTime ?? SelectedTag.LastModifiedDate;
        }

        private void OnSelectedTagChanged(Tag newTag)
        {
            if (newTag == null)
            {
                SelectedTagName = null;
                SelectedTagDescription = null;
                IsSelectedTagVerified = null;
                IsSelectedTagManuallyVerified = null;
                SelectedTagLastUpdatedTime = null;
            }
            else
            {
                SelectedTagName = newTag.Name;
                SelectedTagDescription = newTag.Description;
                IsSelectedTagVerified = newTag.IsVerified;
                IsSelectedTagManuallyVerified = newTag.ManuallyVerified;
                SelectedTagLastUpdatedTime = newTag.LastModifiedDate;
            }
        }

        private void OnSearchTextChanged(string newText)
        {
            //if ((SelectedTag != null) && (String.Equals(newText, SelectedTag.Name, StringComparison.InvariantCultureIgnoreCase)))
            //    return;

            //var newTag =
            //    SearchedFilteredTags.SingleOrDefault(
            //        t => String.Equals(newText, t.Name, StringComparison.InvariantCultureIgnoreCase));
            //if (newTag != null)
            //    SelectedTag = newTag;

            ApplyFilter();
        }

        #endregion

        #endregion

        #region Commands

        private ICommand _addNewTagCommand;
        public ICommand AddNewTagCommand
        {
            get { return _addNewTagCommand ?? (_addNewTagCommand = new SimpleCommand(AddNewTag)); }
        }

        private ICommand _deleteSelectedTagCommand;
        public ICommand DeleteSelectedTagCommand
        {
            get { return _deleteSelectedTagCommand ?? (_deleteSelectedTagCommand = new SimpleCommand(DeleteSelectedTag)); }
        }

        private ICommand _saveTagsCommand;
        public ICommand SaveTagsCommand
        {
            get { return _saveTagsCommand ?? (_saveTagsCommand = new SimpleCommand(SaveTagInfo)); }
        }

        private ICommand _verifyTagCommand;
        public ICommand VerifyTagCommand
        {
            get { return _verifyTagCommand ?? (_verifyTagCommand = new SimpleCommand(VerifyTag)); }
        }

        private ICommand _searchTagCommand;
        public ICommand SearchTagCommand
        {
            get { return _searchTagCommand ?? (_searchTagCommand = new SimpleCommand(SearchTag)); }
        }

        #endregion
    }
}
