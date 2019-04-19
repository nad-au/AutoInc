using System.Threading.Tasks;

namespace AutoInc
{
    public interface IUniqueIdValueStore
    {
        Task Initialise();
        Task Update(string scope, long value);
    }
}
