using Neo4j.Driver.V1;
using System.Threading.Tasks;

namespace AutoInc
{
    public static class Neo4jDriverExtensions
    {
        public static async Task InitialiseUniqueIds(this IDriver driver)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.InitialiseUniqueIds();
            }
        }

        public static async Task InitialiseUniqueIds(this ISession session)
        {
            await session.WriteTransactionAsync(
                async transaction => await transaction.InitialiseUniqueIds());
        }

        public static async Task InitialiseUniqueIds(this ITransaction transaction)
        {
            await new Neo4jUniqueIdGenerator(transaction).Initialise();
        }

        public static async Task UpdateUniqueId(this IDriver driver, string scope, long value)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.UpdateUniqueId(scope, value);
            }
        }

        public static async Task UpdateUniqueId(this ISession session, string scope, long value)
        {
            await session.WriteTransactionAsync(
                async transaction => await transaction.UpdateUniqueId(scope, value));
        }

        public static async Task UpdateUniqueId(this ITransaction transaction, string scope, long value)
        {
            await new Neo4jUniqueIdGenerator(transaction).Update(scope, value);
        }

        public static async Task<long> NextUniqueId(this IDriver driver, string scope)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                return await session.NextUniqueId(scope);
            }
        }

        public static async Task<long> NextUniqueId(this ISession session, string scope)
        {
            return await session.WriteTransactionAsync(
                async transaction => await transaction.NextUniqueId(scope));
        }

        public static async Task<long> NextUniqueId(this ITransaction transaction, string scope)
        {
            return await new Neo4jUniqueIdGenerator(transaction).NextId(scope);
        }
    }
}
