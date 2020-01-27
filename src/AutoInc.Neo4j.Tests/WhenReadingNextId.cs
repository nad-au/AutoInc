using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class WhenReadingNextId : TestFixtureBase
    {
        [Test]
        public async Task ItShouldFetchInitialValue()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";

            // Act
            var initialId = await Driver.NextUniqueIdAsync(scope).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, initialId);
        }

        [Test]
        public async Task ItShouldFetchNextValue()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";

            var session = Driver.AsyncSession();
            var tx = await session.BeginTransactionAsync().ConfigureAwait(false);

            var initialId = await tx.NextUniqueIdAsync(scope).ConfigureAwait(false);

            Assert.AreEqual(1, initialId);

            // Act
            var nextId = await tx.NextUniqueIdAsync(scope).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(2, nextId);
            
            await tx.CommitAsync().ConfigureAwait(false);
            await session.CloseAsync().ConfigureAwait(false);
        }
    }
}
