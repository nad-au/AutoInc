using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver;

namespace AutoInc
{
    internal static class TransactionExtensions
    {
        public static void Initialise(this ITransaction tx)
        {
            tx.Run(Query.UniqueScopeConstraint);

            if (Neo4jOptions.UseNeo4jEnterprise)
            {
                tx.Run(Query.ScopeExistsConstraint);
                tx.Run(Query.ValueExistsConstraint);
            }
        }

        public static void Update(this ITransaction tx, string scope, long value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope},
                {"value", value}
            };

            tx.Run(Query.Update, parameters);
        }

        public static long NextId(this ITransaction tx, string scope)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope}
            };

            var result = tx.Run(Query.NextId, parameters);

            var record = result.Single();
            return record[0].As<long>();
        }
    }
}
