using Newtonsoft.Json.Linq;

namespace simple_test;

public class Parser
{
    private readonly ModelBuilder _modelBuilder = new();
    private string? _currentRoot;
    
    public void Run(object obj)
    {
        Parse(obj);
        _modelBuilder.GenerateModelsFile();
    }

    private void Parse(object obj, string? lastroot = null, List<Dictionary<string, string>> properties = null)
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
                    Parse(tuple.Value, _currentRoot, properties);
                    if (tuple.Value is JArray or JObject)
                        _modelBuilder.AddTable(tuple.Key, properties, _currentRoot);
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

                    Parse(keyValuePair.Value, _currentRoot, properties);
                }

                break;

            case JValue value:
                var name = value.Parent.Parent.Value<JObject>().ToObject<JObject>().Properties()
                    .FirstOrDefault(c => c.Value.Value<object>().Equals(value.Value<object>()))?.Name;
                properties.Add(new Dictionary<string, string> { { name, GetValueType(value) } });
                var textValue = value.Value<string>();
                if (properties.Count > 0 && properties.FirstOrDefault(c => c.Keys.Contains(name)) != null)
                {
                    if (GetDbCharType(textValue.Length) > (DbValuesTypes)Enum.Parse(typeof(DbValuesTypes),
                            properties.Where(c => c.Keys.Contains(name)).Select(x => x[name]).First())) properties.FirstOrDefault(c => c.Keys.Contains(name))[name] = GetValueType(value);break;
                }
                properties.Add(new Dictionary<string, string> { { name, GetValueType(value) } });
                break;
        }
    }

    private string GetValueType(JValue value)
    {
        if (Type.GetTypeCode(value.Value.GetType()) == TypeCode.String)
        {
            return Enum.GetName(GetDbCharType(value.Value<string>().Length));
        }

        return Enum.GetName(Type.GetTypeCode(value.Value.GetType())).Contains("Int")
            ? Enum.GetName(DbValuesTypes.Integer)
            : Enum.GetName(Type.GetTypeCode(value.Value.GetType()));
    }

    private DbValuesTypes GetDbCharType(int lenght)
    {
        return lenght switch
        {
            >= 1 and <= 32 => DbValuesTypes.Varchar32,
            > 32 and <= 64 => DbValuesTypes.Varchar64,
            > 64 and <= 128 => DbValuesTypes.Varchar128,
            > 128 and <= 256 => DbValuesTypes.Varchar256,
            _ => DbValuesTypes.Text
        };
    }
}