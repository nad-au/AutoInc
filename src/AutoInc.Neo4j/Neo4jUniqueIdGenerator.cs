using Neo4j.Driver.V1;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoInc
{
    // ReSharper disable once InconsistentNaming
    public class Neo4jUniqueIdGenerator : IUniqueIdGenerator
    {
        private readonly ITransaction transaction;

        public Neo4jUniqueIdGenerator(ITransaction transaction)
        {
            this.transaction = transaction;
        }

        public async Task Initialise()
        {
            await transaction.RunAsync(
                $"CREATE CONSTRAINT ON (u:{Neo4jOptions.LabelName}) ASSERT u.Scope IS UNIQUE");

            if (Neo4jOptions.UseNeo4jEnterprise)
            {
                await transaction.RunAsync(
                    $"CREATE CONSTRAINT ON (p:{Neo4jOptions.LabelName}) ASSERT exists(p.Scope)");
                await transaction.RunAsync(
                    $"CREATE CONSTRAINT ON (p:{Neo4jOptions.LabelName}) ASSERT exists(p.Value)");
            }
        }

        public void Update(string scope, long value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope},
                {"value", value}
            };

            transaction.Run(GetUpdateQuery() , parameters);
        }

        public async Task UpdateAsync(string scope, long value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope},
                {"value", value}
            };

            await transaction.RunAsync(GetUpdateQuery(), parameters);
        }

        public long NextId(string scope)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope}
            };

            var result = transaction.Run(GetNextIdQuery(), parameters);

            var record = result.Single();
            return record[0].As<long>();
        }

        public async Task<long> NextIdAsync(string scope)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope}
            };

            var cursor = await transaction.RunAsync(GetNextIdQuery(), parameters);

            var record = await cursor.SingleAsync();
            return record[0].As<long>();
        }

        private string GetNextIdQuery()
        {
            return $@"
                MERGE (n:{Neo4jOptions.LabelName} {{Scope: $scope}})
                SET n.Value = COALESCE(n.Value, 0) + 1
                RETURN n.Value";
        }

        private string GetUpdateQuery()
        {
            return $@"
                MERGE (n:{Neo4jOptions.LabelName} {{Scope: $scope}})
                SET n.Value = $value";
        }
    }
}
