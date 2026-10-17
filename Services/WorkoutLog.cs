using TrainingLog.Models;

namespace TrainingLog.Services;

// Temporary storage for the first milestone. SQL storage comes next.
public class WorkoutLog
{
    private readonly List<WorkoutEntry> entries = new();
    private readonly object sync = new();
    private int nextId = 1;

    public IReadOnlyList<WorkoutEntry> GetAll()
    {
        lock (sync)
        {
            return entries.OrderByDescending(entry => entry.Date)
                .ThenByDescending(entry => entry.Id).ToArray();
        }
    }

    public void Add(WorkoutEntry entry)
    {
        lock (sync)
        {
            entry.Id = nextId++;
            entries.Add(entry);
        }
    }
}
