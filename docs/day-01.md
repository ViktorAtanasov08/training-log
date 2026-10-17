# Day 1: Follow one workout entry through the app

Goal: understand how input travels from a form to C# and back to the journal. SQL storage comes in the next milestone.

## Run and explore

1. Open `TrainingLog.sln` in Visual Studio with the .NET 10 SDK installed.
2. Select the `http` profile and run with Ctrl+F5.
3. Choose **Log an exercise** and save Bench press: 3 sets, 5 reps, 100 kg.
4. Try an empty exercise name and 0 sets. The server must reject both.
5. Restart the app. Entries disappear because storage is currently in memory.

## Follow the request

1. `Views/Workouts/Create.cshtml` renders the form.
2. Submitting it sends a POST request to `Workouts/Create`.
3. MVC binds the form fields to a `WorkoutEntry`.
4. `ModelState.IsValid` checks binding errors and the model's validation attributes. The controller also rejects future dates.
5. `WorkoutLog.Add` puts a valid entry in the temporary list.
6. `RedirectToAction` sends the browser back to the journal.
7. `Views/Workouts/Index.cshtml` renders the saved entries.

## Be ready to explain

- Why is weight a `decimal`?
- Why validate in C# when the HTML fields already have `min` and `max`?
- Why do the GET and POST actions have different responsibilities?
- What does `OrderByDescending` change in the journal?
- What happens to entries when the application restarts?
- Why does the POST form include an antiforgery token?

## Your first change

Add a second exercise using your own training data. Log an older date and explain why it appears below newer entries. Then read the validation rules and choose one useful change to implement and verify.

## Git and GitHub

A **commit** records a change locally. A **push** publishes local commits to GitHub. Commit and push completed changes as you work.

The starter has already been published. For a local copy that shares its Git history, clone it into a new folder:

```sh
git clone https://github.com/ViktorAtanasov08/training-log.git TrainingLog-portfolio
cd TrainingLog-portfolio
git log --oneline
git status
```

The repository is named `training-log`. The clone above shares its published commit history.

The earlier ZIP/local starter has a separate Git history. Keep it as a reference and use the clone above for future pushes.

After making and checking a change:

```sh
git diff
git add path/to/the/changed-file
git diff --cached
git commit -m "Describe the completed change"
git push
```

If Git requests sign-in, use the normal Git Credential Manager browser flow.

## Automated checks

The Actions workflow builds the app, starts a fresh instance and runs `scripts/smoke.py`. It checks actual HTTP behaviour, including invalid submissions and CSRF protection.

To run those checks yourself, start a fresh app with the `http` profile, then run `python scripts/smoke.py` in another terminal. They add sample entries to temporary storage.

The first scaffold and checks were prepared with AI assistance. Run the app and understand the request flow before presenting this milestone in an interview.
