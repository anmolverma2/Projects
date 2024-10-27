USE [master]
GO

/****** Object:  StoredProcedure [dbo].[PatientDetail]    Script Date: 15-05-2023 23:07:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PatientDetail]( @PatientName nvarchar(250) NULL, @PatientAddress Nvarchar(512) NULL, @PatientBloodGroup nvarchar(20) NULL, @AppointedDoctor nvarchar(50) NULL,@PhoneNumber nvarchar(50) NULL,@Email nvarchar(50) NULL,@EmergencyNumber nvarchar(50) NULL,@EmergencyPerson nvarchar(50) NULL,@Gender nvarchar(20) NULL, @OperationType nvarchAr(20) NULL,@PatientAge nvarchar(20) null)
AS BEGIN
SET NOCOUNT ON 
	if(@OperationType = 'INSERT')
	BEGIN
		INSERT INTO Hospital_PatientDetail_Sheet(PatientName, PatientAddress, PatientBloodGroup, AppointedDoctor,PhoneNumber,Email,EmergencyContactName,EmergencyContactNumber,Gender,PatientAge) 
		                                  VALUES (@PatientName,@PatientAddress, @PatientBloodGroup,  @AppointedDoctor,@PhoneNumber,@Email,@EmergencyPerson,@EmergencyNumber,@Gender,@PatientAge)
	END
	
END
GO

