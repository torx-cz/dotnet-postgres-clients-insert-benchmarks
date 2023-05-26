using Npgsql;
using NpgsqlTypes;
using PostgresClientBenchmarks;

public class PostgresNpgsql
{
    private readonly NpgsqlCommand _cmd;
    private readonly NpgsqlConnection _connection;

    public PostgresNpgsql()
    {
        _connection = new NpgsqlConnection(BenchmarkSettings.DatabaseConnectionString);
        _connection.Open();
        _cmd = new NpgsqlCommand();
        _cmd.Connection = _connection;
    }

    public async Task CreateTableAsync()
    {
        _cmd.CommandText = "DROP TABLE IF EXISTS teachers";
        await _cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        _cmd.CommandText = "CREATE TABLE teachers (id SERIAL PRIMARY KEY," +
                           "first_name VARCHAR(255)," +
                           "last_name VARCHAR(255)," +
                           "subject VARCHAR(255)," +
                           "salary INT)";
        await _cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task CreateIndexOnSubject()
    {
        var tableName = "teachers";
        _cmd.CommandText = $"CREATE INDEX idx_sessionSubject{tableName} ON {tableName}(subject)";
        await _cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public void CreateTable()
    {
        _cmd.CommandText = "DROP TABLE IF EXISTS teachers";
        _cmd.ExecuteNonQuery();
        _cmd.CommandText = "CREATE TABLE teachers (id SERIAL PRIMARY KEY," +
                           "first_name VARCHAR(255)," +
                           "last_name VARCHAR(255)," +
                           "subject VARCHAR(255)," +
                           "salary INT)";
        _cmd.ExecuteNonQuery();
    }

    public async Task Insert(Teacher teacher)
    {
        _cmd.CommandText =
            "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";
        _cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
        _cmd.Parameters.AddWithValue("lastName", teacher.LastName);
        _cmd.Parameters.AddWithValue("subject", teacher.Subject);
        _cmd.Parameters.AddWithValue("salary", teacher.Salary);
        await _cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public void InsertSync(Teacher teacher)
    {
        // or
        _cmd.CommandText =
            "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";
        _cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
        _cmd.Parameters.AddWithValue("lastName", teacher.LastName);
        _cmd.Parameters.AddWithValue("subject", teacher.Subject);
        _cmd.Parameters.AddWithValue("salary", teacher.Salary);
        _cmd.ExecuteNonQuery();
        _cmd.Parameters.Clear();
    }

    //public void InsertSyncWithId(Teacher teacher)
    //{
    //	// or
    //	cmd.CommandText = $"INSERT INTO teachers (id, first_name, last_name, subject, salary) VALUES (@id, @firstName, @lastName, @subject, @salary)";
    //	cmd.Parameters.AddWithValue("id", teacher.Id);
    //	cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
    //	cmd.Parameters.AddWithValue("lastName", teacher.LastName);
    //	cmd.Parameters.AddWithValue("subject", teacher.Subject);
    //	cmd.Parameters.AddWithValue("salary", teacher.Salary);
    //	cmd.ExecuteNonQuery();
    //}

    public void InsertSyncRawSQL(Teacher teacher)
    {
        _cmd.CommandText =
            $"INSERT INTO teachers (first_name, last_name, subject, salary) VALUES ('{teacher.FirstName}', '{teacher.LastName}', '{teacher.Subject}', {teacher.Salary})";
        _cmd.ExecuteNonQuery();
    }

    public async Task DeleteById(int id)
    {
        _cmd.CommandText = $"DELETE FROM teachers WHERE id = {id}";
        await _cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task UpdateById(int id, Teacher newValues)
    {
        _cmd.CommandText = "UPDATE teachers" +
                           $" SET first_name='{newValues.FirstName}'," +
                           $"last_name='{newValues.LastName}'," +
                           $"subject='{newValues.Subject}'," +
                           $"salary={newValues.Salary}" +
                           $" WHERE id = {id}";
        await _cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task UpdateLastNameById(int id, string newLastName)
    {
        _cmd.CommandText = $"UPDATE {Teacher.TableName} SET last_name='{newLastName}' WHERE id = {id}";
        await _cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<Teacher>> GetBySubject(string subject)
    {
        _cmd.CommandText = $"SELECT * FROM teachers WHERE subject='{subject}'";
        var reader = await _cmd.ExecuteReaderAsync().ConfigureAwait(false);
        var result = new List<Teacher>();
        while (await reader.ReadAsync().ConfigureAwait(false))
            result.Add(new Teacher(
                (int) reader[0],
                reader[1] as string,
                reader[2] as string,
                reader[3] as string,
                (int) reader[4]));
        await reader.CloseAsync().ConfigureAwait(false);
        return result;
    }

    public IEnumerable<Teacher> GetBySubjectSync(string subject)
    {
        _cmd.CommandText = $"SELECT * FROM teachers WHERE subject='{subject}'";
        var reader = _cmd.ExecuteReader();
        var result = new List<Teacher>();
        while (reader.Read())
            result.Add(new Teacher(
                (int) reader["id"],
                reader[1] as string, // column index can be used
                reader.GetString(2), // another syntax option
                reader["subject"] as string,
                (int) reader["salary"]));
        reader.Close();
        return result;
    }

    internal NpgsqlConnection GetConnection()
    {
        return _connection;
    }

    public void BulkInsertRegular(IEnumerable<Teacher> teachers)
    {
        // Create a NpgsqlCommand object to execute the query
        _cmd.Parameters.Clear();
        _cmd.CommandText =
            "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";

        foreach (var teacher in teachers)
        {
            // Add the parameters to the command object
            _cmd.Parameters.AddWithValue("firstName", teacher.FirstName);
            _cmd.Parameters.AddWithValue("lastName", teacher.LastName);
            _cmd.Parameters.AddWithValue("subject", teacher.Subject);
            _cmd.Parameters.AddWithValue("salary", teacher.Salary);

            // Add more parameters and execute the command multiple times to insert multiple items in a batch
            _cmd.ExecuteNonQuery();
            _cmd.Parameters.Clear();
        }
    }

    public void BulkInsertRegular2(IEnumerable<Teacher> teachers)
    {
        _cmd.CommandText = "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES " +
                           string.Join(',',
                               teachers.Select(t => $"('{t.FirstName}','{t.LastName}','{t.Subject}',{t.Salary})"));

        _cmd.ExecuteNonQuery();
    }

    public void BulkInsertInTransaction(IEnumerable<Teacher> teachers)
    {
        using var transaction = _connection.BeginTransaction();

        var sql =
            "INSERT INTO teachers (first_name, last_name, subject, salary) VALUES (@firstName, @lastName, @subject, @salary)";

        // Create a NpgsqlCommand object to execute the query
        using var command = new NpgsqlCommand(sql, _connection, transaction);
        foreach (var teacher in teachers)
        {
            // Add the parameters to the command object
            command.Parameters.AddWithValue("firstName", teacher.FirstName);
            command.Parameters.AddWithValue("lastName", teacher.LastName);
            command.Parameters.AddWithValue("subject", teacher.Subject);
            command.Parameters.AddWithValue("salary", teacher.Salary);

            // Add more parameters and execute the command multiple times to insert multiple items in a batch
            command.ExecuteNonQuery();

            // Clear the parameters for the next batch
            command.Parameters.Clear();
        }

        transaction.Commit();
    }

    // bulk insert items with Npgsql
    public async Task BulkInsertBinaryImporter(IEnumerable<Teacher> teachers)
    {
        using (var writer =
               _connection.BeginBinaryImport(
                   "COPY teachers (first_name, last_name, subject, salary) FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var teacher in teachers)
            {
                await writer.StartRowAsync().ConfigureAwait(false);
                await writer.WriteAsync(teacher.FirstName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await writer.WriteAsync(teacher.LastName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await writer.WriteAsync(teacher.Subject, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await writer.WriteAsync(teacher.Salary, NpgsqlDbType.Integer).ConfigureAwait(false);
            }

            await writer.CompleteAsync().ConfigureAwait(false);
        }
    }

    public async Task BulkInsertBinaryFormatterAndTransaction(IEnumerable<Teacher> teachers)
    {
        using var transaction = _connection.BeginTransaction();


        using (var writer =
               _connection.BeginBinaryImport(
                   "COPY teachers (first_name, last_name, subject, salary) FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var teacher in teachers)
            {
                await writer.StartRowAsync().ConfigureAwait(false);
                await writer.WriteAsync(teacher.FirstName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await writer.WriteAsync(teacher.LastName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await writer.WriteAsync(teacher.Subject, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await writer.WriteAsync(teacher.Salary, NpgsqlDbType.Integer).ConfigureAwait(false);
            }

            await writer.CompleteAsync().ConfigureAwait(false);
        }

        // Commit the transaction
        transaction.Commit();
    }
}
