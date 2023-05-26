using BenchmarkDotNet.Attributes;

namespace PostgresClientBenchmarks;

public class Select
{
    private PostgresDapper _postgresDapper;
    private PostgresEF _postgresEF;

    private PostgresNpgsql _postgresNpgsql;
    // private Postgres _postgres;

    [GlobalSetup]
    public async Task Setup()
    {
        _postgresNpgsql = new PostgresNpgsql();
        await _postgresNpgsql.CreateTableAsync();
        await _postgresNpgsql.CreateIndexOnSubject();
        _postgresDapper = new PostgresDapper(_postgresNpgsql.GetConnection());
        _postgresEF = new PostgresEF();

        var items = 10_000;
        for (var i = 0; i < items; i++)
        {
            var item = Teacher.GetRandomTeacher();
            item.Id = i;
            _postgresNpgsql.InsertSync(item);
        }
    }


    [Benchmark]
    public async Task<int> NpgsqlRead()
    {
        return (await _postgresNpgsql.GetBySubject("S5").ConfigureAwait(false)).Count();
    }


    [Benchmark]
    public async Task<int> DapperRead()
    {
        return (await _postgresDapper.GetBySubject("S5").ConfigureAwait(false)).Count();
    }

    [Benchmark]
    public async Task<int> EFRead()
    {
        return (await _postgresEF.GetBySubject("S5").ConfigureAwait(false)).Count();
    }
}
