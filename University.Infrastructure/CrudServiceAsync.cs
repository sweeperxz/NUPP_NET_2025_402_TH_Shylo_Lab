namespace University.Infrastructure;

using System.Text.Json;

public class CrudServiceAsync<T> : ICrudServiceAsync<T>, IEnumerable<T> where T : class
{
    private readonly IRepository<T> _repository;
    private readonly string _backupFilePath;

    public CrudServiceAsync(IRepository<T> repository, string backupFilePath)
    {
        _repository = repository;
        _backupFilePath = backupFilePath;
    }

    public async Task<bool> CreateAsync(T element)
    {
        await _repository.AddAsync(element);
        return true;
    }

    public async Task<T?> ReadAsync(Guid id)
    {
        var all = await _repository.GetAllAsync();
        var prop = typeof(T).GetProperty("ExternalId");
        if (prop == null) return null;
        return all.FirstOrDefault(x => prop.GetValue(x) is Guid g && g == id);
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1) page = 1;
        if (amount < 1) amount = 10;
        var all = (await _repository.GetAllAsync()).ToList();
        return all.Skip((page - 1) * amount).Take(amount);
    }

    public async Task<bool> UpdateAsync(T element)
    {
        await _repository.Update(element);
        return true;
    }

    public async Task<bool> RemoveAsync(T element)
    {
        await _repository.Delete(element);
        return true;
    }

    public async Task<bool> SaveAsync()
    {
        try
        {
            var all = await _repository.GetAllAsync();
            var json = JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_backupFilePath, json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        // Synchronously blocking wait to fetch data for enumeration
        var task = _repository.GetAllAsync();
        task.Wait();
        return task.Result.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

