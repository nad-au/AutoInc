using System.Threading.Tasks;
using Neo4j.Driver;

namespace AutoInc
{
    public class UniqueIdGenerator : IUniqueIdGenerator
    {
        private readonly IDriver driver;

        public UniqueIdGenerator(IDriver driver)
        {
            this.driver = driver;
        }
        
        public async Task Initialise()
        {
            await driver.InitialiseUniqueIdsAsync().ConfigureAwait(false);
        }

        public long NextId(string scope)
        {
            return driver.NextUniqueId(scope);
        }

        public async Task<long> NextIdAsync(string scope)
        {
            return await driver.NextUniqueIdAsync(scope).ConfigureAwait(false);
        }

        public void Update(string scope, long value)
        {
            driver.UpdateUniqueId(scope, value);
        }

        public async Task UpdateAsync(string scope, long value)
        {
            await driver.UpdateUniqueIdAsync(scope, value).ConfigureAwait(false);
        }
    }
}
