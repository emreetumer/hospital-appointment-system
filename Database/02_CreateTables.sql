-- =============================================
-- Create Tables
-- =============================================

USE HospitalAppointmentDB;
GO

-- Users Table (Base table for authentication)
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Patient', 'Doctor', 'Admin')),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL
);
GO

-- Departments Table
CREATE TABLE Departments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Doctors Table
CREATE TABLE Doctors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    DepartmentId INT NOT NULL,
    Title NVARCHAR(50) NOT NULL, -- Dr., Prof. Dr., Doç. Dr., etc.
    LicenseNumber NVARCHAR(50) NOT NULL UNIQUE,
    Biography NVARCHAR(1000) NULL,
    ExperienceYears INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Doctors_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Doctors_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);
GO

-- Patients Table
CREATE TABLE Patients (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL CHECK (Gender IN ('Male', 'Female', 'Other')),
    Address NVARCHAR(500) NULL,
    EmergencyContact NVARCHAR(100) NULL,
    BloodType NVARCHAR(5) NULL, -- A+, B+, AB+, O+, A-, B-, AB-, O-
    Allergies NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Patients_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- Doctor Schedules Table
CREATE TABLE DoctorSchedules (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DoctorId INT NOT NULL,
    DayOfWeek TINYINT NOT NULL CHECK (DayOfWeek BETWEEN 1 AND 7), -- 1=Monday, 7=Sunday
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DoctorSchedules_Doctors FOREIGN KEY (DoctorId) REFERENCES Doctors(Id) ON DELETE CASCADE
);
GO

-- Appointments Table
CREATE TABLE Appointments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled', 'Completed', 'NoShow')),
    Notes NVARCHAR(1000) NULL,
    CancellationReason NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Appointments_Patients FOREIGN KEY (PatientId) REFERENCES Patients(Id),
    CONSTRAINT FK_Appointments_Doctors FOREIGN KEY (DoctorId) REFERENCES Doctors(Id)
);
GO

-- Medical Records Table
CREATE TABLE MedicalRecords (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentId INT NOT NULL,
    Diagnosis NVARCHAR(1000) NULL,
    Treatment NVARCHAR(1000) NULL,
    Prescription NVARCHAR(1000) NULL,
    Notes NVARCHAR(2000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_MedicalRecords_Appointments FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id) ON DELETE CASCADE
);
GO

PRINT 'All tables created successfully!';
