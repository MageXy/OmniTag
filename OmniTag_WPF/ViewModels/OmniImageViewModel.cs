using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using NCGLib.WPF.Commands;
using NCGLib.WPF.Templates.ViewModels;
using OmniTag.Models;
using OmniTagDB;

namespace OmniTagWPF.ViewModels
{
    class OmniImageViewModel : NCGLibViewModel
    {
        public OmniImageViewModel(Omni omni, OmniTagContext context)
        {
            CurrentOmni = omni;
            Context = context;
        }

        #region Properties

        private OmniTagContext Context { get; set; }

        private Omni _currentOmni;
        public Omni CurrentOmni
        {
            get { return _currentOmni; }
            set { PropNotify.SetProperty(ref _currentOmni, value); }
        }

        private ObservableCollection<Image> _omniImages;
        public ObservableCollection<Image> OmniImages
        {
            get { return _omniImages; }
            set { PropNotify.SetProperty(ref _omniImages, value); }
        }

        private List<Image> _removedImages;
        public List<Image> RemovedImages
        {
            get { return _removedImages; }
            set { PropNotify.SetProperty(ref _removedImages, value); }
        }

        private bool _changesMade;
        public bool ChangesMade
        {
            get { return _changesMade; }
            set { PropNotify.SetProperty(ref _changesMade, value); }
        }

        #endregion

        #region Methods

        public override void LoadData()
        {
            OmniImages = new ObservableCollection<Image>(CurrentOmni.Images.ToList());
            RemovedImages = new List<Image>();
            ChangesMade = false;
        }

        private void RemoveImage(object obj)
        {
            var image = obj as Image;
            if (image == null)
                return;

            OmniImages.Remove(image);
            Context.Images.Remove(image);
            RemovedImages.Add(image);
            ChangesMade = true;
        }

        private void ReplaceImage(object obj)
        {
            var image = obj as Image;
            if (image == null)
                return;

            var ofd = new OpenFileDialog
            {
                Title = "Select image file...",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var ofdResult = ofd.ShowDialog();
            if ((ofdResult ?? false) == false)
                return;

            try
            {
                image.ImageData = File.ReadAllBytes(ofd.FileName);
                ChangesMade = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to read file. Error: " + e.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        #endregion

        #region Commands

        private ICommand _removeImageCommand;
        public ICommand RemoveImageCommand
        {
            get { return _removeImageCommand ?? (_removeImageCommand = new ParameterCommand(RemoveImage)); }
        }

        private ICommand _replaceImageCommand;
        public ICommand ReplaceImageCommand
        {
            get { return _replaceImageCommand ?? (_replaceImageCommand = new ParameterCommand(ReplaceImage)); }
        }

        #endregion
    }
}
