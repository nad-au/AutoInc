using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Wiry.Base32;

namespace AutoInc
{
    public class FileStore : IValueStore
    {
        private readonly string path;
        private readonly long seedValue;
        private readonly Dictionary<string, string> scopes = new Dictionary<string, string>();

        public FileStore(string path) 
            :this(path, Default.SeedValue)
        {
        }

        public FileStore(string path, long seedValue)
        {
            this.path = path;
            this.seedValue = seedValue;
        }

        public async Task<long> GetValue(string scopeName)
        {
            var fileName = GetOrAddFileName(scopeName);
            var filePath = Path.Combine(path, fileName);

            if (!File.Exists(filePath))
            {
                await TryWriteValue(scopeName, seedValue);
            }

            var text = await File.ReadAllTextAsync(filePath);
            return Convert.ToInt64(text);
        }

        public async Task<bool> TryWriteValue(string scopeName, long value)
        {
            var fileName = GetOrAddFileName(scopeName);
            var filePath = Path.Combine(path, fileName);

            try
            {
                await File.WriteAllTextAsync(filePath, value.ToString());
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        private string GetOrAddFileName(string scopeName)
        {
            if (scopes.ContainsKey(scopeName)) return scopes[scopeName];
            
            var base32 = GetFileNameFromScope(scopeName);
            scopes.Add(scopeName, base32);

            return scopes[scopeName];
        }

        public static string GetFileNameFromScope(string scopeName)
        {
            var inputBytes = Encoding.ASCII.GetBytes(scopeName);
            return Base32Encoding.Standard.GetString(inputBytes);
        }
    }
}
