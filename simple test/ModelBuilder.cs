using System.Reflection;

namespace simple_test;

public class ModelBuilder
{
    private readonly List<string> _fileLineList = new();
    private readonly List<Model> Models = new();

    public void AddTable(string TableName, List<Dictionary<string, string>> propertiesList, string toTableRelation)
    {
        var model = new Model
        {
            TableName = NormalizeClassName(TableName), Properties = new List<Properties>(),
            ToFieldRelation = toTableRelation
        };
        foreach (var propertyField in propertiesList.Select(property => new Properties
                     { Name = property.Keys.First(), FieldType = property.Values.First() }))
            model.Properties.Add(propertyField);

        Models.Add(model);
    }

    public void GenerateModelsFile()
    {
        _fileLineList.Add(Fields.Init);
        _fileLineList.Add(Fields.NewLine);
        foreach (var model in Models)
        {
            _fileLineList.Add(string.Format(Fields.ClassDefinition, model.TableName));
            _fileLineList.Add(Fields.Tab + String.Format(Fields.PrimaryKey, model.TableName.ToLower()));
            foreach (var field in model.Properties)
            {
                _fileLineList.Add(Fields.Tab +
                                  String.Format(
                                      typeof(Fields).GetFields(BindingFlags.Public | BindingFlags.Static)
                                          .First(c => c.Name == field.FieldType).GetValue(null).ToString(),
                                      NormalizeFieldName(field.Name),
                                      NormalizeClassName(model.ToFieldRelation))
                );
            }
        }
        SaveToFile();
    }

    private void SaveToFile()
    {
        using var writer = new StreamWriter("C:\\Users\\Andrey\\models.py");
        foreach (var line in _fileLineList) writer.WriteLine(line);
    }

    private string NormalizeClassName(string TableName)
    {
        var firstChapter = TableName.ToLower().ToCharArray()[0].ToString().ToUpper();
        var substring = TableName.AsSpan(1).ToString();
        return $"{firstChapter}{substring}";
    }

    private string NormalizeFieldName(string FieldName)
    {
        return FieldName.ToLower().Replace(" ", "_");
    }
}