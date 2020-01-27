namespace AutoInc
{
    public static class Query
    {
        public static string NextId => $@"
                MERGE (n:{Neo4jOptions.LabelName} {{Scope: $scope}})
                SET n.Value = COALESCE(n.Value, 0) + 1
                RETURN n.Value";

        public static string Update => $@"
                MERGE (n:{Neo4jOptions.LabelName} {{Scope: $scope}})
                SET n.Value = $value";

        public static string UniqueScopeConstraint => $"CREATE CONSTRAINT ON (u:{Neo4jOptions.LabelName}) ASSERT u.Scope IS UNIQUE";
        public static string ScopeExistsConstraint => $"CREATE CONSTRAINT ON (p:{Neo4jOptions.LabelName}) ASSERT exists(p.Scope)";
        public static string ValueExistsConstraint => $"CREATE CONSTRAINT ON (p:{Neo4jOptions.LabelName}) ASSERT exists(p.Value)";
    }
}