using Neo4j.Driver.V1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoInc
{
    public class Neo4jUniqueIdGenerator : IUniqueIdGenerator, IUniqueIdValueStore
    {
        private readonly ITransaction transaction;
        private readonly Neo4jOptions options;

        public Neo4jUniqueIdGenerator(ITransaction transaction)
            : this(transaction, new Neo4jOptions()) { }

        public Neo4jUniqueIdGenerator(ITransaction transaction, Neo4jOptions options)
        {
            this.transaction = transaction;
            this.options = options;
        }

        public async Task Initialise()
        {
            await transaction.RunAsync(
                $"CREATE CONSTRAINT ON (u:{options.LabelName}) ASSERT u.Scope IS UNIQUE");

            if (options.UseNeo4jEnterprise)
            {
                await transaction.RunAsync(
                    $"CREATE CONSTRAINT ON (p:{options.LabelName}) ASSERT exists(p.Scope)");
                await transaction.RunAsync(
                    $"CREATE CONSTRAINT ON (p:{options.LabelName}) ASSERT exists(p.Value)");
            }
        }

        public async Task Update(string scope, long value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope},
                {"value", value}
            };

            await transaction.RunAsync($@"
                MERGE (n:{options.LabelName} {{Scope: $scope}})
                SET n.Value = $value", parameters);
        }

        public async Task<long> NextId(string scope)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope}
            };

            var cursor = await transaction.RunAsync($@"
                MERGE (n:{options.LabelName} {{Scope: $scope}})
                SET n.Value = COALESCE(n.Value, 0) + 1
                RETURN n.Value", parameters);

            var record = await cursor.SingleAsync();
            return record[0].As<long>();
        }
    }
}
