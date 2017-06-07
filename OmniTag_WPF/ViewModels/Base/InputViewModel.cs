using System.Windows.Input;
using NCGLib.WPF.Commands;
using NCGLib.WPF.Templates;

namespace OmniTagWPF.ViewModels.Base
{
    abstract class InputViewModel : CloseableViewModel
    {
        protected InputViewModel(string message, string caption)
        {
            Message = message;
            Caption = caption;
            IsCancelled = true;
        }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set { PropNotify.SetProperty(ref _caption, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { PropNotify.SetProperty(ref _message, value); }
        }

        public bool IsCancelled { get; protected set; }

        #region Methods

        protected virtual void VerifyData()
        {
            IsCancelled = false;
            RequestCloseView();
        }

        #endregion

        #region Commands

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new SimpleCommand(VerifyData)); }
        }

        #endregion
    }
}
