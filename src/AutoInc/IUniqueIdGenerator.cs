using System.Threading.Tasks;

namespace AutoInc
{
    public interface IUniqueIdGenerator
    {
        Task<long> NextId(string scope);
    }
}
