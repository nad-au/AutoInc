using Neo4j.Driver;
using System.Threading.Tasks;

namespace AutoInc
{
    // ReSharper disable once InconsistentNaming
    public static class AsyncAutoIncExtensions
    {
        public static async Task InitialiseUniqueIdsAsync(this IDriver driver)
        {
            var session = driver.AsyncSession(builder => builder.WithDefaultAccessMode(AccessMode.Write));
            await session.InitialiseUniqueIdsAsync().ConfigureAwait(false);
            await session.CloseAsync().ConfigureAwait(false);
        }

        public static async Task InitialiseUniqueIdsAsync(this IAsyncSession session)
        {
            await session.WriteTransactionAsync(
                async transaction => await transaction.InitialiseAsync().ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        public static async Task InitialiseUniqueIdsAsync(this IAsyncTransaction transaction)
        {
            await transaction.InitialiseAsync().ConfigureAwait(false);
        }

        public static async Task UpdateUniqueIdAsync(this IDriver driver, string scope, long value)
        {
            var session = driver.AsyncSession(builder => builder.WithDefaultAccessMode(AccessMode.Write));
            await session.UpdateUniqueIdAsync(scope, value).ConfigureAwait(false);
            await session.CloseAsync().ConfigureAwait(false);
        }

        public static async Task UpdateUniqueIdAsync(this IAsyncSession session, string scope, long value)
        {
            await session.WriteTransactionAsync(
                async transaction => await transaction.UpdateUniqueIdAsync(scope, value).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        public static async Task UpdateUniqueIdAsync(this IAsyncTransaction transaction, string scope, long value)
        {
            await transaction.UpdateAsync(scope, value).ConfigureAwait(false);
        }

        public static async Task<long> NextUniqueIdAsync(this IDriver driver, string scope)
        {
            var session = driver.AsyncSession(builder => builder.WithDefaultAccessMode(AccessMode.Write));
            var nextUniqueId = await session.NextUniqueIdAsync(scope).ConfigureAwait(false);
            await session.CloseAsync().ConfigureAwait(false);
            return nextUniqueId;
        }

        public static async Task<long> NextUniqueIdAsync(this IAsyncSession session, string scope)
        {
            return await session.WriteTransactionAsync(
                async transaction => await transaction.NextUniqueIdAsync(scope).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        public static async Task<long> NextUniqueIdAsync(this IAsyncTransaction transaction, string scope)
        {
            return await transaction.NextIdAsync(scope).ConfigureAwait(false);
        }
    }
}
