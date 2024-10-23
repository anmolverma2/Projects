
CREATE TABLE [dbo].[Students](
	[Std_Id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Age] [int] NOT NULL,
	[ImagePath] [nvarchar](max) NULL,
	[Class] [int] NULL,
	[RollNumber] [nvarchar](50) NULL,
	[Isdeleted] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[SubjectId] [int] NULL
)


CREATE TABLE [dbo].[Subjects](
	[Id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Class] [int] NOT NULL,
	[Language] [nvarchar](100) NOT NULL,
	[Isdeleted] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[TeacherId] [int] NULL
)


CREATE TABLE [dbo].[Teachers](
	[T_Id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ImagePath] [nvarchar](max) NULL,
	[Age] [int] NOT NULL,
	[Sex] [nvarchar](10) NOT NULL,
	[Isdeleted] [bit] NULL,
	[CreatedDate] [datetime] NULL
)


create proc [dbo].[SP_Details]
(

@Name nvarchar(1024) = null,
@operationtype nvarchar(1024) = null)

as begin

begin try

begin transaction

if(@operationtype = 'List')
begin

select t.Name as TeacherName,sb.Name as SubjectName,sb.Language,st.name as StudentName,st.Class
from teachers t inner join Subjects sb on t.T_Id=sb.TeacherId inner join students st on sb.Id=st.SubjectId
order by st.name asc


end



commit transaction
end try
begin catch

rollback

select 0

end catch


end



create proc [dbo].[SP_Students]-- @operationtype='list',@Name='',@className='9'
(

@Name nvarchar(1024) = null,
@Age int = 0,
@Class int = 0,
@SubjectId int = 0,
@RollNumber nvarchar(1024) = null,
@Image nvarchar(1024) = null,
@className nvarchar(1024) = null,
@operationtype nvarchar(1024) = null)

as begin

begin try

begin transaction

if(@operationtype = 'List')
begin

select * from Students where isdeleted = 0 and (@Name is null or @Name = '' or [Name] like '%'+@Name+'%') 
and (@className is null or @className = '' or class = (cast(@className as int) ))  order by class

end

if(@operationtype = 'Insert')
begin

insert into Students(Name, Age, ImagePath, Class, RollNumber,SubjectId,Isdeleted,CreatedDate)
values (@Name,@Age,@Image,@Class,@RollNumber,@SubjectId,0,getdate())

select 1
end


commit transaction
end try
begin catch

rollback

select 0

end catch


end


create proc [dbo].[SP_Subjects]
(

@Name nvarchar(1024) = null,
@Class nvarchar(1024) = null,
@Language nvarchar(1024) = null,
@TeacherId int = null,
@operationtype nvarchar(1024) = null)

as begin

begin try

begin transaction

if(@operationtype = 'Insert')
begin

insert into Subjects(Name,Class,Language,TeacherId,Isdeleted,CreatedDate)
values (@Name,cast(@Class as int),@Language,@TeacherId,0,getdate())

select 1
end


if(@operationtype = 'Teacher')
begin

select * from Teachers order by name asc
end


if(@operationtype = 'Subject')
begin

select * from Subjects order by name asc
end

commit transaction
end try
begin catch

rollback

select 0

end catch


end


create proc [dbo].[SP_Teachers]
(

@Name nvarchar(1024) = null,
@Age int = 0,
@Sex nvarchar(1024) = null,
@RollNumber nvarchar(1024) = null,
@Image nvarchar(1024) = null,
@operationtype nvarchar(1024) = null)

as begin

begin try

begin transaction

if(@operationtype = 'Insert')
begin

insert into Teachers(Name, Age, ImagePath, Sex,Isdeleted,CreatedDate)
values (@Name,@Age,@Image,@Sex,0,getdate())

select 1
end


commit transaction
end try
begin catch

rollback

select 0

end catch


end