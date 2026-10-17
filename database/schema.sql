-- Planned SQL Server storage. The first app milestone does not use it yet.
IF DB_ID(N'TrainingLog') IS NULL
    CREATE DATABASE TrainingLog;
GO
USE TrainingLog;
GO

IF OBJECT_ID(N'dbo.WorkoutEntries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WorkoutEntries
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Exercise NVARCHAR(80) NOT NULL,
        [Date] DATE NOT NULL,
        Sets INT NOT NULL CHECK (Sets BETWEEN 1 AND 20),
        Reps INT NOT NULL CHECK (Reps BETWEEN 1 AND 100),
        WeightKg DECIMAL(6,2) NOT NULL CHECK (WeightKg BETWEEN 0 AND 1000)
    );
END;
