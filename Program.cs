using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
// ...existing code...

class Program
{
    static void Main(string[] args)
    {
        // ...existing code...

        int objectCount = 1000;
        var crudService = new CrudService(); // Ваш CRUD сервіс
        var objects = new List<MyObject>(objectCount);

        // Паралельне створення об'єктів
        Parallel.For(0, objectCount, i =>
        {
            var obj = MyObject.CreateNew(); // Ваш метод створення нового об'єкта
            lock (objects)
            {
                objects.Add(obj);
            }
        });

        // Аналіз цифрових властивостей
        var numericValues = objects.Select(o => o.NumericProperty).ToList(); // Замість NumericProperty використайте вашу властивість

        double min = numericValues.Min();
        double max = numericValues.Max();
        double avg = numericValues.Average();

        Console.WriteLine($"Min: {min}");
        Console.WriteLine($"Max: {max}");
        Console.WriteLine($"Avg: {avg}");

        // Збереження у файл
        string filePath = "objects.json";
        File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(objects));

        Console.WriteLine($"Collection saved to {filePath}");

        // ...existing code...
    }
}

