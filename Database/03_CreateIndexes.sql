-- =============================================
-- Create Indexes for Performance
-- =============================================

USE HospitalAppointmentDB;
GO

-- Users Indexes
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Role ON Users(Role);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
GO

-- Doctors Indexes
CREATE INDEX IX_Doctors_DepartmentId ON Doctors(DepartmentId);
CREATE INDEX IX_Doctors_IsActive ON Doctors(IsActive);
GO

-- Appointments Indexes
CREATE INDEX IX_Appointments_PatientId ON Appointments(PatientId);
CREATE INDEX IX_Appointments_DoctorId ON Appointments(DoctorId);
CREATE INDEX IX_Appointments_AppointmentDate ON Appointments(AppointmentDate);
CREATE INDEX IX_Appointments_Status ON Appointments(Status);
CREATE INDEX IX_Appointments_DoctorId_AppointmentDate ON Appointments(DoctorId, AppointmentDate);
GO

-- DoctorSchedules Indexes
CREATE INDEX IX_DoctorSchedules_DoctorId ON DoctorSchedules(DoctorId);
CREATE INDEX IX_DoctorSchedules_DayOfWeek ON DoctorSchedules(DayOfWeek);
GO

PRINT 'All indexes created successfully!';
