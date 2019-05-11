using Serilog;
using System.IO;

namespace Codes.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger();

            Code code = new PoemCode(File.ReadAllText("poem.txt"), Log.Logger);

            var encoded = code.Encode("The creatures outside looked from pig to man, and from man to pig, " +
                "and from pig to man again; but already it was impossible to say which was which.");
        }
    }
}
