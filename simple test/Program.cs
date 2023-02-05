// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace simple_test;

/*
		IMPORT("from django.db import models"),
		CLASS_DEF("class {name}(models.Model):"),
		PRIMARY_KEY(" = models.AutoField(primary_key=True)"),
		MANYTOONE(" = models.ForeignKey({to}, on_delete=models.CASCADE)"),
		MANYTOMANY(" = models.ManyToManyField({to})"), 
		ONETOONE(" = models.OneToOneField({to},on_delete=models.CASCADE)"), 
		VARCHAR_30("models.CharField(max_length=30)"),
		VARCHAR_100(" = models.CharField(max_length=100)"), 
		INTEGER(" = models.IntegerField()"),
		BOOLEAN(" = models.BooleanField()"),
		DATE(" = models.DateField()"),
		DATETIME(" = models.DateTimeField()"),
		EMAIL(" = models.EmailField(max_length=254)");
*/

internal sealed class Program
{
    private readonly ModelBuilder _modelBuilder = new();
    private string? _currentRoot;

    private static void Main(string[] args)
    {
        var sourceData =
            "{\"person\": {" +
            "\"firstname\": \"John\", \"lastname\": \"Doe\", \"Address\": {" +
            "\"city\": \"Yaound\", \"postcode\": \"BP:324\"}, \"visitedcities\":" +
            " [{\"name\": \"Yaounde\", \"country\": \"Cameroon\"}, {\"name\": \"Douala\", \"country\": \"Cameroon\", \"innerlist\":{\"asdasd\":\"asd\"}}," +
            " {\"name\": \"Bafoussam\", \"country\": \"Cameroon\"}], \"dob\": \"10-10-2000\", \"roomnumber\": 1, \"loveSummer\": \"false\"}}";

        var jsonParsed = JsonConvert.DeserializeObject<dynamic>(sourceData);
        var programm = new Program();
        programm.Run(jsonParsed);
    }

    private void Run(object obj)
    {
        Parser(obj);
        _modelBuilder.GenerateModelsFile();
    }

    private void Parser(object obj, string? lastroot = null, List<Dictionary<string, string>> properties = null)
    {
        switch (obj)
        {
            case JObject jObject:
                foreach (var tuple in jObject)
                {
                    if (tuple.Value.Type is JTokenType.Array or JTokenType.Object)
                    {
                        properties = new List<Dictionary<string, string>>();
                        _currentRoot = tuple.Key; 
                    }
                    Console.WriteLine($"{tuple.Key}, {lastroot}");
                    Parser(tuple.Value, _currentRoot, properties);
                }

                switch (obj)
                {
                    case JObject obs:
                        _modelBuilder.AddTable(obs.Parent.Parent.Value<JObject>().Properties().Select(c=>c.Name).FirstOrDefault(), properties, _currentRoot);
                        break;
                    case JArray oas:
                        _modelBuilder.AddTable(oas.Parent.Parent.Value<JObject>().Properties().Select(c=>c.Name).FirstOrDefault(), properties, _currentRoot);
                        break;
                }
                break;

            case JArray jArray:
                foreach (var token in jArray)
                foreach (var keyValuePair in token.Value<JObject>())
                {
                    if (keyValuePair.Value.Type is JTokenType.Array or JTokenType.Object)
                    {
                        properties = new List<Dictionary<string, string>>();
                        _currentRoot = keyValuePair.Key;
                        _modelBuilder.AddTable(keyValuePair.Key, properties, _currentRoot);
                    }
                    Parser(keyValuePair.Value, _currentRoot, properties);
                }

                break;

            case JValue value:
                var name = value.Parent.Parent.Value<JObject>().ToObject<JObject>().Properties().FirstOrDefault(c => c.Value.Value<object>().Equals(value.Value<object>()))?.Name;
                switch (Type.GetTypeCode(value.Value.GetType()))
                {
                    case TypeCode.String:
                        var textValue = value.Value<string>();
                        if (properties.Count > 0 && properties.FirstOrDefault(c => c.Keys.Contains(name)) != null)
                        {
                            if (GetDbCharType(textValue.Length) < (StringTypes)Enum.Parse(typeof(StringTypes), properties.Where(c => c.Keys.Contains(name)).Select(x => x[name]).First())) break;
                            properties.FirstOrDefault(c => c.Keys.Contains(name))[name] = Enum.GetName(GetDbCharType(textValue.Length));
                        }
                        properties.Add(new Dictionary<string, string> { { name, Enum.GetName(typeof(StringTypes), GetDbCharType(textValue.Length)) } });
                        break;
                    case TypeCode.Double:
                        properties.Add(new Dictionary<string, string> { { name, "Double" } });
                        break;
                    case TypeCode.Int64:
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                        properties.Add(new Dictionary<string, string> { { name, "Integer" } });
                        break;
                    case TypeCode.Boolean:
                        properties.Add(new Dictionary<string, string> { { name, "Boolean" } });
                        break;
                    case TypeCode.DateTime:
                        properties.Add(new Dictionary<string, string> { { name, "DateTimeField" } });
                        break;
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        break;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Char:
                        break;
                    case TypeCode.SByte:
                        break;
                    case TypeCode.Byte:
                        break;
                    case TypeCode.UInt16:
                        break;
                    case TypeCode.UInt32:
                        break;
                    case TypeCode.UInt64:
                        break;
                    case TypeCode.Single:
                        break;
                    case TypeCode.Decimal:
                        break;
                }

                break;
        }
    }

    private StringTypes GetDbCharType(int lenght)
    {
        return lenght switch
        {
            >= 1 and <= 32 => StringTypes.Varchar32,
            > 32 and <= 64 => StringTypes.Varchar64,
            > 64 and <= 128 => StringTypes.Varchar128,
            > 128 and <= 256 => StringTypes.Varchar256,
            _ => StringTypes.Text
        };
    }
}