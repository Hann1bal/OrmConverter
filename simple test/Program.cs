using Newtonsoft.Json;
using simple_test.src;

namespace simple_test;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var json = File.ReadAllText(@"./source.json");
        var jsonParsed = JsonConvert.DeserializeObject<dynamic>(json);
        var parser = new Parser();
        parser.Run(jsonParsed);
    }
}