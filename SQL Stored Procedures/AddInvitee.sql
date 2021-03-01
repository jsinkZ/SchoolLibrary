USE [SchoolLibrary3_0]
GO

/****** Object:  StoredProcedure [dbo].[AddInvitee]    Script Date: 01.03.2021 15:26:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[AddInvitee]
@Email varchar(50),
@TheToken varchar(MAX),
@RoleName varchar(20)
as
begin
	declare @existense varchar(50) = ''
	select @existense = Email from Invities where Email=@Email
	if @existense <> '' delete from Invities where Email=@existense
	insert into Invities values(@Email, @TheToken, GETDATE(), @RoleName)
end
GO

