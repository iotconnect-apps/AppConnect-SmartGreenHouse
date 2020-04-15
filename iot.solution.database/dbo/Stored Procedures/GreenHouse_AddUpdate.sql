
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)	
	,@newid		UNIQUEIDENTIFIER
EXEC [dbo].[GreenHouse_AddUpdate]	
	@companyGuid	= '2D442AEA-E58B-4E8E-B09B-5602E1AA545A'
	,@Guid			= '98611812-0DB2-4183-B352-C3FEC9A3D1A4'
	,@name			= 'New Factory'
	,@description	= 'New Factory Description'
	,@address		= 'AAGreenhouse222'
	,@address2		= 'AAGreenhouse444'
	,@city			= 'AAGreenhouse'
	,@stateGuid		= '0f307bad-1e81-48f3-8dcc-92af2862e223'	
	,@countryGuid	= '3062e60c-361a-4f02-91ee-82134e0928f4'	
	,@zipCode		= '111111'	
	,@image			= NULL
	,@latitude		= NULL
	,@longitude		= NULL
	,@invokingUser	= 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'              
	,@version		= 'v1'              
	,@newid			= @newid		OUTPUT
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName, @newid newid

001	sgh-1 28-11-2019 [Nishit Khakhi]	Added Initial Version to Add GreenHouse
*******************************************************************/
CREATE PROCEDURE [dbo].[GreenHouse_AddUpdate]
(	@companyGuid	UNIQUEIDENTIFIER
	,@guid			UNIQUEIDENTIFIER
	,@name			NVARCHAR(500)
	,@description	NVARCHAR(1000)	
	,@address		NVARCHAR(500)
	,@address2		NVARCHAR(500)		= NULL
	,@city			NVARCHAR(50)		= NULL
	,@stateGuid		UNIQUEIDENTIFIER	= NULL
	,@countryGuid	UNIQUEIDENTIFIER	= NULL
	,@zipCode		NVARCHAR(10)		= NULL
	,@image			NVARCHAR(250)		= NULL
	,@latitude		NVARCHAR(50)		= NULL
	,@longitude		NVARCHAR(50)		= NULL
	,@invokingUser	UNIQUEIDENTIFIER
	,@version		nvarchar(10)    
	,@newid			UNIQUEIDENTIFIER	OUTPUT
	,@output		SMALLINT			OUTPUT    
	,@fieldName		nvarchar(100)		OUTPUT   
	,@culture		nvarchar(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)			= '0'
)	
AS
BEGIN
	SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'GreenHouse_Add' AS '@procName'             
            	, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
            	, CONVERT(nvarchar(MAX),@guid) AS '@guid' 
				, @name AS '@name' 
				, @description AS '@description' 
				, @address AS '@address'
				, @address2 AS '@address2'			
				, @city AS '@city'	
				, CONVERT(nvarchar(MAX),@stateGuid) AS '@stateGuid'	
				, CONVERT(nvarchar(MAX),@countryGuid) AS '@countryGuid'	
				, @zipCode AS '@zipCode'
				, @image AS '@image'
				, @latitude AS '@latitude'
				, @longitude AS '@longitude'
				, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            	, CONVERT(nvarchar(MAX),@version) AS '@version' 
            	, CONVERT(nvarchar(MAX),@output) AS '@output' 
            	, @fieldName AS '@fieldName'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END       
	
	DECLARE @dt DATETIME = GETUTCDATE()
	DECLARE @poutput	SMALLINT,@pFieldName	nvarchar(100)
    
	  IF(@poutput!=1)
      BEGIN
        SET @output = @poutput
        SET @fieldName = @pfieldName
        RETURN;
      END
	SET @newid = @guid

	SET @output = 1
	SET @fieldName = 'Success'

	BEGIN TRY

		IF NOT EXISTS(SELECT TOP 1 1 FROM [GreenHouse] where [guid] = @guid and companyGuid = @companyGuid and isdeleted = 0)
		BEGIN
			IF EXISTS (SELECT TOP 1 1 FROM [GreenHouse] (NOLOCK) WHERE companyguid = @companyguid AND [isdeleted]=0 AND [name]=@name)
			BEGIN
				SET @output = -3
				SET @fieldname = 'GreenHouseNameAlreadyExists'		 
				RETURN;
			END
		END		
		
	BEGIN TRAN	
		IF NOT EXISTS(SELECT TOP 1 1 FROM [GreenHouse] where [guid] = @guid and companyGuid = @companyGuid AND isdeleted = 0 )
		BEGIN	
			INSERT INTO dbo.[GreenHouse](
				[guid]			
				,[name]
				,[description]
				,[companyGuid]
				,[address]
				,[address2]
				,[city]
				,[stateGuid]
				,[countryGuid]
				,[zipCode]
				,[image]
				,[latitude]
				,[longitude]
				,[isActive]
				,[isDeleted]
				,[createddate]
				,[createdby]
				,[updatedDate]
				,[updatedBy]
				)
			VALUES(@guid			
				,@name
				,@description
				,@companyGuid
				,@address
				,@address2
				,@city
				,@stateGuid
				,@countryGuid
				,@zipCode
				,@image
				,@latitude
				,@longitude
				,1
				,0			
				,@dt
				,@invokingUser
				,@dt
				,@invokingUser
			)
		END
		ELSE
		BEGIN
			UPDATE dbo.[GreenHouse]
			SET
				[name] 			= @name
				,[description]	= @description
				,[address]		= @address
				,[address2]		= @address2
				,[city]			= @city
				,[stateGuid]	= @stateGuid
				,[countryGuid]	= @countryGuid
				,[zipCode]		= @zipCode
				,[image]		= @image
				,[latitude]		= @latitude
				,[longitude]	= @longitude
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
			WHERE
				[guid] = @guid
				AND [companyGuid] = @companyGuid
				AND [isDeleted] = 0
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

