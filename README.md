# TrainingLog

[![Build and smoke checks](https://github.com/ViktorAtanasov08/training-log/actions/workflows/build.yml/badge.svg)](https://github.com/ViktorAtanasov08/training-log/actions/workflows/build.yml)

**Status: in progress - initial MVC milestone.**

A small workout journal built with C# and ASP.NET Core MVC. This project develops in milestones to practise forms, validation and database-backed applications around strength training.

## Current milestone

- Log an exercise with a date, sets, reps and added weight.
- View entries with the newest training dates first.
- Reject empty exercise names, invalid numbers and future dates on the server.
- Support decimal weights and 0 kg for exercises without added weight.

Storage is currently in memory. Records are shared within this local app instance and reset when the app restarts. SQL Server integration, accounts and charts are not implemented yet.

## Run locally

Install the .NET 10 SDK, then open a terminal in this folder:

    dotnet restore
    dotnet run --launch-profile http

Open http://localhost:5084. Visual Studio users can open TrainingLog.sln and run the http profile.

## Project structure

| File | Responsibility |
| --- | --- |
| Models/WorkoutEntry.cs | Exercise data and validation rules |
| Controllers/WorkoutsController.cs | Handle the journal and exercise form |
| Services/WorkoutLog.cs | Temporary storage, to be replaced by a SQL repository |
| Views/Workouts/ | Razor views for listing and adding exercises |
| database/schema.sql | Draft SQL Server schema for the next milestone |

The initial app uses the .NET shared framework without extra NuGet packages. No database setup is required to run this milestone. The SQL schema is a draft and has not yet been executed or connected to the app.

## Verification

The GitHub Actions workflow builds the app with warnings treated as errors and runs HTTP smoke checks against a fresh instance. [The first run passed](https://github.com/ViktorAtanasov08/training-log/actions/runs/39038036055).

Checks cover page and stylesheet loading, saved entries, decimal and zero weights, rejection of five invalid inputs, HTML escaping and CSRF protection. Browser layout and the planned SQL integration are outside these checks.

To run the same checks yourself, start a fresh app instance using the `http` launch profile, then run `python scripts/smoke.py` in another terminal. The checks add temporary sample entries.

## Roadmap

- [x] Add and list exercises with server validation
- [ ] Connect SQL Server storage
- [ ] Edit and delete entries
- [ ] Track personal records
- [ ] Add a progress chart
- [ ] Add focused tests for storage and personal-record rules

## Learning notes

See docs/day-01.md for the first lesson and Git workflow. The initial scaffold was prepared with AI assistance. Each milestone should be reviewed, run and understood before it is presented as completed portfolio work.
