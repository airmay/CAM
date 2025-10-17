namespace CAM;

public interface IOperation
{
    string Caption { get; set; }
    bool Enabled { get; set; }
    short Number { get; set; }
    Tool GetTool();
    AcadObject ProcessingArea { get; }
}