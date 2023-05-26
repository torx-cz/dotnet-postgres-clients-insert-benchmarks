# .NET -  Postgres Client Insert Benchmark Project

This project aims to compare the performance of different PostgreSQL clients in dotnet, in terms of inserting data into a PostgreSQL database. 
The benchmark will focus on three popular .NET libraries: Entity Framework, and Dapper, Raw SQL. 
By measuring the time it takes for each client to insert a certain amount of data, 
we can gain insights into their relative performance and make informed decisions when choosing a PostgreSQL client for our projects.

## Prerequisites

Before running the benchmark, ensure that the following prerequisites are met:

- .NET SDK (**dotnet-sdk 7**) installed on your machine.
- A running PostgreSQL server with a database and appropriate credentials to access it.

## Setup

Follow these steps to set up the benchmark project:
1. Clone the repository:
```shell
git clone https://github.com/torx-cz/dotnet-postgres-clients-insert-benchmarks.git
```

2. Navigate to the project directory:

```shell
cd dotnet-postgres-client-benchmark
```

3.  Install the required **dotnet-sdk 7**:
```shell
# if using `nix-shell` use:
nix-shell

# else:
# ... other way of installing dotnet-sdk
```

4. Run PostgreSQL instance:
```shell
# you can use postgres in docker like below:
docker run --rm -p 127.0.0.1:5432:5432/tcp --name testdb_postgres \
    -e POSTGRES_USER=postgres \
    -e POSTGRES_PASSWORD=p0stgres \
    -e POSTGRES_DB=testdb \
    postgres:14
```


### Running the Benchmark

To run the benchmark, execute the following command:
```shell
dotnet run --configuration Release --project PostgresClientBenchmarks
```

The benchmark will run for each client, and the results will be displayed in the console. It will measure the time taken to insert a predefined number of records into the PostgreSQL database using each client. By default, the benchmark inserts 10,000 records. However, you can adjust this number by modifying the RecordsToInsert constant in the Program.cs file.
Interpreting the Results

After running the benchmark, the console will display the time taken by each client to perform the insert operation. This information allows you to compare the performance of the different clients.

It is important to note that the results may vary depending on various factors such as hardware, network latency, database configuration, and the complexity of the insert statements. Therefore, it is recommended to run the benchmark on your target environment and adjust the parameters as needed.

### Contributing
Contributions to this benchmark project are welcome. If you encounter any issues, have suggestions for improvement, or would like to add support for additional PostgreSQL clients, please open an issue or submit a pull request on the project's GitHub repository.

### Acknowledgments
This benchmark project is forked from https://github.com/michaelscodingspot/PostgresClientBenchmarks and modify to my need to compare the performance of different PostgreSQL clients in .NET. Special thanks to Michael Shpilt and his blog post: https://michaelscodingspot.com/npgsql-dapper-efcore-performance/.
