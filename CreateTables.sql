-- Pedros Cantina – oprettelse af tabeller (opgave 3)
CREATE DATABASE PedroCastina;
GO
USE PedroCastina;
GO

CREATE TABLE Medarbejder (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Navn NVARCHAR(50) NOT NULL,
    Telefon NVARCHAR(50) NOT NULL,
    ErLeder BIT NOT NULL DEFAULT 0
);

CREATE TABLE Månedsplan (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Måned INT NOT NULL CHECK (Måned >= 1 AND Måned <= 12),
    År INT NOT NULL,
    UNIQUE (Måned, År)                  -- kun én plan pr. måned
);

CREATE TABLE Vagt (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MånedsplanId INT NOT NULL FOREIGN KEY REFERENCES Månedsplan(Id),
    Dato DATE NOT NULL,
    StartTid TIME NOT NULL,
    SlutTid TIME NOT NULL,
    UNIQUE (Dato, StartTid),            -- kun én vagt pr. periode pr. dag
    CHECK ((StartTid = '09:00' AND SlutTid = '14:00')
        OR (StartTid = '14:00' AND SlutTid = '19:00'))
);

CREATE TABLE VagtMedarbejder (
    VagtId INT NOT NULL FOREIGN KEY REFERENCES Vagt(Id),
    MedarbejderId INT NOT NULL FOREIGN KEY REFERENCES Medarbejder(Id),
    PRIMARY KEY (VagtId, MedarbejderId) -- samme medarbejder kan ikke stå på samme vagt to gange
);
