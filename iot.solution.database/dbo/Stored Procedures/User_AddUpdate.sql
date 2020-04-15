
/*******************************************************************
BEGIN TRAN
DECLARE @output INT = 0 
	,@fieldname	nvarchar(255) 
	
EXEC [dbo].[User_AddUpdate]	
	@isEdit				= 1
	,@email				= 'test@gmail.com'    
	,@companyGuid		='BE89F822-F858-46F8-AD78-5EA63BC67B33' 
	,@greenhouseGuid	='BE89F822-F858-46F8-AD78-5EA63BC67B33' 
	,@roleGuid			='BE89F822-F858-46F8-AD78-5EA63BC67B33' 
	,@firstName			= 'Long CPID Test 1'    
	,@lastName			= 'Long CPID Test 1'    
	,@timezoneGuid		='31C6A008-B76F-423C-8F3F-D330CF1F5AF0'
	,@imageName			='1-9845685452'
	,@contactNo			='1-9845685452'
	,@invokinguser		= '3963F124-A2B7-48CE-A9EB-EB1C205EE44F'
	,@version			= 'v1'              
	,@output			= @output		OUTPUT
	,@fieldname			= @fieldname	OUTPUT	

SELECT @output status, @fieldname fieldname

ROLLBACK

001	SGH-145	07-02-2020 [Nishit Khakhi]	Added Initial Version to Add Update User

*******************************************************************/

CREATE PROCEDURE [dbo].[User_AddUpdate]
(	@email					nvarchar(100)	
	,@companyGuid			UNIQUEIDENTIFIER	
	,@greenhouseGuid		UNIQUEIDENTIFIER	= NULL
	,@roleGuid			    UNIQUEIDENTIFIER	= NULL
	,@firstName				nvarchar(50)		= NULL
	,@lastName				nvarchar(50)		= NULL
	,@timezoneGuid			UNIQUEIDENTIFIER	= NULL
	,@imageName				nvarchar(100)		= NULL	
	,@contactNo				nvarchar(25)		= NULL	
	,@isEdit				BIT					= 0
	,@invokinguser			UNIQUEIDENTIFIER		
	,@version				nvarchar(10)    
	,@output				SMALLINT			OUTPUT
	,@fieldname				nvarchar(100)		OUTPUT
	,@culture				nvarchar(10)		= 'en-Us'
	,@enabledebuginfo		CHAR(1)				= '0'
)	
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()				
    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'User_AddUpdate' AS '@procName'             
            , @email AS '@email' 
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'			
			, CONVERT(nvarchar(MAX),@greenhouseGuid) AS '@greenhouseGuid' 
			, CONVERT(nvarchar(MAX),@roleGuid) AS '@roleGuid' 
			, @firstName AS '@firstName' 
			, @lastName AS '@lastName' 
			, CONVERT(nvarchar(MAX),@timezoneGuid) AS '@timezoneGuid' 
			, @imageName AS '@postalCode' 			
			, @contactNo AS '@contactNo' 			
			, CONVERT(nvarchar(1),@isEdit) AS '@isEdit' 
			, CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser'            
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@output) AS '@output' 
            , CONVERT(nvarchar(MAX),@fieldname) AS '@fieldname'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END       
	
	SET @output = 1
	SET @fieldname = 'Success'

	BEGIN TRY
		IF @isEdit = 0
		BEGIN
			IF EXISTS(SELECT TOP 1 1 FROM [User] (NOLOCK) WHERE [email] = @email AND [isdeleted] = 0)
			BEGIN
				SET @output = -3
				SET @fieldname = 'UserAlreadyExists'			
				RETURN;
			END
		END
		
	BEGIN TRAN	
		IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[User] (NOLOCK) where [email] = @email AND [isdeleted] = 0)
		BEGIN	
			INSERT INTO [dbo].[User]([guid]
			,[email]
			,[companyGuid]
			,[greenHouseGuid]
			,[roleGuid]
			,[firstName]
			,[lastName]
			,[timeZoneGuid]
			,[imageName]
			,[contactNo]
			,[isActive]
			,[isDeleted]
			,[createdDate]
			,[createdBy]
			,[updatedDate]
			,[updatedBy]
			)
			VALUES(
			NEWID()
			,@email
			,@companyGuid
			,@greenhouseGuid
			,@roleGuid
			,@firstName
			,@lastName
			,@timezoneGuid
			,@imageName
			,@contactNo
			,1
			,0
			,@dt
			,@invokinguser
			,@dt
			,@invokinguser	
			)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[User] 
			SET [firstname]			= @firstname
				,[lastname]			= @lastname
				,[timezoneguid]		= @timezoneguid
				,[contactno]		= @contactno
				,[greenhouseGuid]	= @greenhouseGuid  
				,[updatedby]		= @invokinguser
				,[updateddate]		= @dt				
			WHERE [email] = @email AND [isDeleted] = 0 
		END

	COMMIT TRAN	

	END TRY 	
	BEGIN CATCH
	SET @output = 0
	DECLARE @errorReturnMessage nvarchar(MAX)

	SELECT
		@errorReturnMessage = ISNULL(@errorReturnMessage, ' ') + SPACE(1) +
		'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), ' ') +
		'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), ' ') +
		'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), ' ') +
		'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), ' ') +
		'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), ' ') +
		'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(MAX)), ' ')

	RAISERROR (@errorReturnMessage
	, 11
	, 1
	)

	IF (XACT_STATE()) = -1 BEGIN
		ROLLBACK TRANSACTION
	END
	IF (XACT_STATE()) = 1 BEGIN
		ROLLBACK TRANSACTION
	END
	END CATCH
END

