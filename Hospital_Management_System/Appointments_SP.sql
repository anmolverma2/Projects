USE [master]
GO

/****** Object:  StoredProcedure [dbo].[Appointments_SP]    Script Date: 15-05-2023 23:09:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Appointments_SP](@PatientId nvarchar(10) NULL,@AppointmentTime nvarchar(30)=null)
as begin
set nocount on;
UPDATE dbo.Hospital_PatientDetail_Sheet
set Timing = @AppointmentTime
WHERE PatientId = @PatientId
	END
GO

