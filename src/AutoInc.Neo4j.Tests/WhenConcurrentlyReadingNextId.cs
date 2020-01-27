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
            await Driver.InitialiseUniqueIdsAsync().ConfigureAwait(false);

            const int maxConcurrency = 100;

            var scope = $"p{Guid.NewGuid():N}";

            // Act
            var tasks = Enumerable.Range(0, maxConcurrency)
                .Select(i => Driver.NextUniqueIdAsync(scope));
            var allIds = (await Task.WhenAll(tasks).ConfigureAwait(false)).ToList();

            // Assert
            Assert.AreEqual(maxConcurrency, allIds.Count);

            for (var i = 1; i <= maxConcurrency; i++)
            {
                Assert.Contains(i, allIds, $"Ids did not return value: {i}");
            }
        }
    }
}
