using System;
using OmniTagWPF.ViewModels.Base;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Win32;
using NCGLib.Extensions;
using NCGLib.WPF.Commands;
using OmniTag.Models;
using OmniTagWPF.Properties;

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

        
        private Setting AutoTagVerifyThresholdSetting { get; set; }


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

            AutoTagVerifyThresholdSetting = Context.Settings.SingleOrDefault(s => s.Name == Setting.AutoTagVerificationThreshold);
            if (AutoTagVerifyThresholdSetting == null)
            {
                AutoTagVerifyThresholdSetting = new Setting
                {
                    Name = Setting.AutoTagVerificationThreshold,
                    Value = "5",
                    DateCreated = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };
                Context.Settings.Add(AutoTagVerifyThresholdSetting);
                Context.SaveChanges();
            }
            TagThreshold = AutoTagVerifyThresholdSetting.Value;

            IsLoading = false;
        }

        private void BrowseForDatabase()
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "SQLite Database (*.db)|*.db|All Files (*.*)|*.*";
            ofd.ShowDialog();

            DataSource = ofd.FileName;
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

        #endregion

        #region Commands

        private ICommand _browseForDatabaseCommand;
        public ICommand BrowseForDatabaseCommand
        {
            get { return _browseForDatabaseCommand ?? (_browseForDatabaseCommand = new SimpleCommand(BrowseForDatabase)); }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new SimpleCommand(SaveChanges)); }
        }

        #endregion
    }
}
