using Dapper;
using Npgsql;
using PostgresClientBenchmarks;

public class PostgresDapper : IDisposable
{
    private readonly NpgsqlConnection _connection;

    public PostgresDapper()
    {
        _connection = new NpgsqlConnection(BenchmarkSettings.DatabaseConnectionString);
        _connection.Open();
    }

    public PostgresDapper(NpgsqlConnection npgsqlConnection)
    {
        _connection = npgsqlConnection;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public async Task CreateTable()
    {
        await _connection.ExecuteAsync($"DROP TABLE IF EXISTS {Teacher.TableName}").ConfigureAwait(false);
        await _connection.ExecuteAsync($"CREATE TABLE {Teacher.TableName}(id SERIAL PRIMARY KEY," +
                                       "first_name VARCHAR(255), " +
                                       "last_name VARCHAR(255)," +
                                       "subject VARCHAR(255)," +
                                       "salary INT)").ConfigureAwait(false);
    }


    public async Task Insert(Teacher teacher)
    {
        var sqlCommand =
            $"INSERT INTO {Teacher.TableName} (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @Subject, @salary)";

        var queryArguments = new
        {
            firstName = teacher.FirstName,
            lastName = teacher.LastName,
            subject = teacher.Subject,
            salary = teacher.Salary
        };

        await _connection.ExecuteAsync(sqlCommand, queryArguments).ConfigureAwait(false);
    }

    public async Task DeleteById(int id)
    {
        var sqlCommand = $"DELETE FROM {Teacher.TableName} WHERE id = {id}";
        await _connection.ExecuteAsync(sqlCommand).ConfigureAwait(false);
    }

    public async Task UpdateById(int id, Teacher newValues)
    {
        var sqlCommand = $"UPDATE {Teacher.TableName}" +
                         $" SET first_name='{newValues.FirstName}',last_name='{newValues.LastName}',subject='{newValues.Subject}',salary={newValues.Salary}" +
                         $" WHERE id = {id}";
        await _connection.ExecuteAsync(sqlCommand).ConfigureAwait(false);
    }

    public async Task UpdateLastNameById(int id, string newLastName)
    {
        var sqlCommand = $"UPDATE {Teacher.TableName} SET last_name='{newLastName}' WHERE id = {id}";
        await _connection.ExecuteAsync(sqlCommand).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Teacher>> GetBySubject(string subject)
    {
        var commandText = $"SELECT * FROM {Teacher.TableName} WHERE subject='{subject}'";
        var teachers = await _connection.QueryAsync<Teacher>(commandText).ConfigureAwait(false);

        return teachers;
    }
}
