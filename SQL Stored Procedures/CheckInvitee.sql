USE [SchoolLibrary3_0]
GO

DECLARE @RC int
DECLARE @TheToken varchar(max)
DECLARE @Email varchar(50)
DECLARE @RoleName varchar(20)

-- TODO: Set parameter values here.

EXECUTE @RC = [dbo].[CheckInvitee] 
   @TheToken
  ,@Email OUTPUT
  ,@RoleName OUTPUT
GO

