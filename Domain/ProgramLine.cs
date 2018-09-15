namespace CAM.Domain
{
    public class ProgramLine
    {
        public string Name { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }

        public string Value3 { get; set; }

        public string Value4 { get; set; }

        public string Value5 { get; set; }

        public ProgramLine(string name, string value1 = null, string value2 = null, string value3 = null, string value4 = null, string value5 = null)
        {
            Name = name;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
        }
    }
}