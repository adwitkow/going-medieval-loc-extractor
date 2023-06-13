// See https://aka.ms/new-console-template for more information
using GoingMedievalLocExtractor;
using Newtonsoft.Json;

if (args.Length != 1)
{
    Console.WriteLine("Proper usage:");
    Console.WriteLine("GoingMedievalLocExtractor.exe <source path>");
    Console.WriteLine();
    Console.WriteLine("Where <source path> is the location of I2Languages.asset file.");
    return -1;
}

var path = args[0];

var reader = new LocalizationReader();
var dictionary = reader.Read(path);

var json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
File.WriteAllText("output.json", json);

Console.WriteLine("output.json written successfully");
return 0;