namespace OmniTagWPF.Utility
{
    class GridDimensions
    {
        public GridDimensions(int cols, int rows)
        {
            Columns = cols;
            Rows = rows;
        }
        
        public int Columns { get; set; }

        public int Rows { get; set; }
    }
}
