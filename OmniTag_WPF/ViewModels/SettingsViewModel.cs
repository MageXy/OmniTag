using System;
using OmniTagWPF.ViewModels.Base;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using NCGLib.WPF.Commands;
using OmniTag.Models;
using OmniTagWPF.Utility;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace OmniTagWPF.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Properties

        private bool _dataSourceChanged;
        private string _originalDataSource;

        public bool IsPortable { get { return Context.IsPortable; } }
        
        private string _dataSource;
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                PropNotify.SetProperty(ref _dataSource, value, (x) =>
                {
                    if (!IsLoading)
                        _dataSourceChanged = true;
                });
            }
        }

        private string _tagThreshold;
        public string TagThreshold
        {
            get { return _tagThreshold; }
            set { PropNotify.SetProperty(ref _tagThreshold, value); }
        }

        private bool _showTagFilter;
        public bool ShowTagFilter
        {
            get { return _showTagFilter; }
            set { PropNotify.SetProperty(ref _showTagFilter, value); }
        }

        private string _tempImageLocation;
        public string TempImageLocation
        {
            get
            {
                return String.Equals(Path.GetFullPath(_tempImageLocation), 
                        Path.GetFullPath(OmniTextRenderer.DefaultEmbeddedImageLocation), 
                        StringComparison.InvariantCultureIgnoreCase)
                    ? "[default location]"
                    : _tempImageLocation;
            }
            set { PropNotify.SetProperty(ref _tempImageLocation, value); }
        }
        
        private Setting AutoTagVerifyThresholdSetting { get; set; }
        private Setting ShowTagFilterSetting { get; set; }
        private Setting TempImageLocationSetting { get; set; }
        
        #endregion

        #region Methods

        public override void LoadData()
        {
            IsLoading = true;

            if (IsPortable)
            {
                if (ConfigurationManager.ConnectionStrings["SQLiteDatabaseContext"] == null)
                    DataSource = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                else
                    DataSource = ConfigurationManager.ConnectionStrings["SQLiteDatabaseContext"]
                                    .ConnectionString.Replace("data source=", String.Empty);
            }
            else
            {
                if (ConfigurationManager.ConnectionStrings["SQLServerDatabaseContext"] == null)
                {
                    DataSource = "data source=";
                    MessageBox.Show("Unable to locate connection details in config file.");
                }
                else
                    DataSource = ConfigurationManager.ConnectionStrings["SQLServerDatabaseContext"].ConnectionString;
            }
            _originalDataSource = DataSource;

            AutoTagVerifyThresholdSetting = Context.GetSettingOrDefaultAndSave(Setting.AutoTagVerificationThreshold, "5");
            TagThreshold = AutoTagVerifyThresholdSetting.Value;

            ShowTagFilterSetting = Context.GetSettingOrDefaultAndSave(Setting.ShowTagSearchOnStartup, "false");
            ShowTagFilter = Boolean.Parse(ShowTagFilterSetting.Value);

            TempImageLocationSetting = Context.GetSettingOrDefaultAndSave(Setting.EmbeddedImageTempDirectory,
                    OmniTextRenderer.DefaultEmbeddedImageLocation);
            TempImageLocation = TempImageLocationSetting.Value;

            IsLoading = false;
        }

        private void BrowseForDatabase()
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "SQLite Database (*.db)|*.db|All Files (*.*)|*.*";
            var result = ofd.ShowDialog();
            if (!(result ?? false))
                return;

            DataSource = ofd.FileName;
        }

        private void BrowseForTempImages()
        {
            var ofd = new FolderBrowserDialog();
            var result = ofd.ShowDialog();
            if (result != DialogResult.OK)
                return;

            TempImageLocation = ofd.SelectedPath + "\\";
        }

        private void ClearTempImages()
        {
            DirectoryInfo di = new DirectoryInfo(_tempImageLocation);
            foreach (var file in di.GetFiles())
                file.Delete();

            MessageBox.Show("Temporary image cache has been cleared.", "Images Cleared", MessageBoxButton.OK,
                MessageBoxImage.Information, MessageBoxResult.OK);
        }

        private void ResetDefaults()
        {
            TempImageLocation = OmniTextRenderer.DefaultEmbeddedImageLocation;
            TagThreshold = "5";
            ShowTagFilter = false;
        }

        private void SaveChanges()
        {
            if (_dataSourceChanged)
            {
                var result = MessageBox.Show("Updating the data source setting requires OmniTag to restart. " +
                                             "Unsaved work will be lost.\n\nAre you sure you want to continue?",
                    "Alert", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.OK);

                if (result == MessageBoxResult.No)
                    return;
            }

            var success = SaveConnectionString();
            success = success && SaveTagThreshold();
            success = success && SaveShowTagFilter();
            success = success && SaveTempImageLocation();

            if (!success)
                return;

            Context.SaveChanges();

            if (_dataSourceChanged)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
                return;
            }
            RequestCloseView();
        }

        private bool SaveConnectionString()
        {
            var connectionString = DataSource;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (IsPortable)
            {
                connectionString = "data source=" + connectionString;
                config.ConnectionStrings.ConnectionStrings["SQLiteDatabaseContext"].ConnectionString = connectionString;
            }
            else
            {
                config.ConnectionStrings.ConnectionStrings["SQLServerDatabaseContext"].ConnectionString = connectionString;
            }
            config.Save(ConfigurationSaveMode.Modified);
            return true;
        }

        private bool SaveTagThreshold()
        {
            int threshold;
            if (!Int32.TryParse(TagThreshold, out threshold) || (threshold < 0))
            {
                MessageBox.Show("Automatic tag verification threshold must be a positive whole number.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            AutoTagVerifyThresholdSetting.Value = TagThreshold;
            foreach (var tag in Context.Tags)
            {
                if (tag.Omnis.Count >= Int32.Parse(TagThreshold))
                    tag.IsVerified = true;
                else
                {
                    if (!tag.ManuallyVerified)
                        tag.IsVerified = false;
                }
            }
            return true;
        }

        private bool SaveShowTagFilter()
        {
            ShowTagFilterSetting.Value = ShowTagFilter.ToString();
            return true;
        }

        private bool SaveTempImageLocation()
        {
            TempImageLocationSetting.Value = _tempImageLocation;
            return true;
        }

        #endregion

        #region Commands

        private ICommand _browseForDatabaseCommand;
        public ICommand BrowseForDatabaseCommand
        {
            get { return _browseForDatabaseCommand ?? (_browseForDatabaseCommand = new SimpleCommand(BrowseForDatabase)); }
        }

        private ICommand _browseForTempImagesCommand;
        public ICommand BrowseForTempImagesCommand
        {
            get { return _browseForTempImagesCommand ?? (_browseForTempImagesCommand = new SimpleCommand(BrowseForTempImages)); }
        }

        private ICommand _clearTempImagesCommand;
        public ICommand ClearTempImagesCommand
        {
            get { return _clearTempImagesCommand ?? (_clearTempImagesCommand = new SimpleCommand(ClearTempImages)); }
        }

        private ICommand _resetDefaultsCommand;
        public ICommand ResetDefaultsCommand
        {
            get { return _resetDefaultsCommand ?? (_resetDefaultsCommand = new SimpleCommand(ResetDefaults)); }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new SimpleCommand(SaveChanges)); }
        }

        #endregion
    }
}
