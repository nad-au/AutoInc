using Neo4j.Driver.V1;
using System.Threading.Tasks;

namespace AutoInc
{
    public static class Neo4jDriverExtensions
    {
        public static async Task InitialiseUniqueIds(this IDriver driver, Neo4jOptions options = null)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.InitialiseUniqueIds(options);
            }
        }

        public static async Task InitialiseUniqueIds(this ISession session, Neo4jOptions options = null)
        {
            await session.WriteTransactionAsync(
                async transaction => await transaction.InitialiseUniqueIds(options));
        }

        public static async Task InitialiseUniqueIds(this ITransaction transaction, Neo4jOptions options = null)
        {
            var uniqueIdGenerator = options != null
                ? new Neo4jUniqueIdGenerator(transaction, options)
                : new Neo4jUniqueIdGenerator(transaction);

            await uniqueIdGenerator.Initialise();
        }

        public static async Task UpdateUniqueId(this IDriver driver, string scope, long value, Neo4jOptions options = null)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.UpdateUniqueId(scope, value, options);
            }
        }

        public static async Task UpdateUniqueId(this ISession session, string scope, long value, Neo4jOptions options = null)
        {
            await session.WriteTransactionAsync(
                async transaction => await transaction.UpdateUniqueId(scope, value, options));
        }

        public static async Task UpdateUniqueId(this ITransaction transaction, string scope, long value, Neo4jOptions options = null)
        {
            var uniqueIdGenerator = options != null
                ? new Neo4jUniqueIdGenerator(transaction, options)
                : new Neo4jUniqueIdGenerator(transaction);

            await uniqueIdGenerator.Update(scope, value);
        }

        public static async Task<long> NextUniqueId(this IDriver driver, string scope, Neo4jOptions options = null)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                return await session.NextUniqueId(scope, options);
            }
        }

        public static async Task<long> NextUniqueId(this ISession session, string scope, Neo4jOptions options = null)
        {
            return await session.WriteTransactionAsync(
                async transaction => await transaction.NextUniqueId(scope, options));
        }

        public static async Task<long> NextUniqueId(
            this ITransaction transaction,
            string scope,
            Neo4jOptions options = null)
        {
            var uniqueIdGenerator = options != null
                ? new Neo4jUniqueIdGenerator(transaction, options)
                : new Neo4jUniqueIdGenerator(transaction);

            return await uniqueIdGenerator.NextId(scope);
        }
    }
}
