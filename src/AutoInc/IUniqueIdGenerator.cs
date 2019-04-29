using System.Threading.Tasks;

namespace AutoInc
{
    public interface IUniqueIdGenerator
    {
        Task Initialise();

        long NextId(string scope);
        Task<long> NextIdAsync(string scope);

        void Update(string scope, long value);
        Task UpdateAsync(string scope, long value);
    }
}
