using System;
using OmniTagWPF.ViewModels.Base;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.Win32;
using NCGLib.WPF.Commands;

namespace OmniTagWPF.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Properties
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

        private bool _dataSourceChanged;

        public bool IsPortable { get { return Context.IsPortable; } }
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

            if (_dataSourceChanged)
            {
                MessageBox.Show("The data source has been modified. OmniTag will now restart to prevent data corruption.",
                    "Alert", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
                return;
            }
                
            RequestCloseView();
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
