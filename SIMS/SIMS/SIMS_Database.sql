/* =====================================================================
   SIMS - Student Information Management System
   Database creation script for SQL Server (run in SSMS)
   Run this whole file once. It creates the database, all tables,
   keys/constraints, and a small set of test data.

   NOTE on passwords: the seed users below use the placeholder text
   'CHANGEME' in PasswordHash. Do NOT keep plain-text passwords in the
   final system - it loses security marks. Replace these by registering
   users through the app (which hashes the password) or update the rows
   with a real hash from your C# PasswordHasher before submission.
   ===================================================================== */

IF DB_ID('SIMS') IS NULL
    CREATE DATABASE SIMS;
GO
USE SIMS;
GO

/* ---------- Core: Users (login + role) ---------- */
CREATE TABLE Users (
    UserID        INT IDENTITY(1,1) PRIMARY KEY,
    Username      NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash  NVARCHAR(255) NOT NULL,
    Email         NVARCHAR(100) NOT NULL,
    Role          NVARCHAR(20)  NOT NULL
                  CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin','Lecturer','Student')),
    IsActive      BIT           NOT NULL DEFAULT 1,
    CreatedDate   DATETIME      NOT NULL DEFAULT GETDATE()
);

/* ---------- Programmes ---------- */
CREATE TABLE Programmes (
    ProgrammeID   INT IDENTITY(1,1) PRIMARY KEY,
    ProgrammeCode NVARCHAR(20)  NOT NULL UNIQUE,
    ProgrammeName NVARCHAR(100) NOT NULL,
    DurationYears INT           NOT NULL
);

/* ---------- Students ---------- */
CREATE TABLE Students (
    StudentID    INT IDENTITY(1,1) PRIMARY KEY,
    UserID       INT NOT NULL UNIQUE,
    ProgrammeID  INT NOT NULL,
    FullName     NVARCHAR(100) NOT NULL,
    DateOfBirth  DATE NULL,
    Phone        NVARCHAR(20) NULL,
    EnrollDate   DATE NOT NULL DEFAULT GETDATE(),
    Status       NVARCHAR(20) NOT NULL DEFAULT 'Active',
    CONSTRAINT FK_Students_Users      FOREIGN KEY (UserID)      REFERENCES Users(UserID),
    CONSTRAINT FK_Students_Programmes FOREIGN KEY (ProgrammeID) REFERENCES Programmes(ProgrammeID)
);

/* ---------- Lecturers ---------- */
CREATE TABLE Lecturers (
    LecturerID INT IDENTITY(1,1) PRIMARY KEY,
    UserID     INT NOT NULL UNIQUE,
    FullName   NVARCHAR(100) NOT NULL,
    Phone      NVARCHAR(20) NULL,
    Department NVARCHAR(100) NULL,
    CONSTRAINT FK_Lecturers_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

/* ---------- Courses ---------- */
CREATE TABLE Courses (
    CourseID    INT IDENTITY(1,1) PRIMARY KEY,
    CourseCode  NVARCHAR(20)  NOT NULL UNIQUE,
    CourseName  NVARCHAR(100) NOT NULL,
    Credits     INT           NOT NULL,
    ProgrammeID INT           NOT NULL,
    LecturerID  INT           NULL,
    Semester    NVARCHAR(20)  NULL,
    CONSTRAINT FK_Courses_Programmes FOREIGN KEY (ProgrammeID) REFERENCES Programmes(ProgrammeID),
    CONSTRAINT FK_Courses_Lecturers  FOREIGN KEY (LecturerID)  REFERENCES Lecturers(LecturerID)
);

/* ---------- Enrolments (Student <-> Course, many-to-many) ---------- */
CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID   INT NOT NULL,
    CourseID    INT NOT NULL,
    EnrolDate   DATE NOT NULL DEFAULT GETDATE(),
    Status      NVARCHAR(20) NOT NULL DEFAULT 'Enrolled',
    CONSTRAINT FK_Enrol_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    CONSTRAINT FK_Enrol_Courses  FOREIGN KEY (CourseID)  REFERENCES Courses(CourseID),
    CONSTRAINT UQ_Enrol UNIQUE (StudentID, CourseID)
);

/* ---------- Attendance ---------- */
CREATE TABLE Attendance (
    AttendanceID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID    INT NOT NULL,
    CourseID     INT NOT NULL,
    AttDate      DATE NOT NULL,
    Status       NVARCHAR(10) NOT NULL
                 CONSTRAINT CK_Att_Status CHECK (Status IN ('Present','Absent','Late')),
    RecordedBy   INT NULL,
    CONSTRAINT FK_Att_Students  FOREIGN KEY (StudentID)  REFERENCES Students(StudentID),
    CONSTRAINT FK_Att_Courses   FOREIGN KEY (CourseID)   REFERENCES Courses(CourseID),
    CONSTRAINT FK_Att_Lecturers FOREIGN KEY (RecordedBy) REFERENCES Lecturers(LecturerID)
);

/* ---------- Grades ---------- */
CREATE TABLE Grades (
    GradeID     INT IDENTITY(1,1) PRIMARY KEY,
    StudentID   INT NOT NULL,
    CourseID    INT NOT NULL,
    AssessType  NVARCHAR(50) NOT NULL,
    Marks       DECIMAL(5,2) NOT NULL,
    MaxMarks    DECIMAL(5,2) NOT NULL,
    GradeLetter NVARCHAR(5)  NULL,
    GPA         DECIMAL(4,2) NULL,
    Published   BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Grades_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    CONSTRAINT FK_Grades_Courses  FOREIGN KEY (CourseID)  REFERENCES Courses(CourseID)
);

/* ---------- Announcements ---------- */
CREATE TABLE Announcements (
    AnnouncementID INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(150) NOT NULL,
    Content     NVARCHAR(MAX) NOT NULL,
    PostedBy    INT NOT NULL,
    CourseID    INT NULL,
    PostedDate  DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Ann_Users   FOREIGN KEY (PostedBy) REFERENCES Users(UserID),
    CONSTRAINT FK_Ann_Courses FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

/* ---------- Fees ---------- */
CREATE TABLE Fees (
    FeeID      INT IDENTITY(1,1) PRIMARY KEY,
    StudentID  INT NOT NULL,
    Amount     DECIMAL(10,2) NOT NULL,
    PaidAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    DueDate    DATE NULL,
    Status     NVARCHAR(20) NOT NULL DEFAULT 'Unpaid',
    Semester   NVARCHAR(20) NULL,
    CONSTRAINT FK_Fees_Students FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);
GO

/* =====================================================================
   SEED / TEST DATA  (so login + pages can be tested immediately)
   ===================================================================== */

INSERT INTO Programmes (ProgrammeCode, ProgrammeName, DurationYears) VALUES
('BCS', 'BSc Computer Science', 3),
('BIT', 'BSc Information Technology', 3);

/* One user per role for testing. PasswordHash = placeholder. */
INSERT INTO Users (Username, PasswordHash, Email, Role) VALUES
('admin1',    'CHANGEME', 'admin1@sims.edu',    'Admin'),
('lecturer1', 'CHANGEME', 'lecturer1@sims.edu', 'Lecturer'),
('student1',  'CHANGEME', 'student1@sims.edu',  'Student');

INSERT INTO Lecturers (UserID, FullName, Phone, Department) VALUES
((SELECT UserID FROM Users WHERE Username='lecturer1'), 'Dr. Lim', '0123456789', 'Computing');

INSERT INTO Students (UserID, ProgrammeID, FullName, DateOfBirth, Phone) VALUES
((SELECT UserID FROM Users WHERE Username='student1'),
 (SELECT ProgrammeID FROM Programmes WHERE ProgrammeCode='BCS'),
 'Tan Wei', '2004-03-15', '0198765432');

INSERT INTO Courses (CourseCode, CourseName, Credits, ProgrammeID, LecturerID, Semester) VALUES
('CS101', 'Introduction to Programming', 3,
 (SELECT ProgrammeID FROM Programmes WHERE ProgrammeCode='BCS'),
 (SELECT LecturerID FROM Lecturers WHERE FullName='Dr. Lim'), '2026-1'),
('CS102', 'Database Systems', 3,
 (SELECT ProgrammeID FROM Programmes WHERE ProgrammeCode='BCS'),
 (SELECT LecturerID FROM Lecturers WHERE FullName='Dr. Lim'), '2026-1');

INSERT INTO Enrolments (StudentID, CourseID) VALUES
((SELECT StudentID FROM Students WHERE FullName='Tan Wei'),
 (SELECT CourseID FROM Courses WHERE CourseCode='CS101'));
GO

PRINT 'SIMS database created successfully with test data.';
