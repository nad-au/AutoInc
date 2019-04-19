using Neo4j.Driver.V1;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await driver.UpdateUniqueId(scope, 123);

            // Assert
            using (var session = driver.Session())
            {
                var parameters = new Dictionary<string, object>
                {
                    {"scope", scope}
                };

                var cursor = await session.RunAsync($@"
                    MATCH (n:{Neo4jOptions.LabelName} {{Scope: $scope}})
                    RETURN n.Value", parameters);

                var record = await cursor.SingleAsync();
                var value = record[0].As<long>();

                Assert.AreEqual(123, value);

                var nextId = await session.NextUniqueId(scope);

                Assert.AreEqual(124, nextId);
            }
        }

        [Test]
        public async Task ItShouldUpdateScopeValue()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";

            using (var session = driver.Session(AccessMode.Write))
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var initialId = await tx.NextUniqueId(scope);

                    Assert.AreEqual(1, initialId);

                    // Act
                    await tx.UpdateUniqueId(scope, 123);

                    // Assert
                    var nextId = await tx.NextUniqueId(scope);

                    Assert.AreEqual(124, nextId);
                });
            }
        }
    }
}
