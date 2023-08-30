create database EffortTrackingSystem
use EffortTrackingSystem

create table Admin(
	admin_id int identity primary key,
	admin_name varchar(50),
	admin_email varchar(100),
	hashed_password VARCHAR(max),
	salt_value NVARCHAR(max),
	role varchar(10) default 'Admin'
)
create table Users(
	user_id int identity primary key,
	user_name varchar(50),
	designation varchar(50),
	user_email varchar(100),
	hashed_password VARCHAR(max),
	salt_value NVARCHAR(max),
	role varchar(10) default 'User'
);

create table Project(
	project_id int identity(1,1) primary key,
	project_name varchar(100) not null
);
create table Task(
	task_id int identity(1,1) primary key,
	task_name varchar(50) not null
);
create table Shifts(
	shift_id int identity(1,1) primary key,
	shift_name varchar(50) not null,
	start_time time not null,
	end_time time not null
);

create table Assign_Task (
    assign_task_id int identity(1,1) primary key,
    user_id int references users(user_id),
    project_id int references project(project_id),
    task_id int references task(task_id),
    shift_id int references shifts(shift_id),
    start_date date default getdate(),
	end_date date default getdate(),
	Status varchar(20) default 'Pending'
);

create table Effort(
	effort_id int identity(1,1) primary key,
	assign_task_id int references Assign_Task(assign_task_id),
	shift_id INT REFERENCES Shifts(shift_id),
	hours_worked int not null,
	submitted_date date default getdate(),
	status varchar(20) default 'pending'
);

create table Leave(
	leave_id int identity(1,1) primary key,
	user_id int references Users(user_id),
	date date,
	reason varchar(max),
	status varchar(20) default 'Pending'
);
create table Shift_Change (
    shift_Change_id int identity(1,1) primary key,
    user_id int references Users(user_id),
    assigned_shift_id int references Shifts(shift_id),
    date date not null,
    new_shift_id int references Shifts(shift_id),
    reason varchar(max) not null,
   status varchar(20) default 'Pending'
);





-- Sample data for Users table
INSERT INTO Users (user_name, designation, user_email, hashed_password, role)
VALUES
    ('John Doe', 'Support', 'john@example.com', 'hashedpass123', 'User'),
    ('Jane Smith', 'Developer', 'jane@example.com', 'hashedpass456', 'User')

-- Sample data for Project table
INSERT INTO Project (project_name)
VALUES
    ('Project 1'),
    ('Project 2'),
    ('Project 3')

-- Sample data for Task table
INSERT INTO Task (task_name)
VALUES
    ('Task 1'),
    ('Task 3'),
    ('Task 3')

-- Sample data for Shifts table
INSERT INTO Shifts (shift_name, start_time, end_time)
VALUES
    ('Morning Shift', '08:00', '16:00'),
    ('Afternoon Shift', '12:00', '20:00'),
    ('Night Shift', '20:00', '04:00');

-- Sample data for Assign_Task table
INSERT INTO Assign_Task (user_id, project_id, task_id, shift_id, start_date, end_date, Status)
VALUES
    (1, 1, 1, 1, '2023-08-21', '2023-08-27', 'Pending'),
    (2, 2, 3, 2, '2023-08-21', '2023-08-27', 'Pending');

INSERT INTO Effort (assign_task_id, shift_id, hours_worked, submitted_date, status) VALUES
(1, 1, 9, '2023-08-25', 'Approved'),
(1, 1, 9, '2023-08-24', 'Approved'),
(1, 1, 9, '2023-08-23', 'Approved'),
(1, 1, 9, '2023-08-22', 'Approved'),
(1, 1, 9, '2023-08-21', 'Approved');
INSERT INTO Effort (assign_task_id, shift_id, hours_worked, submitted_date, status) VALUES
(2, 2, 9, '2023-08-25', 'Approved'),
(2, 2, 9, '2023-08-26', 'Approved');
INSERT INTO Effort (assign_task_id, shift_id, hours_worked, submitted_date, status) VALUES
(1, 1, 9, '2022-07-25', 'Approved'),
(1, 1, 9, '2022-07-24', 'Approved');


select * from admin
select * from users
select * from project
select * from task
select * from shifts
select * from Leave
select * from Shift_Change
select * from effort
select * from Assign_Task

delete Users where user_id = 7
delete Effort where effort_id = 34
delete Shift_Change where shift_Change_id = 11
delete leave where leave_id = 9
delete Assign_Task where assign_task_id = 12

update Assign_Task set end_date = '2023-09-02' where assign_task_id = 2
update leave set status = 'Pending' where leave_id=9
update Shift_Change set status = 'Pending' where shift_Change_id=12
update Effort set status = 'Pending' where effort_id = 27