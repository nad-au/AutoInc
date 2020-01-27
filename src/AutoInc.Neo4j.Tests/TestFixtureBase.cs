using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        protected IDriver Driver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var neo4JConfiguration = configuration.GetSection(typeof(Neo4jConfiguration).Name).Get<Neo4jConfiguration>();

            Driver = GraphDatabase.Driver(neo4JConfiguration.Neo4jBoltUri,
                AuthTokens.Basic(neo4JConfiguration.Neo4jUsername, neo4JConfiguration.Neo4jPassword),
                builder => builder.WithMaxTransactionRetryTime(TimeSpan.FromSeconds(15)));

            Neo4jOptions.LabelName = $"p{Guid.NewGuid():N}";
        }

        [TearDown]
        public async Task TearDown()
        {
            var session = Driver.AsyncSession();
            await session.RunAsync(
                    $"MATCH (id:{Neo4jOptions.LabelName}) DELETE id").ConfigureAwait(false);

            try
            {
                var tx = await session.BeginTransactionAsync().ConfigureAwait(false);
                await tx.RunAsync(
                    $"DROP CONSTRAINT ON (u:{Neo4jOptions.LabelName}) ASSERT u.Scope IS UNIQUE").ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);
            }
            catch (DatabaseException)
            {
            }

            await session.CloseAsync().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Driver.Dispose();
        }
    }
}
