USE [master]
GO

/****** Object:  StoredProcedure [dbo].[Appointments_SP_Delete]    Script Date: 15-05-2023 23:09:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE [dbo].[Appointments_SP_Delete](@PatientId nvarchar(10) NULL)
as begin
set nocount on;
UPDATE dbo.Hospital_PatientDetail_Sheet
set Timing = null
WHERE PatientId = @PatientId
	END
GO

