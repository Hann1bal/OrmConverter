using System.Reflection;
using simple_test.src.DataModel;
using simple_test.src.Static;

namespace simple_test.src;

public class ModelBuilder
{
    private readonly List<string> _fileLineList = new();
    private readonly List<Model> Models = new();

    public void AddTable(string? tableName, Dictionary<string, string?> propertiesList, string? toTableRelation = null, Enum? relationTypes = null)
    {
        if (Models.Any(c => c.TableName == NormalizeClassName(tableName))) return;
        Models.Add(
            new Model
            {
                TableName = NormalizeClassName(tableName),
                Properties = new List<Properties>(propertiesList.Select(c =>
                    new Properties()
                    {
                        FieldType = c.Value,
                        Name = c.Key
                    }
                ).ToList()),
                ToFieldRelation = toTableRelation, DbRelationTypes = relationTypes,
            }
        );
    }

    public void GenerateModelsFile()
    {
        _fileLineList.Add(Fields.Init);
        if (Models.Any(c => c.Properties.Any(x => x.FieldType.Contains("Array")))) _fileLineList.Add(Fields.InitArray);
        _fileLineList.Add(Fields.NewLine);
        foreach (var model in Models)
        {
            _fileLineList.Add(string.Format(Fields.ClassDefinition, model.TableName));
            _fileLineList.Add($"{Fields.Tab}{string.Format(Fields.PrimaryKey, model.TableName?.ToLower())}");
            if (model.DbRelationTypes != null) _fileLineList.Add($"{Fields.Tab}{GetFullRelationString(model)}");

            foreach (var field in model.Properties)
                _fileLineList.Add(
                    $"{Fields.Tab}{GetFullString(field, model)}"
                );
            _fileLineList.Add(Fields.NewLine);
        }

        Console.WriteLine("Done");
        Console.WriteLine("Writing to file");
        SaveToFile();
        Console.WriteLine("Done");
    }

    private void SaveToFile()
    {
        using var writer = new StreamWriter(Fields.Path);
        foreach (var line in _fileLineList) writer.WriteLine(line);
    }

    private static string NormalizeClassName(string? tableName)
    {
        return string.IsNullOrEmpty(tableName)
            ? string.Empty
            : $"{char.ToUpper(tableName.First())}{tableName.Substring(1).ToLower().Replace(" ", "_")}";
    }

    private static string GetFullString(Properties field, Model model)
    {
        return string.Format
        (
            field.FieldType.Contains("Array") 
                ? GetArrayTypeOfFieldAsSting(field) 
                : GetTypeOfFieldAsString(field),
            NormalizeFieldName(field.Name),
            NormalizeClassName(model.ToFieldRelation)
        );
    }

    private static string GetArrayTypeOfFieldAsSting(Properties field)
    {
        var arrayType = field.FieldType?.Split("|").Last();
        return string.Format
        (
            Fields.ArrayField,
            field.Name,
            GetTypeOfFieldAsString(arrayType).Split("{0} = ").Last().Trim()
        );
    }

    private static string GetFullRelationString(Model model)
    {
        return string.Format
        (
            GetTypeOfFieldAsString(model.DbRelationTypes),
            NormalizeFieldName(model.ToFieldRelation),
            NormalizeClassName(model.ToFieldRelation)
        );
    }

    private static string? GetTypeOfFieldAsString(Properties field)
    {
        return typeof(Fields).GetFields(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(c => c.Name == field.FieldType)?.GetValue(null)?.ToString();
    }

    private static string? GetTypeOfFieldAsString(string field)
    {
        return typeof(Fields).GetFields(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(c => c.Name == field)
            ?.GetValue(null)?.ToString();
    }

    private static string? GetTypeOfFieldAsString(Enum field)
    {
        return typeof(Fields).GetFields(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(c => c.Name == Enum.GetName(typeof(DbRelationTypes), field))?.GetValue(null)?.ToString();
    }

    private static string NormalizeFieldName(string fieldName)
    {
        return fieldName.ToLower().Replace(" ", "_");
    }
}