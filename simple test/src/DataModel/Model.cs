﻿namespace simple_test.src.DataModel;

public class Model
{
    public string? TableName { get; set; }
    public List<Properties> Properties { get; set; }
    public string? ToFieldRelation { get; set; }
    public Enum? DbRelationTypes { get; set; }
}