namespace CommandLineTool.Models
{
    public class Cell
    {
        public int x { get; set; }
        public int y { get; set; }
        public string type {get; set;}
        public bool hasTurbine {get; set;}

        public Cell(int x, int y, string type, bool hasTurbine)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            this.hasTurbine = hasTurbine;
        }
    }    
}
