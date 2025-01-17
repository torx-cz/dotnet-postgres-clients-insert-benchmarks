using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace PostgresClientBenchmarks;

public class SingleInsert
{
    private const int iterations = 5000;
    private static int _id;
    private Teacher _item;
    private PostgresDapper _postgresDapper;
    private PostgresEF _postgresEF;

    private PostgresNpgsql _postgresNpgsql;
    // private Postgres _postgres;

    [GlobalSetup]
    public async Task Setup()
    {
        _item = Teacher.GetRandomTeacher();
        _postgresNpgsql = new PostgresNpgsql();
        await _postgresNpgsql.CreateTableAsync();

        _postgresDapper = new PostgresDapper(_postgresNpgsql.GetConnection());

        _postgresEF = new PostgresEF();
    }

    [IterationSetup]
    public void MyIterationSetup()
    {
        // Resetting table so as not to have performance issues with tables that are too big with increasing iterations
        if (_id + 1 % 1_000_000 == 0)
        {
            _postgresNpgsql.CreateTable();
            _id = 0;
        }
    }

    [Benchmark]
    public async Task NpgsqlInsertAsync()
    {
        _id++;
        //_item.Id = _id;
        await _postgresNpgsql.Insert(_item).ConfigureAwait(false);
    }

    [Benchmark]
    public void NpgsqlInsertSync()
    {
        _id++;
        //_item.Id = _id;
        _postgresNpgsql.InsertSync(_item);
    }

    [Benchmark]
    public void NpgsqlInsertRawSql()
    {
        _id++;
        //_item.Id = _id;
        _postgresNpgsql.InsertSyncRawSQL(_item);
    }

    [Benchmark]
    public async Task EfInsertAsync()
    {
        _id++;
        // _item.Id = _id;
        await _postgresEF.InsertAsync(_item).ConfigureAwait(false);
    }

    [Benchmark]
    public async Task DapperInsert()
    {
        _id++;
        //_item.Id = _id;
        await _postgresDapper.Insert(_item).ConfigureAwait(false);
    }
}
