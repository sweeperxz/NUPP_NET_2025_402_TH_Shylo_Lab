namespace University.Infrastructure;

using Microsoft.EntityFrameworkCore;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly string _dbFilePath;

    public EfRepository(string dbFilePath)
    {
        _dbFilePath = dbFilePath;
    }

    private UniversityContext CreateContext() => new UniversityContext(_dbFilePath);

    public async Task<T?> GetByIdAsync(int id)
    {
        using var ctx = CreateContext();
        return await ctx.Set<T>().FindAsync(id).AsTask();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using var ctx = CreateContext();
        return await ctx.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        using var ctx = CreateContext();
        ctx.Set<T>().Add(entity);
        await ctx.SaveChangesAsync();
    }

    public async Task Update(T entity)
    {
        using var ctx = CreateContext();
        ctx.Set<T>().Update(entity);
        await ctx.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        using var ctx = CreateContext();
        ctx.Set<T>().Remove(entity);
        await ctx.SaveChangesAsync();
    }
}

