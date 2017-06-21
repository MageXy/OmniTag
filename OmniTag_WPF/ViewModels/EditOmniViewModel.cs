using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NCGLib;
using NCGLib.WPF.Commands;
using OmniTag.Models;
using OmniTag.Models.Utility;
using OmniTagWPF.Utility;
using OmniTagWPF.ViewModels.Base;
using OmniTagWPF.ViewModels.Controls;
using NCGLib.Extensions;

namespace OmniTagWPF.ViewModels
{
    class EditOmniViewModel : BaseViewModel
    {
        public EditOmniViewModel()
        {
            CurrentOmni = new Omni
            {
                DateCreated = DateTime.Now,
                LastModifiedDate = DateTime.Now
            };
            _isNewOmni = true;
            Initialize();
        }

        public EditOmniViewModel(Omni currentOmni)
        {
            CurrentOmni = currentOmni;
            _isNewOmni = false;
            Initialize();
        }

        private void Initialize()
        {
            _omniSummary = CurrentOmni.Summary;
            _omniDescription = CurrentOmni.Description;
            _currentOmniLastUpdatedTime = CurrentOmni.LastModifiedDate;

            AddedTags = new List<Tag>();
            DeletedTags = new List<Tag>();
        }

        #region Properties

        private Omni CurrentOmni { get; set; }
        private List<Tag> AddedTags { get; set; }
        private List<Tag> DeletedTags { get; set; }
        private bool _isNewOmni;
        private int _autoVerifyThreshold;
        
        private string _omniSummary;
        public string OmniSummary
        {
            get { return _omniSummary; }
            set
            {
                PropNotify.SetProperty(ref _omniSummary, value,
                    s =>
                    {
                        if ((CurrentOmni != null) && (s != CurrentOmni.Description))
                            ChangesMade = true;
                    });
            }
        }

        private string _omniDescription;
        public string OmniDescription
        {
            get { return _omniDescription; }
            set
            {
                PropNotify.SetProperty(ref _omniDescription, value,
                    s =>
                    {
                        if ((CurrentOmni != null) && (s != CurrentOmni.Description))
                            ChangesMade = true;
                    });
            }
        }

        private DateTime? _currentOmniLastUpdatedTime;
        public DateTime? CurrentOmniLastUpdatedTime
        {
            get { return _currentOmniLastUpdatedTime; }
            set { PropNotify.SetProperty(ref _currentOmniLastUpdatedTime, value); }
        }


        private List<Tag> _omniTags;
        public List<Tag> OmniTags
        {
            get { return _omniTags; }
            set { PropNotify.SetProperty(ref _omniTags, value); }
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

        private string _tagSearchText;
        public string TagSearchText
        {
            get { return _tagSearchText; }
            set { PropNotify.SetProperty(ref _tagSearchText, value); }
        }

        private ObservableCollection<TagButtonViewModel> _imageButtons;
        public ObservableCollection<TagButtonViewModel> ImageButtons
        {
            get { return _imageButtons; }
            set { PropNotify.SetProperty(ref _imageButtons, value); }
        }

        private bool _changesMade;
        public bool ChangesMade
        {
            get { return _changesMade; }
            set { PropNotify.SetProperty(ref _changesMade, value); }
        }

        private string _renderedMarkdownHtml;
        public string RenderedMarkdownHtml
        {
            get { return _renderedMarkdownHtml; }
            set { PropNotify.SetProperty(ref _renderedMarkdownHtml, value); }
        }

        private List<string> _tagNameList;
        public List<string> TagNameList
        {
            get { return _tagNameList; }
            set { PropNotify.SetProperty(ref _tagNameList, value); }
        }

        [DependsOnProperty(nameof(CurrentOmniLastUpdatedTime))]
        public string CurrentOmniLastUpdatedString
        {
            get
            {
                if (CurrentOmniLastUpdatedTime == null)
                    return String.Empty;

                return "Last Updated: " + CurrentOmniLastUpdatedTime.Value.ToString("MMM dd, yyyy hh:mm tt");
            }
        }

        #endregion

        #region Methods

        public override void LoadData()
        {
            OmniTags = CurrentOmni.Tags.ToList();

            var ibvmList = OmniTags.Select(t => new TagButtonViewModel(t));
            ImageButtons = new ObservableCollection<TagButtonViewModel>(ibvmList);

            string avtStr = Context.Settings.SingleOrDefault(s => s.Name == Setting.AutoTagVerificationThreshold)?.Value;

            if (!Int32.TryParse(avtStr, out _autoVerifyThreshold))
                _autoVerifyThreshold = 5;

            TagNameList = Context.Tags.Select(t => t.Name).ToList();
        }

        public void PreviewHtml()
        {
            // convert all newlines to Environment.NewLine when rendering, to avoid mixing newline types.
            RenderedMarkdownHtml = OmniTextRenderer.Render(OmniDescription?.Replace("\r\n", "\n")
                .Replace("\n", Environment.NewLine) ?? String.Empty);
        }

        private void OnSearchTextChanged(string newString)
        {
            if (newString.IsEmpty())
            {
                SuggestedText = String.Empty;
                return;
            }

            var tagName = Context.Tags.Where(t => t.Name.ToLower().StartsWith(SearchText.ToLower()))
                                  .Select(t => t.Name)
                                  .FirstOrDefault();
            if (tagName.IsEmpty())
            {
                SuggestedText = String.Empty;
                return;
            }
            SuggestedText = tagName.Substring(SearchText.Length);
        }

        private void SaveOmni()
        {
            if (!ChangesMade)
                return;

            if (OmniTags.Count == 0)
            {
                var result = MessageBox.Show("This Omni is not associated with any tags. This will prevent it from " +
                             "being listed if any tag filters are chosen during searches.\n\n" +
                             "Are you sure you want to continue " +
                             "without associating a tag?", "Warning", MessageBoxButton.YesNo,
                             MessageBoxImage.Warning, MessageBoxResult.No);
                if (result == MessageBoxResult.No)
                    return;
            }

            if (CurrentOmni.Summary != OmniSummary)
            {
                if (OmniSummary.Length > 100)
                {
                    MessageBox.Show("Omni summary cannot be longer than 100 characters (including spaces). The current " +
                                   $"length of the summary is {OmniSummary.Length} characters.", "Error", MessageBoxButton.OK,
                                   MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
                CurrentOmni.Summary = OmniSummary;
                CurrentOmni.LastModifiedDate = DateTime.Now;
            }
            if (CurrentOmni.Description != OmniDescription)
            {
                CurrentOmni.Description = OmniDescription;
                CurrentOmni.LastModifiedDate = DateTime.Now;
            }

            if (!OmniTags.IsEqualTo(CurrentOmni.Tags))
            {
                CurrentOmni.Tags = OmniTags;
                CurrentOmni.LastModifiedDate = DateTime.Now;
            }

            foreach (var tag in AddedTags)
            {
                Context.Tags.Add(tag);
                CurrentOmni.LastModifiedDate = DateTime.Now;

                if (tag.Omnis.Count >= _autoVerifyThreshold)
                    tag.IsVerified = true;
            }
            AddedTags.Clear();

            foreach (var tag in DeletedTags)
            {
                if ((!tag.ManuallyVerified) && (tag.Omnis.Count < _autoVerifyThreshold))
                    tag.IsVerified = false;

                // If an unverified tag isn't being used, we delete it since the user hasn't marked the tag as important.
                if ((!tag.ManuallyVerified) && (tag.Omnis.Count == 1)) // use 1 here to represent the current tag and no others
                    tag.DateDeleted = DateTime.Now;

                CurrentOmni.LastModifiedDate = DateTime.Now;
            }
            DeletedTags.Clear();

            if (_isNewOmni)
                Context.Omnis.Add(CurrentOmni);

            Context.SaveChanges();

            foreach (var tag in OmniTags)
            {
                if (tag.Omnis.Count >= _autoVerifyThreshold)
                    tag.IsVerified = true;
            }
            Context.SaveChanges();

            ChangesMade = false;
            _isNewOmni = false;
            CurrentOmniLastUpdatedTime = CurrentOmni.LastModifiedDate;
            OnDataChanged();
        }

        private void AddTag()
        {
            if (TagSearchText.IsEmpty())
                return;
            if (TagSearchText.Length > 20)
            {
                MessageBox.Show("Tag name cannot be longer than 20 characters (including space).",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            if (OmniTags.Any(t => t.Name.Equals(TagSearchText, StringComparison.CurrentCultureIgnoreCase)))
            {
                MessageBox.Show($"This Omni is already associated with the \"{TagSearchText}\" tag.",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            var tag = Context.Tags.SingleOrDefault(t => t.Name == TagSearchText);
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = TagSearchText,
                    Description = null,
                    IsVerified = false,
                    DateCreated = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };
                AddedTags.Add(tag);
            }
            tag.DateDeleted = null; // undelete the tag (if it was in the DB but deleted)
            var buttonVm = new TagButtonViewModel(tag);
            OmniTags.Add(tag);
            ImageButtons.Add(buttonVm);
            TagSearchText = "";
            ChangesMade = true;
        }

        private void DeleteTag(object obj)
        {
            var ibvm = obj as TagButtonViewModel;
            if (ibvm == null)
                return;

            OmniTags.Remove(ibvm.CurrentTag);
            ImageButtons.Remove(ibvm);
            if (!AddedTags.Remove(ibvm.CurrentTag))
                DeletedTags.Add(ibvm.CurrentTag);
            ChangesMade = true;
        }

        private void ShowHelp()
        {
            var result = MessageBox.Show("The editor screen makes use of a simplified version of Markdown. Not all " +
                            "commands documented on the official site are available.\n" +
                            "Do you want to open your internet browser to view the official online" +
                            "documentation?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (result != MessageBoxResult.Yes)
                return;

            var proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.FileName = "https://daringfireball.net/projects/markdown/syntax";
            proc.Start();
        }

        private void OnDataChanged()
        {
            var handler = DataChanged;
            if (handler != null)
                handler(this, new OmniDataChangedEventArgs(CurrentOmni));
        }

        public override void RequestCloseView()
        {
            base.RequestCloseView();

            DataChanged = null;
        }

        #endregion

        #region Commands

        public event EventHandler DataChanged;

        private ICommand _saveOmniCommand;
        public ICommand SaveOmniCommand
        {
            get { return _saveOmniCommand ?? (_saveOmniCommand = new SimpleCommand(SaveOmni)); }
        }

        private ICommand _addTagCommand;
        public ICommand AddTagCommand
        {
            get { return _addTagCommand ?? (_addTagCommand = new SimpleCommand(AddTag)); }
        }

        private ICommand _deleteTagCommand;
        public ICommand DeleteTagCommand
        {
            get { return _deleteTagCommand ?? (_deleteTagCommand = new ParameterCommand(DeleteTag)); }
        }

        private ICommand _showHelpCommand;
        public ICommand ShowHelpCommand
        {
            get { return _showHelpCommand ?? (_showHelpCommand = new SimpleCommand(ShowHelp)); }
        }

        #endregion
    }
}
