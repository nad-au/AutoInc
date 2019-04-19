using Neo4j.Driver.V1;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
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
                CREATE (id:{options.LabelName})
                SET id.Scope = $scope
                SET id.Value = $value";

            // Act
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await new Neo4jUniqueIdGenerator(tx, options).Initialise();
                });
            }

            // Assert
            var exception = Assert.ThrowsAsync<ClientException>(async () =>
            {
                using (var session = driver.Session(AccessMode.Write))
                {
                    await session.WriteTransactionAsync(async tx =>
                    {
                        await tx.RunAsync(createIdQuery, parameters);
                        await tx.RunAsync(createIdQuery, parameters);
                    });
                }
            });

            var expectedMessage = $"already exists with label `{options.LabelName}` and property `Scope` = '{scope}'";
            Assert.IsTrue(exception.Message.EndsWith(expectedMessage));
        }
    }
}
