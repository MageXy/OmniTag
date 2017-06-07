using System;
using System.Windows;
using OmniTagWPF.ViewModels.Base;

namespace OmniTagWPF.ViewModels
{
    class SimpleInputViewModel : InputViewModel
    {
        public SimpleInputViewModel(string message, string caption) : base(message, caption)
        {
            Input = null;
            DataValidationFunction = (s) => true;
        }

        #region Properties

        private string _input;
        public string Input
        {
            get { return _input; }
            set { PropNotify.SetProperty(ref _input, value); }
        }

        private Func<string, bool> _dataValidationFunction;
        public Func<string, bool> DataValidationFunction
        {
            get { return _dataValidationFunction; }
            set { PropNotify.SetProperty(ref _dataValidationFunction, value); }
        }

        //private bool _isMultiline;
        //public bool IsMultiline
        //{
        //	get { return _isMultiline; }
        //	set { PropNotify.SetProperty(ref _isMultiline, value); }
        //}

        //private bool _isResizeable;
        //public bool IsResizeable
        //{
        //	get { return _isResizeable; }
        //	set { PropNotify.SetProperty(ref _isResizeable, value); }
        //}
        
        #endregion
        
        #region Methods
        
        public override void RequestCloseView()
        {
            if (IsCancelled)
                Input = null;

            base.RequestCloseView();
        }

        protected override void VerifyData()
        {
            if (!DataValidationFunction(Input))
            {
                MessageBox.Show($"\"{Input}\" is not a valid input.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            base.VerifyData();
        }

        #endregion

        #region Commands



        #endregion
    }
}
