using System.ComponentModel.DataAnnotations;
using TrainingLog.Models;
using TrainingLog.Services;

var directory = Path.Combine(Path.GetTempPath(), "training-checks-" + Guid.NewGuid());
Directory.CreateDirectory(directory);
var path = Path.Combine(directory, "journal.db");
var count = 0;
void Check(bool condition, string label)
{
    if (!condition) throw new Exception("FAILED: " + label);
    Console.WriteLine("PASS: " + label);
    count++;
}
WorkoutEntry Entry(string name = "Bench") => new() {
    Exercise = name, Date = DateTime.Today, Sets = 3, Reps = 5, WeightKg = 82.55m
};
try
{
    var log = new WorkoutLog(path);
    var entry = Entry("  Bench  ");
    log.Add(entry);
    var reopened = new WorkoutLog(path);
    Check(reopened.Find(entry.Id)?.Exercise == "Bench", "Create survives reopening and trims exercise");
    Check(reopened.Find(entry.Id)?.WeightKg == 82.55m, "Decimal weight round-trips exactly");
    entry.Exercise = "O'Brien's press";
    entry.WeightKg = 0;
    Check(reopened.Update(entry), "Update finds existing record");
    Check(new WorkoutLog(path).Find(entry.Id)?.Exercise == "O'Brien's press", "Parameterized update persists quotes");
    Check(log.Find(entry.Id)?.WeightKg == 0, "Zero weight persists");
    var invalid = Entry(); invalid.Sets = 0;
    try { log.Add(invalid); throw new Exception("Invalid sets accepted"); }
    catch (ValidationException) { Check(log.GetAll().Count == 1, "Invalid create leaves database unchanged"); }
    invalid = Entry(); invalid.Id = entry.Id; invalid.WeightKg = 1.234m;
    try { log.Update(invalid); throw new Exception("Excess precision accepted"); }
    catch (ValidationException) { Check(log.Find(entry.Id)?.WeightKg == 0, "Invalid update preserves original"); }
    Check(!log.Delete(999) && log.Find(999) is null, "Unknown IDs do not affect records");
    Check(log.Delete(entry.Id) && new WorkoutLog(path).Find(entry.Id) is null, "Delete survives reopening");
    var instances = Enumerable.Range(0, 8).Select(_ => new WorkoutLog(path)).ToArray();
    await Task.WhenAll(instances.Select((instance, i) => Task.Run(() => instance.Add(Entry("Parallel " + i)))));
    Check(log.GetAll().Count == 8 && log.GetAll().Select(e => e.Id).Distinct().Count() == 8,
        "Separate connections retain concurrent inserts with unique IDs");
    Check(log.GetAll().All(e => e.Id > entry.Id), "Deleted IDs are not reused");
    Console.WriteLine($"{count} storage checks passed.");
}
finally { Directory.Delete(directory, recursive: true); }
