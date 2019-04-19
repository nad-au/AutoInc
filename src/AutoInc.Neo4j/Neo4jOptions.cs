namespace AutoInc
{
    public class Neo4jOptions
    {
        public const string DefaultLabelName = "UniqueId";

        public string LabelName { get; set; } = DefaultLabelName;
        public bool UseNeo4jEnterprise { get; set; } = false;
    }
}
