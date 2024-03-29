
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](520) NULL,
	[EmailId] [nvarchar](520) NULL,
	[MobileNumber] [nvarchar](520) NULL,
	[Gender] [nvarchar](520) NULL,
	[Age] [int] NULL,
	[PaymentAmount] [float] NULL,
	[PaymentDate] [datetime] NULL,
	[Isdeleted] [bit] NULL,
	[ModifiedDate] [datetime] NULL,
	[DeletedDate] [datetime] NULL,
	[IsModified] [int] default 0
) 

--select * from users


ALTER procedure [WebApplication_SP].[SP_User_Crud_Export_Bulk] --'SELECTBYID',null,null,null,null,null,null,null,4
(@chvnOperationType nvarchar(520) = null,
@chvnUserName nvarchar(520) = null,
@chvnEmailId nvarchar(520) = null,
@chvnMobileNumber nvarchar(520) = null,
@chvnGender nvarchar(520) = null,
@chvnAge int = null,
@chvnPaymentAmount double precision = null,
@chvnUserId int = null)
as begin

if(@chvnOperationType = 'SELECT')
 
begin

select UserName,EmailId,MobileNumber,Gender,Age,PaymentAmount,convert(nvarchar(520),PaymentDate,103) as PaymentDate,UserId from Users where Isdeleted=0 order by UserId desc

end
else if(@chvnOperationType = 'SELECTBYID')
 
begin

select UserId,UserName,EmailId,MobileNumber,Gender,Age,PaymentAmount,convert(nvarchar(520),PaymentDate,103) as PaymentDate from Users where Isdeleted=0 and UserId=@chvnUserId

end
else if(@chvnOperationType = 'INSERT')
 
begin

INSERT INTO USERS (UserName,EmailId,MobileNumber,Gender,Age,PaymentAmount,PaymentDate,Isdeleted) 
VALUES (@chvnUserName,@chvnEmailId,@chvnMobileNumber,@chvnGender,@chvnAge,@chvnPaymentAmount,getdate(),0)

select 1
end

else if(@chvnOperationType = 'UPDATE')
 
begin

UPDATE Users
SET 
    UserName = @chvnUserName,
    EmailId = @chvnEmailId,
    MobileNumber = @chvnMobileNumber,
    Gender = @chvnGender,
    Age = @chvnAge,
    PaymentAmount = @chvnPaymentAmount,
    ModifiedDate = GETDATE()
	,    IsModified = IsModified + 1
WHERE UserId = @chvnUserId;


select 2
end

else if(@chvnOperationType = 'DELETE')
 
begin

update users set isdeleted=1,DeletedDate=getdate() where UserId=@chvnUserId

select 3
end

else if(@chvnOperationType = 'EXPORT')
begin 

select UserId,UserName,EmailId,MobileNumber,Gender,Age,PaymentAmount,convert(nvarchar(520),PaymentDate,103) as PaymentDate,
case 
when Isdeleted=0 then 'Active User' 
when Isdeleted=1 then 'Inactive User' 
end as [Current Status],convert(nvarchar(520),ModifiedDate,103) as ModifiedDate
from Users order by UserId desc
end

end