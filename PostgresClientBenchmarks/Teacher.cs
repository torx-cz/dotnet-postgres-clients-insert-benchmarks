using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PostgresClientBenchmarks;

public class SchoolContext : DbContext
{
    public DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(BenchmarkSettings.DatabaseConnectionString); //.LogTo(Console.WriteLine, LogLevel.Information);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>(e => e.ToTable("teachers"));
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasColumnName("first_name");
            entity.Property(e => e.LastName).IsRequired().HasColumnName("last_name");
            entity.Property(e => e.Subject).IsRequired().HasColumnName("subject");
            entity.Property(e => e.Salary).HasColumnName("salary");
        });

        base.OnModelCreating(modelBuilder);
    }
}

//[Table(Teacher.TableName)]
public class Teacher : DbContext
{
    public const string TableName = "teachers";
    private static Random _rnd = new();

    public Teacher(int id, string firstName, string lastName, string subject, int salary)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Subject = subject;
        Salary = salary;
    }


    public Teacher(string firstName, string lastName, string subject, int salary)
    {
        FirstName = firstName;
        LastName = lastName;
        Subject = subject;
        Salary = salary;
    }

    //[System.ComponentModel.DataAnnotations.Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; internal set; } = default;

    //[Column("first_name")]
    public string FirstName { get; internal set; }

    //[Column("last_name")]
    public string LastName { get; internal set; }

    //[Column("subject")]
    public string Subject { get; internal set; }

    //[Column("salary")]
    public int Salary { get; internal set; }

    public static Teacher GetRandomTeacher()
    {
        var n = _rnd.Next(1000);
        return new Teacher("N" + n, "L" + n, "S" + n, n);
    }
}
