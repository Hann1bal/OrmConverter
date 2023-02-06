using Newtonsoft.Json;
using simple_test.src;

namespace simple_test;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var json = File.ReadAllText(@"C:\Users\Andrey\RiderProjects\simple test\simple test\src\Data\test.json");
        var jsonParsed = JsonConvert.DeserializeObject<dynamic>(json);
        var parser = new Parser();
        parser.Run(jsonParsed);
    }
}