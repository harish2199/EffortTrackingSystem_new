CREATE PROCEDURE GetAllAdmins
AS
BEGIN
    SELECT * FROM Admin;
END;


exec GetAllAdmins