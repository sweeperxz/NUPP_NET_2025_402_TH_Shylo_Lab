using System.Text.Json;

namespace University.Common;

public class CrudService<T> : ICrudService<T> where T : class
{
    private List<T> _elements = new List<T>();

    public void Create(T element)
    {
        _elements.Add(element);
    }

    public T? Read(Guid id)
    {
        return _elements.Find(e => (e as dynamic).Id == id);
    }

    public IEnumerable<T> ReadAll()
    {
        return _elements;
    }

    public void Update(T element)
    {
        var existingElement = Read((element as dynamic).Id);
        if (existingElement != null)
        {
            var index = _elements.IndexOf(existingElement);
            _elements[index] = element;
        }
        else
        {
            throw new Exception("Element not found");
        }
    }

    public void Remove(T element)
    {
        _elements.Remove(element);
    }

    public void Save(string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_elements, options);
        File.WriteAllText(filePath, json);
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        var json = File.ReadAllText(filePath);
        var items = JsonSerializer.Deserialize<List<T>>(json);
        if (items != null)
        {
            _elements.Clear();
            _elements.AddRange(items);
        }
    }
}