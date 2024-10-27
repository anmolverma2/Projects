USE [master]
GO

/****** Object:  StoredProcedure [dbo].[PatientDetail_Update]    Script Date: 15-05-2023 23:08:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PatientDetail_Update]( @PatientId nvarchar(20) null,@PatientName nvarchar(250) NULL, @PatientAddress Nvarchar(512) NULL, @PatientBloodGroup nvarchar(20) NULL, @AppointedDoctor nvarchar(50) NULL,@PhoneNumber nvarchar(50) NULL,@Email nvarchar(50) NULL,@EmergencyNumber nvarchar(50) NULL,@EmergencyPerson nvarchar(50) NULL,@Gender nvarchar(20) NULL, @PatientAge nvarchar(20) null)
AS BEGIN
SET NOCOUNT ON 
	
		UPDATE Hospital_PatientDetail_Sheet
		SET PatientName = @PatientName,
		PatientAge = @PatientAge,
		PatientAddress = @PatientAddress,
		PatientBloodGroup = @PatientBloodGroup,
		AppointedDoctor = @AppointedDoctor,
		PhoneNumber = @PhoneNumber,
		Email = @Email,
		EmergencyContactName = @EmergencyPerson,
		EmergencyContactNumber = @EmergencyNumber,
		Gender = @Gender
		WHERE
		PatientId = @PatientId
END
GO

