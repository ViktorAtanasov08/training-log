using Microsoft.AspNetCore.Mvc;
using TrainingLog.Models;
using TrainingLog.Services;

namespace TrainingLog.Controllers;

public class WorkoutsController : Controller
{
    private readonly WorkoutLog log;

    public WorkoutsController(WorkoutLog log)
    {
        this.log = log;
    }

    public IActionResult Index() => View(log.GetAll());

    public IActionResult Create() => View(new WorkoutEntry());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(WorkoutEntry entry)
    {
        if (entry.Date.Date > DateTime.Today)
        {
            ModelState.AddModelError(nameof(entry.Date), "Choose today or an earlier date.");
        }

        if (decimal.Round(entry.WeightKg, 2) != entry.WeightKg)
            ModelState.AddModelError(nameof(entry.WeightKg), "Use at most two decimal places for weight.");

        if (!ModelState.IsValid)
        {
            return View(entry);
        }

        entry.Exercise = entry.Exercise.Trim();
        entry.Date = entry.Date.Date;
        log.Add(entry);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var entry = log.Find(id);
        return entry is null ? NotFound() : View("Create", entry);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, WorkoutEntry entry)
    {
        if (id != entry.Id) return BadRequest();
        if (log.Find(id) is null) return NotFound();
        if (entry.Date.Date > DateTime.Today)
            ModelState.AddModelError(nameof(entry.Date), "Choose today or an earlier date.");
        if (decimal.Round(entry.WeightKg, 2) != entry.WeightKg)
            ModelState.AddModelError(nameof(entry.WeightKg), "Use at most two decimal places for weight.");
        if (!ModelState.IsValid) return View("Create", entry);
        return log.Update(entry) ? RedirectToAction(nameof(Index)) : NotFound();
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var entry = log.Find(id);
        return entry is null ? NotFound() : View(entry);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id) =>
        log.Delete(id) ? RedirectToAction(nameof(Index)) : NotFound();
}
