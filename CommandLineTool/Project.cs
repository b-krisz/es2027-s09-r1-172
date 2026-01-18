namespace CommandLineTool
{
    public class Project
    {
        public string name;
        public string mapData;
        public Cell[] cells;

        public Project(string name, string mapData, Cell[] cells)
        {
            this.name = name;
            this.mapData = mapData;
            this.cells = cells;
        }
    }
}
