using BenchmarkDotNet.Attributes;

namespace PostgresClientBenchmarks;

public class SingleUpdate
{
    private int _postfix;
    private PostgresDapper _postgresDapper;
    private PostgresEF _postgresEF;
    private PostgresNpgsql _postgresNpgsql;

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
    public async Task DapperUpdate()
    {
        _postfix = (_postfix + 1) % 10000;
        await _postgresDapper.UpdateLastNameById(100, $"Smith{_postfix}").ConfigureAwait(false);
    }

    [Benchmark]
    public async Task NpgsqlUpdate()
    {
        _postfix = (_postfix + 1) % 10000;
        await _postgresNpgsql.UpdateLastNameById(100, $"Smith{_postfix}").ConfigureAwait(false);
    }


    [Benchmark]
    public async Task EFUpdate()
    {
        _postfix = (_postfix + 1) % 10000;
        await _postgresEF.UpdateLastNameById(100, $"Smith{_postfix}").ConfigureAwait(false);
    }

    [Benchmark]
    public async Task EFUpdateWithExecuteUpdate()
    {
        _postfix = (_postfix + 1) % 10000;
        await _postgresEF.UpdateLastNameByIdWithExecuteUpdate(100, $"Smith{_postfix}").ConfigureAwait(false);
    }
}
