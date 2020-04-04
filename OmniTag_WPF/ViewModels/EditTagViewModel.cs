using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NCGLib;
using NCGLib.Extensions;
using NCGLib.WPF.Commands;
using NCGLib.WPF.Utility;
using OmniTag.Models;
using OmniTagWPF.ViewModels.Base;
using OmniTagWPF.ViewModels.Controls;
using OmniTagWPF.Views;

namespace OmniTagWPF.ViewModels
{
    class EditTagViewModel : DataChangeViewModel
    {
        public EditTagViewModel()
        {
            AddedTags = new List<Tag>();
            DeletedTags = new List<Tag>();
        }

        #region Properties

        #region Main ViewModel Properties
        private ObservableCollection<Tag> AllTags { get; set; }
        private List<Tag> AddedTags { get; set; }
        private List<Tag> DeletedTags { get; set; }

        private Tag _selectedTag;
        public Tag SelectedTag
        {
            get { return _selectedTag; }
            set { SetProperty(ref _selectedTag, value, OnSelectedTagChanging, OnSelectedTagChanged); }
        }

        private TagSearchViewModel _tagSearchDataContext;
        public TagSearchViewModel TagSearchDataContext
        {
            get { return _tagSearchDataContext; }
            set { SetProperty(ref _tagSearchDataContext, value); }
        }

        private bool _changesMade;
        public bool ChangesMade
        {
            get { return _changesMade; }
            set { SetProperty(ref _changesMade, value); }
        }
        #endregion

        #region Cached Tag Properties
        private string _selectedTagName;
        public string SelectedTagName
        {
            get { return _selectedTagName; }
            set
            {
                SetProperty(ref _selectedTagName, value,
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
                SetProperty(ref _selectedTagDescription, value,
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
                SetProperty(ref _isSelectedTagVerified, value,
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
            set { SetProperty(ref _isSelectedTagManuallyVerified, value); }
        }

        private DateTime? _selectedTagLastUpdatedTime;
        public DateTime? SelectedTagLastUpdatedTime
        {
            get { return _selectedTagLastUpdatedTime; }
            set { SetProperty(ref _selectedTagLastUpdatedTime, value); }
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

        [DependsOnProperty(nameof(SelectedTag))]
        public string OmniCount
        {
            get
            {
                if (SelectedTag == null)
                    return String.Empty;
                return $"{SelectedTag.Omnis.Count} Omnis.";
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
                    return true;
            }
        }
        #endregion

        #endregion

        #region Methods

        public override void LoadData()
        {
            AllTags = new ObservableCollection<Tag>(Context.Tags.Where(t => t.DateDeleted == null).OrderBy(t => t.Name).ToList());
            SelectedTag = null;
            
            TagSearchDataContext = new TagSearchViewModel(AllTags)
            {
                EnterCommand = AddNewTagCommand,
                SearchCommand = SearchTagCommand,
                EnterText = "Add New",
                DisplayMember = nameof(Tag.Name),
                ShowEnterButton = true,
                ShowStatusFilter = true,
                EnableEnterFunc = searchText =>
                {
                    if (String.IsNullOrWhiteSpace(searchText))
                        return false;
                    if (TagSearchDataContext.AllValues.Any(t => String.Equals(t.Name, searchText, StringComparison.InvariantCultureIgnoreCase)))
                        return false;
                    return true;
                },
                Filter = s => s.Name.ToUpper().Contains(TagSearchDataContext.SearchText.ToUpper())
            };

            TagSearchDataContext.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TagSearchDataContext.SelectedValue))
                {
                    SelectedTag = TagSearchDataContext.SelectedValue;
                }
            };
        }

        private void VerifyTag()
        {
            IsSelectedTagVerified = true;
            IsSelectedTagManuallyVerified = true;
            ChangesMade = true;
        }

        private void SearchTag()
        {
            if (TagSearchDataContext.FullSearchText.IsEmpty())
                return;

            if (!AllTags.Any(t => t.Name.EqualsIgnoreCase(TagSearchDataContext.FullSearchText)))
                AddNewTag();
            else
            {
                SelectedTag = AllTags.Single(t => t.Name == TagSearchDataContext.FullSearchText);
                TagSearchDataContext.FullSearchText = String.Empty;
            }
        }

        private void ViewAssociatedOmnis()
        {
            if (SelectedTag == null)
                return;

            var vm = new AssociatedOmnisViewModel(SelectedTag);
            var view = ViewFactory.CreateViewWithDataContext<AssociatedOmnisView>(vm);
            view.ShowDialog();
        }

        #region Add/Delete/Save Tag Methods
        private void AddNewTag()
        {
            if (TagSearchDataContext.FullSearchText.Length > 20)
            {
                MessageBox.Show("Tag name cannot be longer than 20 characters (including space).", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            // Check if this tag has been deleted before, then prompt the user to see if we should restore or replace it.
            var newTag = DeletedTags.SingleOrDefault(t => t.Name == TagSearchDataContext.FullSearchText);
            if (newTag == null)
                newTag = Context.Tags.SingleOrDefault(t => t.DateDeleted != null && t.Name == TagSearchDataContext.FullSearchText);

            if (newTag != null)
            {
                var result = 
                    MessageBox.Show($"The tag [{newTag.Name}] was previously deleted. You can either restore this tag and all Omni associations, " +
                                "or replace the deleted tag with a brand new one.\n\nWhen this tag is undeleted, do you want to restore " +
                                "Omni associations?", "Confirm Restore", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Cancel)
                    return;
                else if (result == MessageBoxResult.No)
                {
                    newTag.Omnis.Clear();
                    Context.Tags.Remove(newTag);
                    newTag = null;
                }
                DeletedTags.Remove(newTag);
            }

            if (newTag == null)
            {
                newTag = new Tag
                {
                    Name = TagSearchDataContext.FullSearchText,
                };
                AddedTags.Add(newTag);
            }

            newTag.DateDeleted = null;
            newTag.LastModifiedDate = DateTime.Now;
            newTag.IsVerified = true;
            newTag.ManuallyVerified = true;
            
            AllTags.Add(newTag);
            TagSearchDataContext.FullSearchText = String.Empty;
            SelectedTag = newTag;
            ChangesMade = true;
        }

        private void DeleteSelectedTag()
        {
            var result = MessageBox.Show($"Are you sure you want to delete the [{SelectedTag.Name}] tag? It will be disassociated from any Omnis.",
                                         $"Delete Tag [{SelectedTag.Name}]",
                                         MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            if (!AddedTags.Remove(SelectedTag))
                DeletedTags.Add(SelectedTag);
            SelectedTag.DateDeleted = DateTime.Now;
            AllTags.Remove(SelectedTag);
            SelectedTag = null;
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
            OnDataChanged();
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

        private ICommand _viewAssociatedOmnisCommand;
        public ICommand ViewAssociatedOmnisCommand
        {
            get { return _viewAssociatedOmnisCommand ?? (_viewAssociatedOmnisCommand = new SimpleCommand(ViewAssociatedOmnis)); }
        }

        #endregion
    }
}
