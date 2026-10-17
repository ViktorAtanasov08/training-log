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

        if (!ModelState.IsValid)
        {
            return View(entry);
        }

        entry.Exercise = entry.Exercise.Trim();
        entry.Date = entry.Date.Date;
        log.Add(entry);
        return RedirectToAction(nameof(Index));
    }
}
