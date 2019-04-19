using Microsoft.Extensions.Configuration;
using Neo4j.Driver.V1;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AutoInc.Neo4j.Tests
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        protected IDriver driver;
        protected Neo4jOptions options;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var neo4JConfiguration = configuration.GetSection(typeof(Neo4jConfiguration).Name).Get<Neo4jConfiguration>();

            driver = GraphDatabase.Driver(neo4JConfiguration.Neo4jBoltUri,
                AuthTokens.Basic(neo4JConfiguration.Neo4jUsername, neo4JConfiguration.Neo4jPassword),
                new Config { MaxTransactionRetryTime = TimeSpan.FromSeconds(15) });

            options = new Neo4jOptions
            {
                LabelName = $"p{Guid.NewGuid():N}"
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                await session.RunAsync(
                    $"MATCH (id:{options.LabelName}) DELETE id");

                try
                {
                    await session.RunAsync(
                        $"DROP CONSTRAINT ON (u:{options.LabelName}) ASSERT u.Scope IS UNIQUE");
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver.Dispose();
        }
    }
}
