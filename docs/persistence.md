# Persistence walkthrough

## Follow a request

1. WorkoutsController receives a form and validates it on the server.
2. WorkoutLog binds values as SQL parameters. User input never becomes SQL syntax.
3. SQLite stores the row in data/training.db and assigns its ID.
4. The controller redirects to the journal, which reads saved rows.

Editing updates an existing ID. Deletion uses a confirmation GET followed by a protected POST. A GET never deletes data. Missing IDs return 404.

## Choices to explain

- SQLite makes the local demo easy to run. It is a relational database, but it is not SQL Server.
- Weight is stored as integer hundredths: 82.55 kg becomes 8255. Inputs with more precision are rejected rather than silently rounded.
- Each operation creates and disposes its own connection. SQLite handles write transactions and IDs; no process-local counter is used.
- Validation appears in the HTTP layer and the storage service; database constraints add a final guard.
- CREATE TABLE IF NOT EXISTS initializes a new database. It does not migrate an existing table to a changed schema.
- No accounts means no privacy boundary between users. This app is deliberately local-only in intended use.

## Demonstrate it

Create a workout, stop the app, start it again and locate the same row. Edit its weight, restart again, then delete it through the confirmation page. Run the automated checks against temporary databases.

## Independent follow-up

Implement a per-exercise heaviest-weight summary. Decide whether differently capitalized exercise names belong together; write down the rule, add a test and make a small commit describing your own change. This exercise is not completed by the current update.
