namespace CommandLineTool
{
    public class Cell
    {
        public int x;
        public int y;
        public string type;
        public bool hasTurbine;

        public Cell(int x, int y, string type, bool hasTurbine)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.hasTurbine = hasTurbine;
        }
    }    
}
