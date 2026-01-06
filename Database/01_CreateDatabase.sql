-- =============================================
-- Hospital Appointment System Database Creation
-- =============================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HospitalAppointmentDB')
BEGIN
    CREATE DATABASE HospitalAppointmentDB;
END
GO

USE HospitalAppointmentDB;
GO

PRINT 'Database HospitalAppointmentDB created successfully!';
