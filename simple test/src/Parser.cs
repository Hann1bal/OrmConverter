using Newtonsoft.Json.Linq;
using simple_test.src.Static;

namespace simple_test.src;

public class Parser
{
    private readonly ModelBuilder _modelBuilder = new();

    public void Run(object sourceJsonObject)
    {
        Console.WriteLine("Start preprocessing...");
        Parse(sourceJsonObject);
        Console.WriteLine("Done");
        Console.WriteLine("Building Structure...");
        _modelBuilder.GenerateModelsFile();
    }

    private void Parse(object sourceJsonObject, string? rootNodeName = null, string? innerObjectName = null,
        DbRelationTypes? relationTypes = null)
    {
        Console.WriteLine("Start making Django model...");
        var modelProperties = new Dictionary<string, string?>();
        var jsonObjects = new Dictionary<string, JObject>();
        var jsonArray = new Dictionary<string, JArray>();
        var currentNodeName = "";
        switch (sourceJsonObject)
        {
            case JObject keyValuePairs:
                foreach (var keyValuePair in keyValuePairs)
                    KeyValuePairParse(keyValuePair, ref currentNodeName, jsonObjects, jsonArray, modelProperties);
                break;
            case JArray keyValuePairs:
                foreach (var pair in keyValuePairs)
                foreach (var keyValuePair in (pair as JObject)!)
                    KeyValuePairParse(keyValuePair, ref currentNodeName, jsonObjects, jsonArray, modelProperties);
                break;
        }

        if (modelProperties.Count != 0)
        {
            _modelBuilder.AddTable(innerObjectName, modelProperties, rootNodeName, relationTypes);
            rootNodeName = innerObjectName;
        }

        foreach (var jsonKeyValuePair in jsonObjects)
        {
            if (rootNodeName != null) relationTypes = DbRelationTypes.OneToOne;
            Parse(jsonKeyValuePair.Value, rootNodeName, jsonKeyValuePair.Key, relationTypes);
            relationTypes = null;
        }

        foreach (var arrayJsonKeyValuePair in jsonArray)
        {
            if
            (
                rootNodeName != null &&
                arrayJsonKeyValuePair.Value.Any(c => c.Type is JTokenType.Array or JTokenType.Object)
            )
                relationTypes = DbRelationTypes.ForeignKey;
            Parse(arrayJsonKeyValuePair.Value, rootNodeName, arrayJsonKeyValuePair.Key, relationTypes);
            relationTypes = null;
        }

        Console.WriteLine("Done");
    }

    private static void KeyValuePairParse(KeyValuePair<string, JToken?> keyValuePair, ref string? currentNode,
        IDictionary<string, JObject> jsonObjects, IDictionary<string, JArray> jsonArray,
        IDictionary<string, string?> properties)
    {
        if (keyValuePair.Value is { Type: JTokenType.Array or JTokenType.Object }) currentNode = keyValuePair.Key;

        switch (keyValuePair.Value)
        {
            case JObject jObject:
                jsonObjects.Add(keyValuePair.Key, jObject);
                break;

            case JArray jArray:
                var tmpNodeName = currentNode;
                if (jArray.HasValues)
                {
                    if (jArray.Any(c => c.Type is not (JTokenType.Array or JTokenType.Object)))
                    {
                        if (properties.Any(c => c.Key == tmpNodeName)) break;
                        // ReSharper disable once HeapView.BoxingAllocation
                        properties.Add(tmpNodeName, $"ArrayField|{GetValueType(jArray.First().Value<JValue>())}");
                        break;
                    }

                    jsonArray.Add(keyValuePair.Key, jArray);
                    break;
                }

                if (properties.All(c => c.Key != tmpNodeName))
                    properties.Add(tmpNodeName, "ArrayField|TextField");
                break;

            case JValue value:
                if (properties.ContainsKey(keyValuePair.Key)) break;
                properties.Add(keyValuePair.Key, GetValueType(value));
                break;
        }
    }

    private static string? GetValueType(JValue value)
    {
        if (Type.GetTypeCode(value.Value?.GetType()) == TypeCode.String)
            return Enum.GetName(GetDbCharType(value.Value<string>()!.Length));

        return Enum.GetName(Type.GetTypeCode(value.Value?.GetType()))!.Contains("Int")
            ? Enum.GetName(DbValuesTypes.Integer)
            : Enum.GetName(Type.GetTypeCode(value.Value?.GetType()));
    }

    private static DbValuesTypes GetDbCharType(int lenght)
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