using System.Windows;
using OmniTagWPF.ViewModels.Base;

namespace OmniTagWPF.ViewModels
{
    class TableMarkdownSizeViewModel : InputViewModel
    {
        public TableMarkdownSizeViewModel(string message, string caption) : base(message, caption)
        {

        }

        #region Properties

        private int _numCols;
        public int NumCols
        {
            get { return _numCols; }
            set { PropNotify.SetProperty(ref _numCols, value); }
        }

        private int _numRows;
        public int NumRows
        {
            get { return _numRows; }
            set { PropNotify.SetProperty(ref _numRows, value); }
        }

        #endregion

        #region Methods

        protected override void VerifyData()
        {
            if (NumCols < 1)
            {
                MessageBox.Show("Number of columns must be greater than zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
            if (NumRows < 1)
            {
                MessageBox.Show("Number of rows must be greater than zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            base.VerifyData();
        }

        #endregion

        #region Commands

        #endregion
    }
}
