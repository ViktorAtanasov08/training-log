# TrainingLog

[![Build and smoke checks](https://github.com/ViktorAtanasov08/training-log/actions/workflows/build.yml/badge.svg)](https://github.com/ViktorAtanasov08/training-log/actions/workflows/build.yml)

A local workout journal built with **C#, ASP.NET Core MVC and SQLite**. Create, edit and delete exercises; records survive application restarts.

## Features

- Exercise name, date, sets, reps and weight, ordered by training date.
- Server-side validation including future dates, numeric ranges and weight precision.
- Persistent SQLite storage with parameterized queries and database constraints.
- Exact weight storage in hundredths of a kilogram, including zero for bodyweight exercises.
- Edit forms and a separate delete confirmation page; all mutations require anti-forgery tokens.

## Run

Install the .NET 10 SDK. Run from this repository:

    dotnet restore
    dotnet run --launch-profile http

Open http://localhost:5084. The app creates **data/training.db** automatically. No database server is required. Set the configuration key **Database:Path** (environment variable **Database__Path**) to use another file. The app needs write access to its database directory.

This is a **single-user local portfolio application**, with no accounts or access control. Keep it bound to localhost; do not expose it as a shared public service. Copy the database while the app is stopped to back it up. Never commit real workout data or local protection keys. Existing in-memory data from the previous version cannot be recovered after that old process has stopped.

## Verify

    dotnet build -c Release --warnaserror
    dotnet run --project tests/TrainingLog.Checks/TrainingLog.Checks.csproj -c Release
    python scripts/persistence.py

Storage checks use temporary databases and exercise reopening, exact decimals, validation, deletion and concurrent inserts. The HTTP suite starts and stops real application processes to verify create/edit/delete persistence, missing records, invalid edits and CSRF protection.

The original forms/escaping checks are in **scripts/smoke.py** and expect a running app on port 5084 with a fresh database. GitHub Actions runs both suites in isolation. Test results are available from the badge above; a passing build does not establish production readiness.

## Code map

| Path | Responsibility |
| --- | --- |
| Models/WorkoutEntry.cs | Form fields and validation |
| Controllers/WorkoutsController.cs | Create, read, edit and delete HTTP actions |
| Services/WorkoutLog.cs | SQLite initialization and parameterized CRUD |
| Views/Workouts/ | Journal, form and delete confirmation |
| tests/TrainingLog.Checks/ | Database integration checks |
| scripts/persistence.py | HTTP checks across process restarts |

The old **database/schema.sql** is an archived SQL Server design exercise, not the runtime schema. SQLite initialization lives in WorkoutLog.cs; SQL Server integration is not claimed.

## Next improvements

- Personal-record summaries and progress charts.
- Pagination for large journals.
- Account isolation before any shared deployment.
- Versioned schema migrations when the storage model changes.

## Development and learning

Built with AI assistance, including the persistence and CRUD update. This repository demonstrates the implementation; it does not establish that every part was written independently. See [the persistence walkthrough](docs/persistence.md) for design decisions, interview questions and an independent follow-up exercise. Only describe features you can explain and demonstrate in an interview.
