using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using PostgresClientBenchmarks;

public class PostgresEF
{
    private readonly SchoolContext _db;

    public PostgresEF()
    {
        _db = new SchoolContext();
        //_db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    ~PostgresEF()
    {
        _db.Dispose();
    }

    public async Task InsertAsync(Teacher teacher)
    {
        _db.Teachers.Update(teacher);
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task InsertRangeAsync(IEnumerable<Teacher> teachers)
    {
        _db.Teachers.UpdateRange(teachers);
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task BulkInsertAsyn(IEnumerable<Teacher> teachers)
    {
        await _db.BulkInsertAsync(teachers);
    }

    public async Task<Teacher> GetById(int id)
    {
        using (var db = new SchoolContext())
        {
            return await db.Teachers.FindAsync(id).ConfigureAwait(false);
        }
    }

    public async Task<List<Teacher>> GetBySubject(string subject)
    {
        return await _db.Teachers.Where(t => t.Subject == subject).ToListAsync().ConfigureAwait(false);
    }

    public async Task UpdateAllValues(int id, Teacher teacher)
    {
        var t = await _db.Teachers.FindAsync(id).ConfigureAwait(false);

        teacher.Id = t.Id;
        _db.Entry(t).CurrentValues.SetValues(teacher);

        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateSalary(int id, int newSalary)
    {
        var t = await _db.Teachers.FindAsync(id).ConfigureAwait(false);
        t.Salary = newSalary;
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateLastNameById(int id, string newLastName)
    {
        var t = await _db.Teachers.FindAsync(id).ConfigureAwait(false);
        t.LastName = newLastName;
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateLastNameByIdWithExecuteUpdate(int id, string newLastName)
    {
        var t = await _db.Teachers
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(t => t.SetProperty(t => t.LastName, t => newLastName))
            .ConfigureAwait(false);
    }
}
