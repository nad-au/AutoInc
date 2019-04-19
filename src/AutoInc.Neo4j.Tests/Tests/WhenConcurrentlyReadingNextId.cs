using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class WhenConcurrentlyReadingNextId : TestFixtureBase
    {
        [Test]
        public async Task ItShouldReturnUniqueValues()
        {
            // Arrange
            await driver.InitialiseUniqueIds();

            const int maxConcurrency = 100;

            var scope = $"p{Guid.NewGuid():N}";

            // Act
            var tasks = Enumerable.Range(0, maxConcurrency)
                .Select(i => driver.NextUniqueId(scope));
            var allIds = (await Task.WhenAll(tasks)).ToList();

            // Assert
            Assert.AreEqual(maxConcurrency, allIds.Count);

            for (var i = 1; i <= maxConcurrency; i++)
            {
                Assert.Contains(i, allIds, $"Ids did not return value: {i}");
            }
        }
    }
}
