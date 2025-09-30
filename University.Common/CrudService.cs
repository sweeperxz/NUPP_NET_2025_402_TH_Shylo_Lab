using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;

namespace University.Common;

public class CrudService<T> : ICrudService<T> where T : class
{
    private readonly ConcurrentDictionary<Guid, T> _elements = new();


    public void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        var json = File.ReadAllText(filePath);
        var items = JsonSerializer.Deserialize<List<T>>(json);
        if (items != null)
        {
            _elements.Clear();
            _elements.TryAdd(Guid.NewGuid(), items.First());
            foreach (var item in items.Skip(1))
            {
                _elements.TryAdd(Guid.NewGuid(), item);
            }
        }
    }

    public Task<bool> CreateAsync(T element)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            throw new InvalidOperationException("Element must have an Id property of type Guid.");

        var id = (Guid)idProperty.GetValue(element);
        return Task.FromResult(_elements.TryAdd(id, element));
    }

    public Task<T> ReadAsync(Guid id)
    {
        _elements.TryGetValue(id, out var element);
        return Task.FromResult(element);
    }

    public Task<IEnumerable<T>> ReadAllAsync()
    {
        return Task.FromResult(_elements.Values.AsEnumerable());
    }

    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        var items = _elements.Values.Skip((page - 1) * amount).Take(amount);
        return Task.FromResult(items);
    }

    public Task<bool> UpdateAsync(T element)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            throw new InvalidOperationException("Element must have an Id property of type Guid.");

        var id = (Guid)idProperty.GetValue(element);
        if (!_elements.ContainsKey(id))
            return Task.FromResult(false);

        _elements[id] = element;
        return Task.FromResult(true);
    }

    public Task<bool> RemoveAsync(T element)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            throw new InvalidOperationException("Element must have an Id property of type Guid.");

        var id = (Guid)idProperty.GetValue(element);
        return Task.FromResult(_elements.TryRemove(id, out _));
    }

    public Task<bool> SaveAsync()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_elements, options);
        File.WriteAllText("result.json", json);
        return Task.FromResult(true);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _elements.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}