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

sealed class Program
{
    private static void Main(string[] args)
    {
        var sourceData =
            "{\"person\": {" +
            "\"firstname\": \"John\", \"lastname\": \"Doe\", \"Address\": {" +
            "\"city\": \"Yaound\", \"postcode\": \"BP:324\"}, \"visitedcities\":" +
            " [{\"name\": \"Yaounde\", \"country\": \"Cameroon\"}, {\"name\": \"Douala\", \"country\": \"Cameroon\"}," +
            " {\"name\": \"Bafoussam\", \"country\": \"Cameroon\"}], \"dob\": \"10-10-2000\", \"roomnumber\": 1, \"loveSummer\": \"false\"}}";

        var jsonParsed = JsonConvert.DeserializeObject<dynamic>(sourceData);
        var parser = new Parser();
        parser.Run(jsonParsed);
    }
}