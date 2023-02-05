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
            _fileLineList.Add(Fields.NewLineWithTab);
            _fileLineList.Add(Fields.PrimaryKey);
            foreach (var field in model.Properties)
                switch (field.FieldType)
                {
                    case "Text":
                        _fileLineList.Add(string.Format(Fields.TextField, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Varchar32":
                        _fileLineList.Add(string.Format(Fields.Varchar32, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Varchar64":
                        _fileLineList.Add(string.Format(Fields.Varchar64, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Varchar128":
                        _fileLineList.Add(string.Format(Fields.Varchar128, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Varchar256":
                        _fileLineList.Add(string.Format(Fields.Varchar256, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Integer":
                        _fileLineList.Add(string.Format(Fields.Integer, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Boolean":
                        _fileLineList.Add(string.Format(Fields.Boolean, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Double":
                        _fileLineList.Add(string.Format(Fields.Double, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Date":
                        _fileLineList.Add(string.Format(Fields.Date, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "DateTime":
                        _fileLineList.Add(string.Format(Fields.DateTime, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "Email":
                        _fileLineList.Add(string.Format(Fields.Email, NormalizeFieldName(field.Name)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "OneToOne":
                        _fileLineList.Add(string.Format(Fields.OneToOne, NormalizeFieldName(field.Name),
                            NormalizeClassName(model.ToFieldRelation)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "ForeignKey":
                        _fileLineList.Add(string.Format(Fields.ForeignKey, NormalizeFieldName(field.Name),
                            NormalizeClassName(model.ToFieldRelation)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                    case "ManyToMany":
                        _fileLineList.Add(string.Format(Fields.ManyToMany, NormalizeFieldName(field.Name),
                            NormalizeClassName(model.ToFieldRelation)));
                        _fileLineList.Add(Fields.NewLine);
                        break;
                }
        }

        _fileLineList.Add(Fields.NewLine);
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