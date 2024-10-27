USE [master]
GO

/****** Object:  StoredProcedure [dbo].[PatientDetail_ShowById]    Script Date: 15-05-2023 23:08:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PatientDetail_ShowById](@PatientId nvarchar(10) NULL)
as begin
set nocount on;
SELECT * FROM dbo.Hospital_PatientDetail_Sheet WHERE PatientId = @PatientId
	END
GO

