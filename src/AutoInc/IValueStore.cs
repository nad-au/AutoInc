using System.Threading.Tasks;

namespace AutoInc
{
    public interface IValueStore
    {
        Task<long> GetValue(string scopeName);
        Task<bool> TryWriteValue(string scopeName, long value);
    }
}