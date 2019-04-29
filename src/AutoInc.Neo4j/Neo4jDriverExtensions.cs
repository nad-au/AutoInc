using Neo4j.Driver.V1;
using System.Threading.Tasks;

namespace AutoInc
{
    // ReSharper disable once InconsistentNaming
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

        public static void UpdateUniqueId(this IDriver driver, string scope, long value)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                session.UpdateUniqueId(scope, value);
            }
        }

        public static async Task UpdateUniqueIdAsync(this IDriver driver, string scope, long value)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.UpdateUniqueIdAsync(scope, value);
            }
        }

        public static void UpdateUniqueId(this ISession session, string scope, long value)
        {
            session.WriteTransaction(
                transaction => transaction.UpdateUniqueId(scope, value));
        }

        public static async Task UpdateUniqueIdAsync(this ISession session, string scope, long value)
        {
            await session.WriteTransactionAsync(
                async transaction => await transaction.UpdateUniqueIdAsync(scope, value));
        }

        public static void UpdateUniqueId(this ITransaction transaction, string scope, long value)
        {
            new Neo4jUniqueIdGenerator(transaction).Update(scope, value);
        }

        public static async Task UpdateUniqueIdAsync(this ITransaction transaction, string scope, long value)
        {
            await new Neo4jUniqueIdGenerator(transaction).UpdateAsync(scope, value);
        }

        public static long NextUniqueId(this IDriver driver, string scope)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                return session.NextUniqueId(scope);
            }
        }

        public static async Task<long> NextUniqueIdAsync(this IDriver driver, string scope)
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                return await session.NextUniqueIdAsync(scope);
            }
        }

        public static long NextUniqueId(this ISession session, string scope)
        {
            return session.WriteTransaction(
                transaction => transaction.NextUniqueId(scope));
        }

        public static async Task<long> NextUniqueIdAsync(this ISession session, string scope)
        {
            return await session.WriteTransactionAsync(
                async transaction => await transaction.NextUniqueIdAsync(scope));
        }

        public static long NextUniqueId(this ITransaction transaction, string scope)
        {
            return new Neo4jUniqueIdGenerator(transaction).NextId(scope);
        }

        public static async Task<long> NextUniqueIdAsync(this ITransaction transaction, string scope)
        {
            return await new Neo4jUniqueIdGenerator(transaction).NextIdAsync(scope);
        }
    }
}
