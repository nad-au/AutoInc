using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class WhenInitialising : TestFixtureBase
    {
        [Test]
        public async Task ItShouldCreateAUniqueConstraint()
        {
            // Arrange
            var scope = $"p{Guid.NewGuid():N}";

            var parameters = new Dictionary<string, object>
            {
                {"scope", scope},                    
                {"value", 123}
            };

            var createIdQuery = $@"
                CREATE (id:{Neo4jOptions.LabelName})
                SET id.Scope = $scope
                SET id.Value = $value";

            // Act
            await Driver.InitialiseUniqueIdsAsync().ConfigureAwait(false);

            // Assert
            var exception = Assert.ThrowsAsync<ClientException>(async () =>
            {
                var session = Driver.AsyncSession();
                var tx = await session.BeginTransactionAsync().ConfigureAwait(false);
                await tx.RunAsync(createIdQuery, parameters).ConfigureAwait(false);
                await tx.RunAsync(createIdQuery, parameters).ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);
                await session.CloseAsync().ConfigureAwait(false);
            });

            var expectedMessage = $"already exists with label `{Neo4jOptions.LabelName}` and property `Scope` = '{scope}'";
            Assert.IsTrue(exception.Message.EndsWith(expectedMessage));
        }
    }
}
