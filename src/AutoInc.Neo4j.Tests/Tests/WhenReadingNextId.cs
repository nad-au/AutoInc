using Neo4j.Driver.V1;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
    public class WhenReadingNextId : TestFixtureBase
    {
        [Test]
        public async Task ItShouldFetchInitialValue()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";

            // Act
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var uniqueIdGenerator = new Neo4jUniqueIdGenerator(tx, options);

                    var initialId = await uniqueIdGenerator.NextId(scope);

                    // Assert
                    Assert.AreEqual(1, initialId);
                });
            }
        }

        [Test]
        public async Task ItShouldFetchNextValue()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";

            using (var session = driver.Session(AccessMode.Write))
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var uniqueIdGenerator = new Neo4jUniqueIdGenerator(tx, options);

                    var initialId = await uniqueIdGenerator.NextId(scope);

                    Assert.AreEqual(1, initialId);

                    // Act
                    var nextId = await uniqueIdGenerator.NextId(scope);

                    // Assert
                    Assert.AreEqual(2, nextId);
                });
            }
        }
    }
}
