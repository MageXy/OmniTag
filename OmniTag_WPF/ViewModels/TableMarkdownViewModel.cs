using System;
using System.Data;
using NCGLib.WPF.Utility.Input;
using OmniTagWPF.Utility;

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

            UseFullBorder = false;
            UseHeaderBorder = true;
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

        private bool _useFullBorder;
        public bool UseFullBorder
        {
            get { return _useFullBorder; }
            set
            {
                PropNotify.SetProperty(ref _useFullBorder, value, n =>
                {
                    if (n == true)
                        UseHeaderBorder = true;
                });
            }
        }

        private bool _useHeaderBorder;
        public bool UseHeaderBorder
        {
            get { return _useHeaderBorder; }
            set { PropNotify.SetProperty(ref _useHeaderBorder, value); }
        }

        #endregion

        #region Methods

        public string GetTableString()
        {
            if (UserCancelled)
                return String.Empty;
            
            var tableStringCreator = new TableMarkdownGenerator(Data);
            var str = tableStringCreator.GetTableMarkdownString(UseFullBorder, UseHeaderBorder);
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
        private const string Corner_TL          = "╔";
        private const string Corner_TR          = "╗";
        private const string Corner_BL          = "╚";
        private const string Corner_BR          = "╝";
        private const string Horiz_DoubleLine   = "═";
        private const string Horiz_SingleLine   = "─";
        private const string Vert_DoubleLine    = "║";
        private const string Vert_SingleLine    = "│";
        private const string Intersection       = "┼";
        private const string ColumnSplit_Top    = "╤";
        private const string ColumnSplit_Bottom = "╧";
        private const string ColumnSplit_Left   = "╟";
        private const string ColumnSplit_Right  = "╢";
        private const string Indent             = "    ";

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

        public string GetTableMarkdownString(bool useFullBorders, bool useHeaderBorders)
        {
            /* Top Row Border */
            var retVal = PrintHomogeneousRow(Corner_TL, Corner_TR, ColumnSplit_Top, Horiz_DoubleLine);

            /* Column Headers */
            retVal += PrintTextRow(0, Vert_DoubleLine, Vert_DoubleLine, Vert_SingleLine);

            /* Header/Body Separator */
            if (useHeaderBorders)
                retVal += PrintHomogeneousRow(ColumnSplit_Left, ColumnSplit_Right, Intersection, Horiz_SingleLine);

            /* Data */
            for (var r = 1; r < Table.Rows.Count; r++)
            {
                retVal += PrintTextRow(r, Vert_DoubleLine, Vert_DoubleLine, Vert_SingleLine);
                if (useFullBorders && (r < Table.Rows.Count-1))
                    retVal += PrintHomogeneousRow(ColumnSplit_Left, ColumnSplit_Right, Intersection, Horiz_SingleLine);
            }
            
            /* Bottom Row Border */
            retVal += PrintHomogeneousRow(Corner_BL, Corner_BR, ColumnSplit_Bottom, Horiz_DoubleLine);

            return retVal;
        }

        private string PrintHomogeneousRow(string leftBorder, string rightBorder, string columnSeparator, string filler)
        {
            var retVal = Indent + leftBorder;
            for (var count = 0; count < Table.Columns.Count; count++)
            {
                for (int x = 0; x < _widestInColumn[count]+2; x++)
                    retVal += filler;

                if (count < Table.Columns.Count - 1)
                    retVal += columnSeparator;
            }
            retVal += rightBorder + "\n";

            return retVal;
        }

        private string PrintTextRow(int rowIndex, string leftBorder, string rightBorder, string columnSeparator)
        {
            var retVal = Indent + leftBorder;
            for (var count = 0; count < Table.Columns.Count; count++)
            {
                retVal += " " + Table.Rows[rowIndex][count].ToString()
                                .PadRight(_widestInColumn[count] + 1);

                if (count < Table.Columns.Count - 1)
                    retVal += columnSeparator;
            }
            retVal += rightBorder + "\n";

            return retVal;
        }
    }
}
