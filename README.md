# OrmConverter
Convert Json to Django Orm model. 
# Example: 
JSON: 
```json
{
  "person": {
    "first Name": "John",
    "last Name": "Doe",
    "sds": [1, 2, 2, 3, 4, 5, 6],
    "Address": {
      "city": "Yaoundé",
      "post code": "BP:324",
      "sds": [1, 2, 2, 3, 4, 5, 6],
      "FirstInner": {
        "asdasdasd": 1,
        "asdasd32": 132,
       "sds": [1, 2, 2, 3, 4, 5, 6],
        "SecondInner": [{
            "asd22": "asdasd",
            "asd25": 1,
            "asd27": ["asd", "asdasd", "asdasdasd", "asdasda"]
          },{
            "asd22": "asdasd",
            "asd25": 1,
            "asd27": ["asd", "asdasd", "asdasdasd", "asdasda"]
          }]
      }
    },
    "visited cities": [{
        "name": "Yaounde",
        "country": "Cameroon"
      },{
        "name": "Douala",
        "country": "Cameroon"
      },{
        "name": "Bafoussam",
        "country": "Cameroon"
      }],
    "dob": "10-10-2000",
    "room number": 23,
    "loveSummer": "false"
  }
}
```

Django Orm output: 
```python
from django.db import models
from django.contrib.postgres.fields import ArrayField


class Person(models.Model):
	id = models.AutoField(primary_key=True)
	first_name = models.CharField(max_length=32)
	last_name = models.CharField(max_length=32)
	sds = ArrayField(models.IntegerField(), blank=True)
	dob = models.CharField(max_length=32)
	room_number = models.IntegerField()
	lovesummer = models.CharField(max_length=32)


class Address(models.Model):
	id = models.AutoField(primary_key=True)
	person_id =  models.OneToOneField(Person, on_delete=models.CASCADE)
	city = models.CharField(max_length=32)
	post_code = models.CharField(max_length=32)
	sds = ArrayField(models.IntegerField(), blank=True)


class Firstinner(models.Model):
	id = models.AutoField(primary_key=True)
	address_id =  models.OneToOneField(Address, on_delete=models.CASCADE)
	asdasdasd = models.IntegerField()
	asdasd32 = models.IntegerField()
	sds = ArrayField(models.IntegerField(), blank=True)


class Secondinner(models.Model):
	id = models.AutoField(primary_key=True)
	firstinner_id = models.ForeignKey(Firstinner, on_delete=models.CASCADE)
	asd22 = models.CharField(max_length=32)
	asd25 = models.IntegerField()
	asd27 = ArrayField(models.CharField(max_length=32), blank=True)


class Visited_cities(models.Model):
	id = models.AutoField(primary_key=True)
	person_id = models.ForeignKey(Person, on_delete=models.CASCADE)
	name = models.CharField(max_length=32)
	country = models.CharField(max_length=32)


```

