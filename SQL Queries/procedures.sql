CREATE or alter PROCEDURE GetAssignedTasksByUserId
    @userId INT
AS
BEGIN
    SELECT
        AT.assign_task_id,
		U.user_id,
        P.project_name,
        T.task_name,
		S.shift_id,
        S.shift_name,
        AT.start_date,
        AT.end_date,
        AT.Status
    FROM Assign_Task AS AT
	INNER JOIN Users AS U ON AT.user_id = U.user_id
    INNER JOIN Project AS P ON AT.project_id = P.project_id
    INNER JOIN Task AS T ON AT.task_id = T.task_id
    INNER JOIN Shifts AS S ON AT.shift_id = S.shift_id
    WHERE AT.user_id = @userId;

END;
exec GetAssignedTasksByUserId @userId=2

CREATE OR ALTER PROCEDURE SubmitEffort
    @assign_task_id int,
    @shift_id int,
    @hours_worked int,
    @submitted_date date,
    @status varchar(20),
    @output_message varchar(100) OUTPUT
AS
BEGIN
    DECLARE @user_id int;
    DECLARE @new_shift_id int;
    DECLARE @shift_change_status varchar(20);
    DECLARE @leave_status varchar(20);
    DECLARE @assigned_shift_id int; -- Declare the variable here

    -- Get the user ID for the given assign_task_id
    SELECT @user_id = user_id
    FROM Assign_Task
    WHERE assign_task_id = @assign_task_id;

    -- Check shift change status for the user and the day
    SELECT @new_shift_id = new_shift_id, @shift_change_status = status
    FROM Shift_Change
    WHERE user_id = @user_id
    AND date = @submitted_date;

    -- Check leave status for the user and the day
    SELECT @leave_status = status
    FROM Leave
    WHERE user_id = @user_id
    AND date = @submitted_date;

    -- Check criteria for effort submission based on shift change and leave status
    IF @shift_change_status = 'Approved'
    BEGIN
        -- If shift change is approved, allow effort submission with the new shift
        IF @new_shift_id <> @shift_id
        BEGIN
            SET @output_message = 'Effort can only be submitted with the new shift from approved shift change.';
            RETURN;
        END;
    END
    ELSE IF @shift_change_status = 'Rejected'
    BEGIN
        -- If shift change is rejected, allow effort submission with the assigned shift
        SELECT @assigned_shift_id = shift_id -- Assign the value here
        FROM Assign_Task
        WHERE assign_task_id = @assign_task_id;

        IF @shift_id <> @assigned_shift_id
        BEGIN
            SET @output_message = 'Effort can only be submitted with the assigned shift for rejected shift change.';
            RETURN;
        END;
    END
    ELSE IF @shift_change_status = 'Pending'
    BEGIN
        -- If shift change is pending, disallow effort submission
        SET @output_message = 'Effort cannot be submitted while there is a pending shift change.';
        RETURN;
    END

    -- Allow effort submission if leave status is Rejected
    IF @leave_status = 'Approved'
    BEGIN
        SET @output_message = 'Effort cannot be submitted while leave is approved.';
        RETURN;
    END
   
   ELSE IF @leave_status = 'Pending'
    BEGIN
        -- If leave  change is pending, disallow effort submission
        SET @output_message = 'Effort cannot be submitted while there is a pending leave.';
        RETURN;
    END
    -- Check if there is no shift change for the submitted day
    IF NOT EXISTS (
        SELECT 1
        FROM Shift_Change
        WHERE user_id = @user_id
        AND date = @submitted_date
    )
    BEGIN
        -- Check if the submitted shift matches the assigned shift
        SELECT @assigned_shift_id = shift_id -- Assign the value here
        FROM Assign_Task
        WHERE assign_task_id = @assign_task_id;

        IF @shift_id <> @assigned_shift_id
        BEGIN
            SET @output_message = 'Effort can only be submitted with the assigned shift.';
            RETURN;
        END;
    END

	-- Insert the effort record
    INSERT INTO Effort (assign_task_id, shift_id, hours_worked, submitted_date, status)
    VALUES (@assign_task_id, @shift_id, @hours_worked, @submitted_date, @status);

    -- Set success message
    SET @output_message = 'Effort submitted successfully.';
END;



CREATE OR ALTER PROCEDURE GetEfforts
AS
BEGIN
    SELECT
        E.effort_id,
        AT.assign_task_id,
        U.user_name,
		U.user_id,
        P.project_name,
        T.task_name,
        S.shift_name,
        E.hours_worked,
        E.submitted_date,
		E.status
    FROM
        Effort E
    JOIN
        Assign_Task AT ON E.assign_task_id = AT.assign_task_id
    JOIN
        Users U ON AT.user_id = U.user_id
    JOIN
        Project P ON AT.project_id = P.project_id
        JOIN
        Task T ON AT.task_id = T.task_id
        JOIN
        Shifts S ON E.shift_id = S.shift_id
    ORDER BY
        E.submitted_date DESC;
END;

exec GetEfforts



CREATE OR ALTER PROCEDURE SubmitLeave
    @user_id int,
    @date date,
    @reason varchar(max),
    @status varchar(20) = 'Pending',
    @output_message varchar(100) OUTPUT
AS
BEGIN
    -- Check if the user has already submitted a leave request for the given date
    IF EXISTS (
        SELECT 1
        FROM Leave
        WHERE user_id = @user_id
        AND date = @date
    )
    BEGIN
        SET @output_message = 'Leave request for the user on the given date already exists.';
        RETURN;
    END;

    -- Check if the user has already submitted an effort for the given date
    IF EXISTS (
        SELECT 1
        FROM Effort
        WHERE assign_task_id IN (SELECT assign_task_id FROM Assign_Task WHERE user_id = @user_id)
        AND submitted_date = @date
    )
    BEGIN
        SET @output_message = 'Effort has already been submitted by the user for the given date.';
        RETURN;
    END;

    -- Check if the user has already submitted a shift change request for the given date
    IF EXISTS (
        SELECT 1
        FROM Shift_Change
        WHERE user_id = @user_id
        AND date = @date
    )
    BEGIN
        SET @output_message = 'Shift change request has already been submitted by the user for the given date.';
        RETURN;
    END;

    -- Insert the leave request
    INSERT INTO Leave (user_id, date, reason, status)
    VALUES (@user_id, @date, @reason, @status);

    SET @output_message = 'Leave request submitted successfully.';
END;

CREATE OR ALTER PROCEDURE SubmitShiftChange
    @user_id int,
    @assigned_shift_id int,
    @date date,
    @new_shift_id int,
    @reason varchar(max),
    @status varchar(20) = 'Pending',
    @output_message varchar(100) OUTPUT
AS
BEGIN
    -- Check if the user has already submitted a shift change request for the given date
    IF EXISTS (
        SELECT 1
        FROM Shift_Change
        WHERE user_id = @user_id
        AND date = @date
    )
    BEGIN
        SET @output_message = 'Shift change request for the user on the given date already exists.';
        RETURN;
    END;

    -- Check if the user has submitted an effort for the day
    IF EXISTS (
        SELECT 1
        FROM Effort
        WHERE assign_task_id IN (SELECT assign_task_id FROM Assign_Task WHERE user_id = @user_id)
        AND submitted_date = @date
    )
    BEGIN
        SET @output_message = 'Effort has already been submitted by the user for the given date.';
        RETURN;
    END;

    -- Check if the user has submitted a leave for the day
    IF EXISTS (
        SELECT 1
        FROM Leave
        WHERE user_id = @user_id
        AND date = @date
    )
    BEGIN
        SET @output_message = 'Leave has already been submitted by the user for the given date.';
        RETURN;
    END;

    -- Check if the user is trying to change to the same shift assigned to them
    IF @new_shift_id = @assigned_shift_id
    BEGIN
        SET @output_message = 'The new shift is the same as the user''s current assigned shift.';
        RETURN;
    END;

    -- Insert the shift change request
    INSERT INTO Shift_Change (user_id, assigned_shift_id, date, new_shift_id, reason, status)
    VALUES (@user_id, @assigned_shift_id, @date, @new_shift_id, @reason, @status);

    SET @output_message = 'Shift change request submitted successfully.';
END;


CREATE OR ALTER PROCEDURE GetEffortsByDate
    @year INT = NULL,
    @month INT = NULL,
    @day INT = NULL
AS
BEGIN
    -- Retrieve efforts based on date range
    SELECT
        E.effort_id,
        AT.assign_task_id,
        U.user_name,
        U.user_id,
        P.project_name,
        T.task_name,
        S.shift_name,
		S.start_time,
		S.end_time,
        E.hours_worked,
        E.submitted_date
    FROM
        Effort E
    JOIN
        Assign_Task AT ON E.assign_task_id = AT.assign_task_id
    JOIN
        Users U ON AT.user_id = U.user_id
    JOIN
        Project P ON AT.project_id = P.project_id
    JOIN
        Task T ON AT.task_id = T.task_id
    JOIN
        Shifts S ON E.shift_id = S.shift_id
    WHERE
        E.status = 'Approved'
        AND (@year IS NULL OR YEAR(E.submitted_date) = @year)
        AND (@month IS NULL OR MONTH(E.submitted_date) = @month)
        AND (@day IS NULL OR DAY(E.submitted_date) = @day)
    ORDER BY
        E.submitted_date DESC;
END;


EXEC GetEffortsByDate @year = 2023, @month = 8, @day=25;



