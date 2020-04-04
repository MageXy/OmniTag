using System.Windows;
using NCGLib.WPF.Utility.Input;
using OmniTagWPF.Utility;

namespace OmniTagWPF.ViewModels
{
    class TableMarkdownSizeViewModel : InputViewModel<GridDimensions>
    {
        public TableMarkdownSizeViewModel(string message, string caption) : base(message, caption)
        {

        }

        #region Properties

        private int _numCols;
        public int NumCols
        {
            get { return _numCols; }
            set { SetProperty(ref _numCols, value); }
        }

        private int _numRows;
        public int NumRows
        {
            get { return _numRows; }
            set { SetProperty(ref _numRows, value); }
        }

        #endregion

        #region Methods

        public override void Confirm()
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

            SelectedValue = new GridDimensions(NumCols, NumRows);

            base.Confirm();
        }

        #endregion

        #region Commands

        #endregion
    }
}
