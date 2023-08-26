CREATE OR ALTER PROCEDURE GetAllUsers
AS
BEGIN
    SELECT
        user_id,
        user_name,
        designation,
        user_email,
        hashed_password,
        role
    FROM
        Users;
END;

CREATE PROCEDURE GetProjects
AS
BEGIN
    SELECT * FROM Project;
END;

CREATE PROCEDURE GetTasks
AS
BEGIN
    SELECT * FROM Task;
END;

CREATE or alter PROCEDURE GetShifts
AS
BEGIN
    SELECT * FROM Shifts;
END;

exec GetShifts


CREATE OR ALTER PROCEDURE GetPendingLeaves
AS
BEGIN
    SELECT
        L.leave_id,
        L.user_id,
        U.user_name,
        L.date,
        L.reason,
        L.status
    FROM
        Leave AS L
    INNER JOIN
        Users AS U ON L.user_id = U.user_id
    WHERE
        L.status = 'Pending';
END;

CREATE OR ALTER PROCEDURE GetPendingShiftChange
AS
BEGIN
    SELECT
        SC.shift_change_id,
        SC.user_id,
        U.user_name,
        SC.assigned_shift_id,
        AssignedShift.shift_name AS assigned_shift_name,
        SC.date,
        SC.new_shift_id,
        NewShift.shift_name AS new_shift_name,
        SC.reason,
        SC.status
    FROM
        Shift_Change AS SC
    INNER JOIN
        Users AS U ON SC.user_id = U.user_id
    INNER JOIN
        Shifts AS AssignedShift ON SC.assigned_shift_id = AssignedShift.shift_id
    INNER JOIN
        Shifts AS NewShift ON SC.new_shift_id = NewShift.shift_id
    WHERE
        SC.status = 'Pending';
END;
exec GetPendingShiftChange



CREATE OR ALTER PROCEDURE AddUser
    @user_name varchar(50),
    @designation varchar(50),
    @user_email varchar(100),
    @hashed_password VARCHAR(max),
    @role varchar(10) = 'User',
    @message varchar(100) OUTPUT
AS
BEGIN
    -- Check if the user's email already exists
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_email = @user_email)
    BEGIN
        -- Insert user if email doesn't exist
        INSERT INTO Users (user_name, designation, user_email, hashed_password, role)
        VALUES (@user_name, @designation, @user_email, @hashed_password, @role);

        SET @message = 'User inserted successfully!';
    END
    ELSE
    BEGIN
        SET @message = 'User with this email already exists!';
    END
END;

CREATE or alter PROCEDURE UpdateUser
    @user_id INT,
    @user_name VARCHAR(50),
    @designation VARCHAR(50),
    @user_email VARCHAR(100),
    @hashed_password VARCHAR(MAX),
    @role VARCHAR(10),
    @output_message NVARCHAR(100) OUTPUT
AS
BEGIN
    -- Update user details
    UPDATE Users
    SET user_name = @user_name,
        designation = @designation,
        user_email = @user_email,
        hashed_password = @hashed_password,
        role = @role
    WHERE user_id = @user_id;

    IF @@ROWCOUNT > 0
    BEGIN
        SET @output_message = N'User details updated successfully.';
    END
    ELSE
    BEGIN
        SET @output_message = N'No matching user found.';
    END
END;


CREATE or ALTER PROCEDURE AssignTask
    @user_id INT,
    @project_id INT,
    @task_id INT,
    @shift_id INT,
    @start_date DATE = NULL,
    @end_date DATE = NULL,
    @status VARCHAR(20) = 'Pending',
    @output_message NVARCHAR(100) OUTPUT
AS
BEGIN
    DECLARE @current_date DATE = GETDATE();

    IF @start_date < @current_date
    BEGIN
        SET @output_message = 'Start date cannot be in the past.';
        RETURN;
    END;

    IF @end_date IS NOT NULL AND @end_date < @start_date
    BEGIN
        SET @output_message = 'End date cannot be before start date.';
        RETURN;
    END;

    IF EXISTS (
        SELECT 1 FROM Assign_Task
        WHERE user_id = @user_id
        AND @start_date < end_date
        AND @end_date > start_date
    )
    BEGIN
        SET @output_message = 'Overlapping assignment not allowed.';
        RETURN;
    END;

    INSERT INTO Assign_Task (user_id, project_id, task_id, shift_id, start_date, end_date, Status)
    VALUES (@user_id, @project_id, @task_id, @shift_id, @start_date, @end_date, @status);

    SET @output_message = 'Assignment added successfully.';
END;

CREATE OR ALTER PROCEDURE ApproveEffort
    @effort_id INT,
    @output_message NVARCHAR(100) OUTPUT
AS
BEGIN
    UPDATE Effort
    SET status = 'Approved'
    WHERE effort_id = @effort_id;

    SET @output_message = 'Effort Approved Successfully.';
END;

CREATE OR ALTER PROCEDURE ApproveOrRejectLeave
    @leave_id INT,
    @new_status VARCHAR(20),
    @output_message NVARCHAR(100) OUTPUT
AS
BEGIN
	    UPDATE Leave
        SET status = @new_status
            WHERE leave_id = @leave_id;
            
            IF @new_status = 'Approved'
            BEGIN
                SET @output_message = 'Leave Approved Successfully.';
            END
            ELSE
            BEGIN
                SET @output_message = 'Leave Rejected.';
            END   
END;


CREATE OR ALTER PROCEDURE ApproveOrRejectShiftChange
    @shiftChangeId int,
    @newStatus varchar(20),
    @outputMessage varchar(100) OUTPUT
AS
BEGIN
    UPDATE Shift_Change
    SET status = @newStatus
    WHERE shift_change_id = @shiftChangeId;

    IF @newStatus = 'Approved'
        SET @outputMessage = 'Shift change approved successfully.';
    ELSE IF @newStatus = 'Rejected'
        SET @outputMessage = 'Shift change rejected.';
END;
