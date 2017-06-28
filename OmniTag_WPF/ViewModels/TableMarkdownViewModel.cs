using System;
using System.Data;
using NCGLib.WPF.Utility.Input;

namespace OmniTagWPF.ViewModels
{
    class TableMarkdownViewModel : InputViewModel<string>
    {
        public TableMarkdownViewModel(int numCols, int numRows) : base("Enter table data", "Enter table data")
        {
            NumberOfColumns = numCols;
            NumberOfRows = numRows;

            Data = new DataTable();
            for (var c = 0; c < NumberOfColumns; c++)
                Data.Columns.Add();
            for (var r = 0; r < NumberOfRows; r++)
                Data.Rows.Add(Data.NewRow());
            
            UseHeaderBorder = true;
            UsePrettyPrint = true;
        }

        #region Properties

        private int _numberOfColumns;
        public int NumberOfColumns
        {
            get { return _numberOfColumns; }
            set { PropNotify.SetProperty(ref _numberOfColumns, value); }
        }

        private int _numberOfRows;
        public int NumberOfRows
        {
            get { return _numberOfRows; }
            set { PropNotify.SetProperty(ref _numberOfRows, value); }
        }

        private DataTable _data;
        public DataTable Data
        {
            get { return _data; }
            set { PropNotify.SetProperty(ref _data, value); }
        }

        private bool _useHeaderBorder;
        public bool UseHeaderBorder
        {
            get { return _useHeaderBorder; }
            set { PropNotify.SetProperty(ref _useHeaderBorder, value); }
        }

        private bool _usePrettyPrint;
        public bool UsePrettyPrint
        {
            get { return _usePrettyPrint; }
            set { PropNotify.SetProperty(ref _usePrettyPrint, value); }
        }

        #endregion

        #region Methods

        public string GetTableString()
        {
            if (UserCancelled)
                return String.Empty;
            
            var tableStringCreator = new TableMarkdownGenerator(Data);
            var str = tableStringCreator.GetTableMarkdownString(UseHeaderBorder, UsePrettyPrint);
            return str;
        }

        public override void Confirm()
        {
            UserCancelled = false;
            SelectedValue = GetTableString();

            base.Confirm();
        }

        #endregion

        #region Commands

        #endregion
    }

    class TableMarkdownGenerator
    {
        public TableMarkdownGenerator(DataTable table)
        {
            Table = table;
        }

        private DataTable _table;
        public DataTable Table
        {
            get { return _table; }
            set
            {
                _table = value;
                InitializeWidest();
            }
        }

        private int[] _widestInColumn;

        private void InitializeWidest()
        {
            _widestInColumn = new int[Table.Columns.Count];

            for (var r = 0; r < Table.Rows.Count; r++)
            {
                for (var c = 0; c < Table.Columns.Count; c++)
                {
                    var str = Table.Rows[r][c].ToString();
                    if (str.Length > _widestInColumn[c])
                        _widestInColumn[c] = str.Length;
                }
            }
        }

        public string GetTableMarkdownString(bool useHeaderBorders, bool prettyPrint)
        {
            var retVal = useHeaderBorders
                ? PrintTextRow(0, prettyPrint) + PrintTableHeaderRow(prettyPrint)
                : PrintTableHeaderRow(prettyPrint) + PrintTextRow(0, prettyPrint);

            for (var r = 1; r < Table.Rows.Count; r++)
            {
                retVal += PrintTextRow(r, prettyPrint);
            }

            return retVal;
        }

        private string PrintTableHeaderRow(bool prettyPrint = true)
        {
            var retVal = "|";
            for (var count = 0; count < Table.Columns.Count; count++)
            {
                if (prettyPrint)
                {
                    for (int x = 0; x < _widestInColumn[count] + 2; x++)
                        retVal += "-";
                }
                else
                    retVal += "-";

                retVal += "|";
            }
            retVal += "\n";

            return retVal;
        }

        private string PrintTextRow(int rowIndex, bool prettyPrint = true)
        {
            var retVal = "|";
            for (var count = 0; count < Table.Columns.Count; count++)
            {
                var padWidth = prettyPrint ? _widestInColumn[count] + 1 : 0;
                if (prettyPrint)
                    retVal += " ";
                retVal += Table.Rows[rowIndex][count].ToString().PadRight(padWidth);

                retVal += "|";
            }
            retVal += "\n";

            return retVal;
        }
    }
}
