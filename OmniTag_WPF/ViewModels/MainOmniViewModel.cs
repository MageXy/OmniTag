using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NCGLib;
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

        private TagViewerViewModel _tagSearchDataContext;
        public TagViewerViewModel TagSearchDataContext
        {
            get { return _tagSearchDataContext; }
            set { PropNotify.SetProperty(ref _tagSearchDataContext, value); }
        }

        private List<Omni> _availableOmnis;
        public List<Omni> AvailableOmnis
        {
            get { return _availableOmnis; }
            set { PropNotify.SetProperty(ref _availableOmnis, value); }
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
            AvailableOmnis = Context.Omnis.Where(o => o.DateDeleted == null).OrderBy(o => o.Summary).ToList();
            if (AvailableOmnis.Count > 0)
                SelectedOmni = AvailableOmnis.First();

            var tags = new ObservableCollection<Tag>(Context.Tags.Where(t => t.DateDeleted == null).OrderBy(t => t.Name).ToList());
            TagSearchDataContext = new TagViewerViewModel(tags);
            TagSearchDataContext.ShowAddButton = false;
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

        #endregion
    }
}
