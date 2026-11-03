using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Data.Sqlite;
using TrainingLog.Models;

namespace TrainingLog.Services;

// One connection per operation; SQLite owns IDs and commits writes atomically.
public sealed class WorkoutLog
{
    private readonly string connectionString;

    public WorkoutLog(string databasePath)
    {
        var path = Path.GetFullPath(databasePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        connectionString = new SqliteConnectionStringBuilder {
            DataSource = path, Pooling = false, DefaultTimeout = 10
        }.ToString();
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Workouts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Exercise TEXT NOT NULL CHECK(length(trim(Exercise)) BETWEEN 1 AND 80),
                Date TEXT NOT NULL,
                Sets INTEGER NOT NULL CHECK(Sets BETWEEN 1 AND 20),
                Reps INTEGER NOT NULL CHECK(Reps BETWEEN 1 AND 100),
                WeightHundredths INTEGER NOT NULL CHECK(WeightHundredths BETWEEN 0 AND 100000)
            );
            CREATE INDEX IF NOT EXISTS IX_Workouts_Date ON Workouts(Date DESC, Id DESC);
            """;
        command.ExecuteNonQuery();
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    private static WorkoutEntry Read(SqliteDataReader row) => new() {
        Id = row.GetInt32(0), Exercise = row.GetString(1),
        Date = DateTime.ParseExact(row.GetString(2), "yyyy-MM-dd", CultureInfo.InvariantCulture),
        Sets = row.GetInt32(3), Reps = row.GetInt32(4), WeightKg = row.GetInt32(5) / 100m
    };

    public IReadOnlyList<WorkoutEntry> GetAll()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Exercise, Date, Sets, Reps, WeightHundredths FROM Workouts ORDER BY Date DESC, Id DESC";
        using var reader = command.ExecuteReader();
        var entries = new List<WorkoutEntry>();
        while (reader.Read()) entries.Add(Read(reader));
        return entries;
    }

    public WorkoutEntry? Find(int id)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Exercise, Date, Sets, Reps, WeightHundredths FROM Workouts WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Read(reader) : null;
    }

    private static void Bind(SqliteCommand command, WorkoutEntry entry)
    {
        Validator.ValidateObject(entry, new ValidationContext(entry), validateAllProperties: true);
        if (entry.Date.Date > DateTime.Today)
            throw new ValidationException("Choose today or an earlier date.");
        if (decimal.Round(entry.WeightKg, 2) != entry.WeightKg)
            throw new ValidationException("Use at most two decimal places for weight.");
        command.Parameters.AddWithValue("$exercise", entry.Exercise.Trim());
        command.Parameters.AddWithValue("$date", entry.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$sets", entry.Sets);
        command.Parameters.AddWithValue("$reps", entry.Reps);
        command.Parameters.AddWithValue("$weight", (int)(entry.WeightKg * 100));
    }

    public void Add(WorkoutEntry entry)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Workouts (Exercise, Date, Sets, Reps, WeightHundredths)
            VALUES ($exercise, $date, $sets, $reps, $weight) RETURNING Id;
            """;
        Bind(command, entry);
        entry.Id = Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Update(WorkoutEntry entry)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Workouts SET Exercise=$exercise, Date=$date, Sets=$sets,
                Reps=$reps, WeightHundredths=$weight WHERE Id=$id;
            """;
        Bind(command, entry);
        command.Parameters.AddWithValue("$id", entry.Id);
        return command.ExecuteNonQuery() == 1;
    }

    public bool Delete(int id)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Workouts WHERE Id=$id";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteNonQuery() == 1;
    }
}
