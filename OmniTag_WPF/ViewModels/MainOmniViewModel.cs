using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Input;
using NCGLib;
using NCGLib.Extensions;
using NCGLib.WPF.Commands;
using NCGLib.WPF.Utility;
using OmniTag.Models;
using OmniTagWPF.Utility;
using OmniTagWPF.ViewModels.Base;
using OmniTagWPF.ViewModels.Controls;
using OmniTagWPF.Views;

namespace OmniTagWPF.ViewModels
{
    class MainOmniViewModel : BaseViewModel
    {
        #region Properties

        private List<Omni> AllOmnis { get; set; } 

        private TagSearchViewModel _tagSearchDataContext;
        public TagSearchViewModel TagSearchDataContext
        {
            get { return _tagSearchDataContext; }
            set { PropNotify.SetProperty(ref _tagSearchDataContext, value); }
        }

        private SearchViewModel<Omni> _omniSearchDataContext;
        public SearchViewModel<Omni> OmniSearchDataContext
        {
            get { return _omniSearchDataContext; }
            set { PropNotify.SetProperty(ref _omniSearchDataContext, value); }
        }

        private ObservableCollection<TagButtonViewModel> _tagButtons;
        public ObservableCollection<TagButtonViewModel> TagButtons
        {
            get { return _tagButtons; }
            set { PropNotify.SetProperty(ref _tagButtons, value); }
        }

        private Omni _selectedOmni;
        public Omni SelectedOmni
        {
            get { return _selectedOmni; }
            set { PropNotify.SetProperty(ref _selectedOmni, value); }
        }


        [DependsOnProperty(nameof(SelectedOmni))]
        public string RenderedMarkdownHtml
        {
            get
            {
                return SelectedOmni == null
                    ? "&nbsp;"
                    : OmniTextRenderer.Render(SelectedOmni.Description?.Replace("\r\n", "\n")
                        .Replace("\n", Environment.NewLine) ?? String.Empty);
            }
        }
        
        #endregion

        #region Methods

        public override void LoadData()
        {
            TagButtons = new ObservableCollection<TagButtonViewModel>();
            TagButtons.CollectionChanged += OnTagButtonsChanged;

            AllOmnis = Context.Omnis.Where(o => o.DateDeleted == null).OrderBy(o => o.Summary).ToList();
            
            var tags = new ObservableCollection<Tag>(Context.Tags.Where(t => t.DateDeleted == null).OrderBy(t => t.Name).ToList());
            TagSearchDataContext = new TagSearchViewModel(tags)
            {
                HintText = "Filter by Tag...",
                DisplayMember = nameof(Tag.Name),
                ShowEnterButton = true,
                EnterText = "Add",
                EnterCommand = AddTagCommand,
                SearchCommand = AddTagCommand,
                UpdateSearchTextWhenSelectionChanges = true,
                EnableEnterFunc = s => TagSearchDataContext.FilteredTags.Any(t => t.Name == s),
                Filter = t => t.Name.ToUpper().Contains(TagSearchDataContext.SearchText.ToUpper())
            };

            OmniSearchDataContext = new SearchViewModel<Omni>(new ObservableCollection<Omni>(AllOmnis))
            {
                DisplayMember = nameof(Omni.Summary),
                Filter = o => o.Summary.ToUpper().Contains(OmniSearchDataContext.SearchText.ToUpper())
            };
            OmniSearchDataContext.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(OmniSearchDataContext.SelectedValue))
                    SelectedOmni = OmniSearchDataContext.SelectedValue;
            };
        }

        private void OnTagButtonsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TagButtons.Count == 0)
                OmniSearchDataContext.AllValues = new ObservableCollection<Omni>(AllOmnis);
            else
                OmniSearchDataContext.AllValues = new ObservableCollection<Omni>(
                    AllOmnis.Where(o => TagButtons.All(vm => o.Tags.Contains(vm.CurrentTag))).ToList()
                );
        }

        private void EditOmni(object omniToEdit)
        {
            Window view;
            if (omniToEdit == null)
            {
                view = ViewFactory.CreateViewWithDataContext<EditOmniView>(new EditOmniViewModel());
                view.ShowDialog();
                return;
            }

            var omni = omniToEdit as Omni;
            if (omni == null)
            {
                System.Diagnostics.Debug.WriteLine("Error converting Omni in the EditOmni method.");
                return;
            }

            view = ViewFactory.CreateViewWithDataContext<EditOmniView>(new EditOmniViewModel(omni));
            view.ShowDialog();
        }

        private void EditTags()
        {
            var view = ViewFactory.CreateWindow<EditTagView>();
            view.Show();
        }

        private void EditSettings()
        {
            if (!Context.IsPortable)
                return;

            var view = ViewFactory.CreateWindow<PortableSettingsView>();
            view.Show();
        }

        private void AddTag()
        {
            Tag tag = TagSearchDataContext.SelectedValue;
            if (tag == null)
            {
                tag = TagSearchDataContext.FilteredTags.SingleOrDefault(t => t.Name == TagSearchDataContext.FullSearchText);
                if (tag == null)
                    return;
            }

            if (TagButtons.All(tb => tb.CurrentTag.Name != tag.Name))
                TagButtons.Add(new TagButtonViewModel(tag));

            TagSearchDataContext.FullSearchText = String.Empty;
            TagSearchDataContext.SelectedValue = null;
        }

        private void RemoveTag(object param)
        {
            var tagVM = param as TagButtonViewModel;
            if (tagVM == null)
                return;

            TagButtons.Remove(tagVM);
        }

        #endregion

        #region Commands

        private ICommand _editOmniCommand;
        public ICommand EditOmniCommand
        {
            get { return _editOmniCommand ?? (_editOmniCommand = new ParameterCommand(EditOmni)); }
        }

        private ICommand _editTagsCommand;
        public ICommand EditTagsCommand
        {
            get { return _editTagsCommand ?? (_editTagsCommand = new SimpleCommand(EditTags)); }
        }

        private ICommand _editSettingsCommand;
        public ICommand EditSettingsCommand
        {
            get { return _editSettingsCommand ?? (_editSettingsCommand = new SimpleCommand(EditSettings)); }
        }

        private ICommand _addTagCommand;
        public ICommand AddTagCommand
        {
            get { return _addTagCommand ?? (_addTagCommand = new SimpleCommand(AddTag)); }
        }

        private ICommand _removeTagCommand;
        public ICommand RemoveTagCommand
        {
            get { return _removeTagCommand ?? (_removeTagCommand = new ParameterCommand(RemoveTag)); }
        }

        #endregion
    }
}
