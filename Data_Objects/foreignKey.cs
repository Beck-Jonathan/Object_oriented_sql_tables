namespace Data_Objects
{
    public class foreignKey
    {
        public foreignKey() { }
        public string mainTable { get; set; }
        public string referenceTable { get; set; }
        public string fieldName { get; set; }
        public string dataType { get; set; }
        public string lengthText { get; set; }
    }
}
