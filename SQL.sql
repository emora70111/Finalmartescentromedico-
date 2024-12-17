
--DROP DATABASE CONSULTORIO_VIDA_SALUD;
--ALTER DATABASE CONSULTORIO_VIDA_SALUD 
--SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

--ALTER DATABASE CONSULTORIO_VIDA_SALUD SET MULTI_USER;



CREATE DATABASE CONSULTORIO_VIDA_SALUD;
GO
USE CONSULTORIO_VIDA_SALUD;
GO


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
    DayOfWeek INT CHECK (DayOfWeek BETWEEN 1 AND 7) NOT NULL, -- 1 = Lunes, 7 = Domingo
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);

CREATE TABLE Shifts (
    ShiftID INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleID INT NOT NULL,
	Description VARCHAR(100),
	BlockDurationMinutes INT DEFAULT 30 CHECK (BlockDurationMinutes > 0),
	FOREIGN KEY (ScheduleID) REFERENCES Schedules(ScheduleID)
);

CREATE TABLE Blocks (
    BlockID INT IDENTITY(1,1) PRIMARY KEY,
    ShiftID INT NOT NULL,
    BlockStartTime TIME NOT NULL,
    BlockEndTime TIME NOT NULL,
    FOREIGN KEY (ShiftID) REFERENCES Shifts(ShiftID),
    CHECK (BlockStartTime < BlockEndTime)
);


-- Appointments Table
CREATE TABLE Appointments (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    Date DATE NOT NULL,
    BlockID INT NOT NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Scheduled', 'Canceled', 'Completed')) NOT NULL,
    Notes NVARCHAR(MAX),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID),
	FOREIGN KEY (BlockID) REFERENCES Blocks(BlockID)

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

 
CREATE TABLE PasswordRecovery (
    PasswordRecoveryID INT IDENTITY(1,1) PRIMARY KEY,
	Code VARCHAR(100),
    Date DATETIME NOT NULL,
	UserID INT NOT NULL,
	Status BIT,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
);
--
INSERT INTO Users (FirstName, LastName, Email, Phone, Password, Role, IsActive)
VALUES 
('Admin', 'User', 'admin@gmail.com', '121212122', '12345678', 'Administrator', 1);
