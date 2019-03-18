using System;
using System.Threading.Tasks;
using AutoInc;

namespace TestApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var fileStore = new FileStore(@"c:\temp");

            var value = await fileStore.GetValue("ABC");
            Console.WriteLine(value);
            value++;
            await fileStore.TryWriteValue("ABC", value);
            value++;
            await fileStore.TryWriteValue("ABC", value);
            value = await fileStore.GetValue("ABC");
            Console.WriteLine(value);
        }
    }
}
