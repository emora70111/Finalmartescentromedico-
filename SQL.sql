
--DROP DATABASE CONSULTORIO_VIDA_SALUD;
--ALTER DATABASE CONSULTORIO_VIDA_SALUD 
--SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

--ALTER DATABASE CONSULTORIO_VIDA_SALUD SET MULTI_USER;



CREATE DATABASE CONSULTORIO_VIDA_SALUD;
GO
USE CONSULTORIO_VIDA_SALUD;
GO

 
select * from users;
select * from Doctors;
select * from Patients;

-- Users Table
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) UNIQUE NOT NULL,
    Phone NVARCHAR(15),
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) CHECK (Role IN ('Patient', 'Doctor', 'Administrator')) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Doctors Table
CREATE TABLE Doctors (
    DoctorID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Specialty NVARCHAR(100) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Patients Table
CREATE TABLE Patients (
    PatientID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    BirthDate DATE NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Doctor Schedules Table
CREATE TABLE Schedules (
    ScheduleID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorID INT NOT NULL,
    DayOfWeek NVARCHAR(15) CHECK (DayOfWeek IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday')) NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);
select * from Schedules
-- Appointments Table
CREATE TABLE Appointments (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    Date DATE NOT NULL,
    Time TIME NOT NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Scheduled', 'Canceled', 'Completed')) NOT NULL,
    Notes NVARCHAR(MAX),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);
 
-- Notifications Table
CREATE TABLE Notifications (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    AppointmentID INT,
    Message NVARCHAR(500) NOT NULL,
    SentDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID)
);

-- Medical History Table
CREATE TABLE MedicalHistory (
    HistoryID INT IDENTITY(1,1) PRIMARY KEY,
	AppointmentId INT NOT NULL,
    ConsultationDate DATE NOT NULL,
    Notes NVARCHAR(MAX),
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId)
);

--
INSERT INTO Users (FirstName, LastName, Email, Phone, Password, Role, IsActive)
VALUES 
('Admin', 'User', 'admin@gmail.com', '121212122', '12345678', 'Administrator', 1);
