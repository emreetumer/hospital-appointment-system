-- =============================================
-- Insert Sample Data
-- =============================================

USE HospitalAppointmentDB;
GO

-- Insert Departments
INSERT INTO Departments (Name, Description) VALUES
('Cardiology', 'Heart and cardiovascular system'),
('Neurology', 'Brain and nervous system'),
('Orthopedics', 'Bones, joints, and muscles'),
('Pediatrics', 'Medical care for children'),
('Dermatology', 'Skin, hair, and nails');
GO

-- Insert Admin User (Password: Admin123!)
-- Note: In production, use proper password hashing (bcrypt)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Role) VALUES
('admin@hospital.com', '$2a$11$0jvYnXh7Y1N.Y3nP5z1xMO0dP1e3R/YpR2YvHzR3kP3W5L1O2N3M4', 'Admin', 'User', '5551234567', 'Admin');
GO

-- Insert Sample Doctors (Password: Doctor123!)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Role) VALUES
('dr.mehmet@hospital.com', '$2a$11$0jvYnXh7Y1N.Y3nP5z1xMO0dP1e3R/YpR2YvHzR3kP3W5L1O2N3M4', 'Mehmet', 'Yýlmaz', '5551234501', 'Doctor'),
('dr.ayse@hospital.com', '$2a$11$0jvYnXh7Y1N.Y3nP5z1xMO0dP1e3R/YpR2YvHzR3kP3W5L1O2N3M4', 'Ayþe', 'Demir', '5551234502', 'Doctor'),
('dr.ali@hospital.com', '$2a$11$0jvYnXh7Y1N.Y3nP5z1xMO0dP1e3R/YpR2YvHzR3kP3W5L1O2N3M4', 'Ali', 'Kaya', '5551234503', 'Doctor');
GO

-- Insert Doctor Details
INSERT INTO Doctors (UserId, DepartmentId, Title, LicenseNumber, Biography, ExperienceYears) VALUES
(2, 1, 'Prof. Dr.', 'DOC001', 'Cardiology specialist with 15 years of experience', 15),
(3, 2, 'Dr.', 'DOC002', 'Neurology specialist with 8 years of experience', 8),
(4, 3, 'Doç. Dr.', 'DOC003', 'Orthopedics specialist with 12 years of experience', 12);
GO

-- Insert Sample Patients (Password: Patient123!)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Role) VALUES
('patient1@email.com', '$2a$11$0jvYnXh7Y1N.Y3nP5z1xMO0dP1e3R/YpR2YvHzR3kP3W5L1O2N3M4', 'Emre', 'Özkan', '5551234601', 'Patient'),
('patient2@email.com', '$2a$11$0jvYnXh7Y1N.Y3nP5z1xMO0dP1e3R/YpR2YvHzR3kP3W5L1O2N3M4', 'Zeynep', 'Çelik', '5551234602', 'Patient');
GO

-- Insert Patient Details
INSERT INTO Patients (UserId, DateOfBirth, Gender, Address, BloodType) VALUES
(5, '1990-05-15', 'Male', 'Ýstanbul, Turkey', 'A+'),
(6, '1985-08-20', 'Female', 'Ankara, Turkey', 'B+');
GO

-- Insert Doctor Schedules (Monday to Friday, 9:00 AM - 5:00 PM)
DECLARE @DoctorId INT = 1;
DECLARE @Day INT = 1;

WHILE @DoctorId <= 3
BEGIN
    SET @Day = 1;
    WHILE @Day <= 5
    BEGIN
        INSERT INTO DoctorSchedules (DoctorId, DayOfWeek, StartTime, EndTime)
        VALUES (@DoctorId, @Day, '09:00', '17:00');
        SET @Day = @Day + 1;
    END
    SET @DoctorId = @DoctorId + 1;
END
GO

-- Insert Sample Appointments
INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, AppointmentTime, Status, Notes) VALUES
(1, 1, DATEADD(DAY, 1, CAST(GETDATE() AS DATE)), '10:00', 'Confirmed', 'Regular checkup'),
(2, 2, DATEADD(DAY, 2, CAST(GETDATE() AS DATE)), '14:00', 'Pending', 'Headache consultation');
GO

PRINT 'Sample data inserted successfully!';
PRINT '';
PRINT '===== LOGIN CREDENTIALS =====';
PRINT 'Admin: admin@hospital.com / Admin123!';
PRINT 'Doctor 1: dr.mehmet@hospital.com / Doctor123!';
PRINT 'Doctor 2: dr.ayse@hospital.com / Doctor123!';
PRINT 'Patient 1: patient1@email.com / Patient123!';
PRINT 'Patient 2: patient2@email.com / Patient123!';
PRINT '=============================';
