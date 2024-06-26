
CREATE TABLE [PortalManagement].[Jobs](
	[JobId] [int] IDENTITY(1,1) primary key NOT NULL,
	[JobTitle] [nvarchar](100) NULL,
	[JobDescription] [nvarchar](max) NULL,
	[Location] [nvarchar](100) NULL,
	[Salary] [nvarchar](100) NULL,
	[PostedDate] [datetime] NULL,
	[ExpiryDate] [datetime] NULL,
	[CompanyName] [nvarchar](100) NULL,
	[ContactEmail] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[NoOfApplied] [int] NULL,
)

--------------------------------------------------------------------

CREATE TABLE [PortalManagement].[Applicants](
	[ApplicantID] [int] IDENTITY(1,1) primary key NOT NULL,
	[JobID] [int] NULL,
	[FullName] [nvarchar](500) NULL,
	[Email] [nvarchar](500) NULL,
	[Phone] [nvarchar](50) NULL,
	[Age] [int] NULL,
	[Gender] [nvarchar](100) NULL,
	[ResumeFilePath] [varchar](1024) NULL,
	[PhotoPath] [varchar](1024) NULL,
	[ApplicationDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[QualificationType] [nvarchar](500) NULL,
	[Qualification] [nvarchar](500) NULL,
	[Experience] [nvarchar](500) NULL,
	[Address] [nvarchar](1024) NULL,
	[OTP] [nvarchar](100) NULL,
	[JobTitle] [nvarchar](520) NULL,
	[VerifybyEmail] [int] NULL,
)

---------------------------------------------------------------------------

ALTER PROCEDURE [WebApplication_SP].[ViewApplicant] --'2','viewjob'

	@ApplicantID NVARCHAR(100),
    @OperationType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

   	 IF @OperationType = 'ViewApplicant'
    BEGIN
        
		select ApplicantID
,JobID
,FullName
,Email
,Phone
,Age
,Gender
,ResumeFilePath
,PhotoPath
,Convert(nvarchar(500),ApplicationDate,103) as ApplicationDate
,CreateDate
,IsDeleted
,QualificationType
,Qualification
,Experience from  portalmanagement.Applicants where ApplicantID = CONVERT(INT, @ApplicantID) and IsDeleted=0;

    END

	IF @OperationType = 'RejectApplicant'
    BEGIN
        
		
		declare @jodid int 
		select @jodid=JobID from  portalmanagement.Applicants  where ApplicantID = CONVERT(INT, @ApplicantID);

		update portalmanagement.Jobs set NoOfApplied = NoOfApplied - 1 where JobId=@jodid

		update  portalmanagement.Applicants set IsDeleted=1 where  ApplicantID = CONVERT(INT, @ApplicantID);


    END

END


------------------------------------------------------

ALTER PROCEDURE [WebApplication_SP].[InsertJob]
    @JobTitle NVARCHAR(100),
    @JobDescription NVARCHAR(MAX),
    @Location NVARCHAR(100),
    @Salary nvarchar(500),
    @ExpiryDate nvarchar(520),
    @CompanyName NVARCHAR(100),
    @ContactEmail NVARCHAR(100),
    @OperationType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 'INSERT'
    BEGIN
        INSERT INTO  portalmanagement.jobs (JobTitle, JobDescription, Location, Salary, ExpiryDate, CompanyName, ContactEmail,PostedDate,IsActive,IsDeleted)
        VALUES (@JobTitle, @JobDescription, @Location, @Salary, CONVERT(DATETIME,@ExpiryDate,101), @CompanyName, @ContactEmail,GETDATE(),1,0);

    END
END


-------------------------------------------------------------------

ALTER procedure [WebApplication_SP].[JobList] --'','','3456','john','Aplicant'
(
    @jobtitle nvarchar(500) = null,
    @location nvarchar(500) = null,
    @mobile nvarchar(500) = null,
    @email nvarchar(500) = null,
    @operationtype nvarchar(500) = null
)
as
begin
    if(@operationtype = 'Select')
    begin
        select 
		JobId,
JobTitle,
JobDescription,
Location,
Salary,
convert(nvarchar(200),PostedDate,103) as PostedDate,
convert(nvarchar(200),ExpiryDate,103) as ExpiryDate,
CompanyName,
ContactEmail,
IsActive,
IsDeleted,
NoOfApplied

        from PortalManagement.Jobs 
        where isdeleted=0 and (@jobtitle is null or @jobtitle = '' or JobTitle like '%' + @jobtitle + '%') 
        and (@location is null or @location = '' or [Location] like '%' + @location + '%')
        order by JobId desc
    end

	  if(@operationtype = 'Aplicant')
    begin
        select 
		
		a.ApplicantID,
a.JobID,
b.CompanyName,
a.FullName,
a.Email,
a.Phone,
a.Age,
a.Gender,
b.JobTitle,
a.Qualification,
a.Experience,
a.Address,
a.QualificationType,
a.ResumeFilePath,
a.PhotoPath,
convert(nvarchar(200),a.ApplicationDate,103) as ApplicationDate,
convert(nvarchar(200),a.CreateDate,103) as CreateDate,
a.IsDeleted

        from [PortalManagement].[Applicants] a inner join [PortalManagement].Jobs b on a.JobID=b.JobId where a.isdeleted=0 and (@email is null or @email = '' or a.Email like '%' + @email + '%') 
        and (@mobile is null or @mobile = '' or a.Phone like '%' + @mobile + '%') and a.VerifybyEmail=1
        order by a.ApplicantID desc
    end

end


--alter table [PortalManagement].[Applicants] add Experience nvarchar(500) null

--------------------------------------------------------------------------

ALTER PROCEDURE [WebApplication_SP].[ViewData] --'2','viewjob'
    @JobId NVARCHAR(100),
    @OperationType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 'ViewJob'
    BEGIN
        
		select JobId
,JobTitle
,JobDescription
,Location
,Salary
,convert(nvarchar(520),PostedDate,103) as PostedDate
,convert(nvarchar(520),ExpiryDate,103) as ExpiryDate
,CompanyName
,ContactEmail
,IsActive
,IsDeleted
,NoOfApplied from  portalmanagement.jobs where JobId = CONVERT(INT, @JobId) and IsDeleted=0;

    END

	IF @OperationType = 'RemoveJob'
    BEGIN
        
		update  portalmanagement.jobs set IsDeleted=1 where JobId = CONVERT(INT, @JobId);

    END


END


------------------------------------------------------------------------------------------------

ALTER PROCEDURE [WebApplication_SP].[InsertApplicantDetails]
    @FullName NVARCHAR(100) = null,
    @Email NVARCHAR(MAX)= null,
    @Phone NVARCHAR(100)= null,
    @Age nvarchar(500)= null,
    @Gender nvarchar(520)= null,
    @ResumeFilePath NVARCHAR(100)= null,
    @QualificationType NVARCHAR(100)= null,
    @Qualification NVARCHAR(100)= null,
    @Experience NVARCHAR(100)= null,
    @Address NVARCHAR(100)= null,
    @OTP NVARCHAR(100)= null,
	@JobTitle NVARCHAR(500)= null,
	@ApplicantID NVARCHAR(100)= null,
    @OperationType NVARCHAR(50) = null
AS
BEGIN
    SET NOCOUNT ON;

    IF @OperationType = 'INSERT'
    BEGIN
        INSERT INTO  PortalManagement.Applicants(FullName, Email, Phone, Age, Gender, ResumeFilePath,ApplicationDate,
		CreateDate,IsDeleted,QualificationType,Qualification,Experience,[Address],OTP,JobTitle)
        VALUES (@FullName, @Email, @Phone, cast(@Age as int), @Gender, @ResumeFilePath, getdate()+10,GETDATE(),0,@QualificationType
		,@Qualification,@Experience,@Address,@OTP,@JobTitle);

		select top 1 ApplicantID,Email,convert(nvarchar(200),ApplicationDate,103) as ApplicationDate,OTP from PortalManagement.Applicants order by ApplicantID desc;

    END
	
    IF @OperationType = 'Verify'
    BEGIN

	if exists(select * from PortalManagement.Applicants where ApplicantID=CAST(@ApplicantID AS INT) and OTP=@OTP )
	begin
	update a set 
	a.JobID=b.JobID,a.VerifybyEmail=1
	from PortalManagement.Applicants a inner join PortalManagement.Jobs b on ltrim(rtrim(a.JobTitle))=ltrim(rtrim(b.JobTitle)) where a.ApplicantID=CAST(@ApplicantID AS INT) 
	
	declare @jodid int 
	select @jodid=JobID from  portalmanagement.Applicants  where ApplicantID = CONVERT(INT, @ApplicantID);

	update portalmanagement.Jobs set NoOfApplied = NoOfApplied + 1 where JobId=@jodid

		
		select 1 as result;
		
		end
		else 
		begin

		select 2 as result

		end

    END
END
