namespace CommandLineTool.Models
{
    public class Project
    {
        public string id { get; set; }
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
