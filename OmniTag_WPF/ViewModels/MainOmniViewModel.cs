using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NCGLib.WPF.Commands;
using NCGLib.WPF.Utility;
using OmniTag.Models;
using OmniTagWPF.ViewModels.Base;
using OmniTagWPF.Views;

namespace OmniTagWPF.ViewModels
{
    class MainOmniViewModel : BaseViewModel
    {
        #region Properties

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
        
        #endregion

        #region Methods

        public override void LoadData()
        {
            AvailableOmnis = Context.Omnis.ToList();
            if (AvailableOmnis.Count > 0)
                SelectedOmni = AvailableOmnis.First();

            
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
