using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace AutoInc
{
    internal static class AsyncTransactionExtensions
    {
        public static async Task InitialiseAsync(this IAsyncTransaction tx)
        {
            await tx.RunAsync(Query.UniqueScopeConstraint).ConfigureAwait(false);

            if (Neo4jOptions.UseNeo4jEnterprise)
            {
                await tx.RunAsync(Query.ScopeExistsConstraint).ConfigureAwait(false);
                await tx.RunAsync(Query.ValueExistsConstraint).ConfigureAwait(false);
            }
        }

        public static async Task UpdateAsync(this IAsyncTransaction tx, string scope, long value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope},
                {"value", value}
            };

            await tx.RunAsync(Query.Update, parameters).ConfigureAwait(false);
        }

        public static async Task<long> NextIdAsync(this IAsyncTransaction tx, string scope)
        {
            var parameters = new Dictionary<string, object>
            {
                {"scope", scope}
            };

            var cursor = await tx.RunAsync(Query.NextId, parameters).ConfigureAwait(false);

            var record = await cursor.SingleAsync().ConfigureAwait(false);
            return record[0].As<long>();
        }
    }
}
