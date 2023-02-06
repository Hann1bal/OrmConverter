namespace simple_test.src.Static;

public class Fields
{
    public static readonly string Init = "from django.db import models";
    public static readonly string InitArray = "from django.contrib.postgres.fields import ArrayField";
    public static readonly string ClassDefinition = "class {0}(models.Model):";
    public static readonly string PrimaryKey = "id = models.AutoField(primary_key=True)";
    public static readonly string OneToOne = "{0}_id =  models.OneToOneField({1}, on_delete=models.CASCADE)";
    public static readonly string ForeignKey = "{0}_id = models.ForeignKey({1}, on_delete=models.CASCADE)";
    public static readonly string ManyToMany = "{0} = models.ManyToManyField({1})";
    public static readonly string TextField = "{0} = models.TextField()";
    public static readonly string Varchar32 = "{0} = models.CharField(max_length=32)";
    public static readonly string Varchar64 = "{0} = models.CharField(max_length=64)";
    public static readonly string Varchar128 = "{0} = models.CharField(max_length=128)";
    public static readonly string Varchar256 = "{0} = models.CharField(max_length=256)";
    public static readonly string Boolean = "{0} = models.BooleanField()";
    public static readonly string Date = "{0} = models.DateField()";
    public static readonly string DateTime = "{0} = models.DateTimeField()";
    public static readonly string Email = "{0} = models.EmailField(max_length=254)";
    public static readonly string Integer = "{0} = models.IntegerField()";
    public static readonly string Double = "{0} = models.FloatField()";
    public static readonly string ArrayField = "{0} = ArrayField({1}, blank=True)";
    public static readonly string NewLine = "\n";
    public static readonly string Tab = "\t";
    public static readonly string NewLineWithTab = "\n \t";
    public static readonly string Path = "models.py";
}


