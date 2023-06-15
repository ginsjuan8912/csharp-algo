// See https://aka.ms/new-console-template for more information
using PropertyBinder.ChangeListener;
using PropertyBinder.Model;

Dictionary<int, ItemDto> db = new();


//Initialize some ItemDtos for testing
ItemDto item1 = new ItemDto(1,"Juan") {};
item1.ListenForChanges();
//Save this item in the db
db.Add(item1.Id, item1);

Console.WriteLine("Added element");

//Now retrieve this object from the db
var retrievedObj = db[item1.Id];
//Update the value of the name property
//Here is where the interceptor should listen for the changes

retrievedObj.Name = "AnotherValue";


var updatedProperties = retrievedObj.GetChanges();
Console.ReadKey();





