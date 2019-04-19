namespace AutoInc
{
    public static class Neo4jOptions
    {
        public const string DefaultLabelName = "UniqueId";

        public static string LabelName { get; set; } = DefaultLabelName;
        public static bool UseNeo4jEnterprise { get; set; }
    }
}
