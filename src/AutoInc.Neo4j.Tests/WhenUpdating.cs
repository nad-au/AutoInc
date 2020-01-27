using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class WhenUpdating : TestFixtureBase
    {
        [Test]
        public async Task ItShouldSetScopeInitialValue()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";

            // Act
            await Driver.UpdateUniqueIdAsync(scope, 123).ConfigureAwait(false);

            var session = Driver.AsyncSession();

            var parameters = new Dictionary<string, object>
            {
                {"scope", scope}
            };

            var cursor = await session.RunAsync($@"
                    MATCH (n:{Neo4jOptions.LabelName} {{Scope: $scope}})
                    RETURN n.Value", parameters).ConfigureAwait(false);

            var record = await cursor.SingleAsync().ConfigureAwait(false);
            var value = record[0].As<long>();

            Assert.AreEqual(123, value);

            var nextId = await session.NextUniqueIdAsync(scope).ConfigureAwait(false);

            Assert.AreEqual(124, nextId);
            
            await session.CloseAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task ItShouldUpdateScopeValue()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";
            
            var session = Driver.AsyncSession();
            var tx = await session.BeginTransactionAsync().ConfigureAwait(false);

            var initialId = await tx.NextUniqueIdAsync(scope).ConfigureAwait(false);

            Assert.AreEqual(1, initialId);

            // Act
            await tx.UpdateUniqueIdAsync(scope, 123).ConfigureAwait(false);

            // Assert
            var nextId = await tx.NextUniqueIdAsync(scope).ConfigureAwait(false);

            Assert.AreEqual(124, nextId);
            
            await tx.CommitAsync().ConfigureAwait(false);
            await session.CloseAsync().ConfigureAwait(false);
        }
    }
}
