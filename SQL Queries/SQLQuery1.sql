create database EffortTrackingSystem
use EffortTrackingSystem

CREATE TABLE Users (
    user_id int identity primary key,
    user_name varchar(50) NOT NULL,
    designation varchar(50) NOT NULL,
    user_email varchar(100) NOT NULL UNIQUE,
    hashed_password VARCHAR(max) NOT NULL,
    salt_value NVARCHAR(max) NOT NULL,
    role varchar(10) default 'User'
);

INSERT INTO Users (user_name, designation, user_email, hashed_password, salt_value, role)
VALUES
    ('JohnDoe', 'Software Engineer', 'john@example.com', 'RV/46mwDhKY+3rTqXS/uypA6peg1/o0c+zsDiiI3lY8=', 'TOhaCI0Ib1TAXN5Drjhm2A==', 'Admin'),
    ('JaneSmith', 'Product Manager', 'jane@example.com', 'Cppq+/0LE36ZppgVTp+bvaiaaeraxVdinHgEwaBUGuc=', 'kn6CqXcPdpzGXY4TszuzQw==', 'User'),
    ('AlexJohnson', 'Data Scientist', 'alex@example.com', 'NSJ10W5dzupqTSNqgWg2G6bHqlTOtJNAWLSBJFUz/Z0=', 'q/PrBmcPzUNaMfuM88Y0Fg==', 'User'),
    ('EmilyBrown', 'UI/UX Designer', 'emily@example.com', '2WD1rwsMSE8zj/qhltjWtJPPVN6VecIdtmkd7F5oNxw=', 'mehY+7wR4A5yh2WpMpCvBA==', 'User');
   

create table Project(
	project_id int identity(1,1) primary key,
	project_name varchar(100) NOT NULL
);
create table Task(
	task_id int identity(1,1) primary key,
	task_name varchar(50) NOT NULL,
);
create table Shifts(
	shift_id int identity(1,1) primary key,
	shift_name varchar(50) NOT NULL,
	start_time time NOT NULL,
	end_time time NOT NULL,
);
-- Sample data for Project table
INSERT INTO Project (project_name)
VALUES
    ('Project A'),
    ('Project B'),
    ('Project C'),
    ('Project D'),
    ('Project E');

-- Sample data for Task table
INSERT INTO Task (task_name)
VALUES
    ('Task 1'),
    ('Task 2'),
    ('Task 3'),
    ('Task 4'),
    ('Task 5');

-- Sample data for Shifts table
INSERT INTO Shifts (shift_name, start_time, end_time)
VALUES
    ('Morning Shift', '08:00:00', '16:00:00'),
    ('Afternoon Shift', '12:00:00', '20:00:00'),
    ('Night Shift', '20:00:00', '04:00:00'),
    ('Day Shift', '09:00:00', '17:00:00'),
    ('Late Night Shift', '22:00:00', '06:00:00');

create table Assign_Task (
    assign_task_id int identity(1,1) primary key,
    user_id int references users(user_id),
    project_id int references project(project_id),
    task_id int references task(task_id),
    shift_id int references shifts(shift_id),
    start_date date NOT NULL,
	end_date date NOT NULL,
	Status varchar(20) NOT NULL
);

-- Sample data for Assign_Task table
INSERT INTO Assign_Task (user_id, project_id, task_id, shift_id, start_date, end_date, Status)
VALUES
    (1, 1, 1, 1, '2023-08-24', '2023-09-08', 'In Progress'),
    (2, 2, 2, 2, '2023-08-24', '2023-09-08', 'In Progress'),
    (3, 3, 3, 3, '2023-08-28', '2023-09-12', 'In Progress'),
    (4, 4, 4, 4, '2023-08-28', '2023-09-12', 'In Progress');

create table Effort(
	effort_id int identity(1,1) primary key,
	assign_task_id int references Assign_Task(assign_task_id),
	shift_id INT REFERENCES Shifts(shift_id),
	hours_worked int NOT NULL,
	submitted_date date NOT NULL,
	status varchar(20) NOT NULL
);
-- Sample data for Effort table based on assigned tasks
INSERT INTO Effort (assign_task_id, shift_id, hours_worked, submitted_date, status)
VALUES
    -- User 1
    (1, 1, 9, '2023-08-24', 'Approved'),
    (1, 1, 9, '2023-08-25', 'Approved'),
    (1, 1, 9, '2023-08-28', 'Approved'),
    (1, 1, 9, '2023-08-29', 'Approved'),
    (1, 1, 9, '2023-08-30', 'Approved'),
    -- User 2
    (2, 2, 9, '2023-08-24', 'Approved'),
    (2, 2, 9, '2023-08-25', 'Approved'),
    (2, 2, 9, '2023-08-28', 'Approved'),
    (2, 2, 9, '2023-08-29', 'Approved'),
    (2, 2, 9, '2023-08-30', 'Approved'),
    -- User 3
    (3, 3, 9, '2023-08-28', 'Approved'),
    (3, 3, 9, '2023-08-29', 'Approved'),
    (3, 3, 9, '2023-08-30', 'Approved'),
    -- User 4
    (4, 4, 9, '2023-08-28', 'Approved'),
    (4, 4, 9, '2023-08-29', 'Approved'),
    (4, 4, 9, '2023-08-30', 'Approved');


create table Leave(
	leave_id int identity(1,1) primary key,
	user_id int references Users(user_id),
	date date NOT NULL,
	reason varchar(max) NOT NULL,
	status varchar(20) NOT NULL
);
create table Shift_Change (
    shift_Change_id int identity(1,1) primary key,
    user_id int references Users(user_id),
    assigned_shift_id int references Shifts(shift_id),
    date date NOT NULL,
    new_shift_id int references Shifts(shift_id),
    reason varchar(max) NOT NULL,
   status varchar(20) NOT NULL
);


select * from users
select * from project
select * from task
select * from shifts
select * from Leave
select * from Shift_Change
select * from effort
select * from Assign_Task

