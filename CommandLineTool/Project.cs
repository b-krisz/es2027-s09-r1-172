namespace CommandLineTool
{
    public class Project
    {
        public string name {get; set;}
        public string mapData {get; set;}
        public Cell[] cells {get; set;}

        public Project(string name, string mapData, Cell[] cells)
        {
            this.name = name;
            this.mapData = mapData;
            this.cells = cells;
        }
    }
}
