
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)
	,@newid UNIQUEIDENTIFIER

EXEC [dbo].[Device_AddUpdate]
	@guid					= '895019CF-1D3E-420C-828F-8971253E5784'
	,@companyGuid			= '895019CF-1D3E-420C-828F-8971253E5784'
	,@greenHouseGuid		= '5330D171-DA1E-4554-A868-87D222CD5D35'
	,@templateGuid			= '12A5CD86-F6C6-455F-B27A-EFE587ED410D'
	,@parentDeviceGuid		= '12A5CD86-F6C6-455F-B27A-EFE587ED410D'
	,@type					= 1
	,@uniqueId				= 'Device656 uniqueId'
	,@name					= 'Device 656 name'
	,@note					= 'Device 656 note'
	,@tag					= 'Device 656 tag'
	,@image					= 'sdfsdfsdfsdf.jpg'
	,@isProvisioned			= 0
	,@invokingUser			= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version				= 'v1'
	,@output				= @output		OUTPUT
	,@fieldName				= @fieldName	OUTPUT
	,@newid					= @newid		OUTPUT

SELECT @output status, @fieldName fieldname,@newid newid

001	SGH-1	05-12-2019 [Nishit Khakhi]	Added Initial Version to Add Device
002	SGH-97	13-01-2020[Nishit Khakhi]	Updated to merge Add Update call
*******************************************************************/

CREATE PROCEDURE [dbo].[Device_AddUpdate]
(	@guid				UNIQUEIDENTIFIER
	,@companyGuid		UNIQUEIDENTIFIER
	,@greenHouseGuid	UNIQUEIDENTIFIER
	,@templateGuid		UNIQUEIDENTIFIER
	,@parentDeviceGuid	UNIQUEIDENTIFIER	= NULL
	,@type				TINYINT				= NULL
	,@uniqueId			NVARCHAR(500)
	,@name	 			NVARCHAR(500)
	,@note				NVARCHAR(1000)		= NULL
	,@tag				NVARCHAR(50)		= NULL
	,@image				NVARCHAR(200)		= NULL
	,@isProvisioned		BIT					= 0
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			NVARCHAR(10)
	,@output			SMALLINT			OUTPUT
	,@fieldName			NVARCHAR(100)		OUTPUT
	,@newid				UNIQUEIDENTIFIER   	OUTPUT
	,@culture			NVARCHAR(10)		= 'en-Us'
	,@enableDebugInfo	 CHAR(1)			= '0'
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'Device_AddUpdate' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX),@greenHouseGuid) AS '@greenHouseGuid'
			, CONVERT(nvarchar(MAX),@templateGuid) AS '@templateGuid'
			, CONVERT(nvarchar(MAX),@parentDeviceGuid) AS '@parentDeviceGuid'
			, CONVERT(nvarchar(MAX),@type) AS '@type'
			, @uniqueId AS '@uniqueId'
            , @name AS '@name'
			, @note AS '@note'
			, @tag AS '@tag'
			, @image AS '@image'
			, CONVERT(nvarchar(MAX),@isProvisioned) AS '@isProvisioned'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			, CONVERT(nvarchar(MAX),@guid) AS '@newid'
            	FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	SET @output = 1
	SET @fieldName = 'Success'
	
	BEGIN TRY
		
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[GreenHouse] (NOLOCK) WHERE [guid]=@greenHouseGuid AND [companyGuid]=@companyGuid AND [isDeleted]=0)
		BEGIN
			Set @output = -3
			SET @fieldname = 'GreenHouseNotFound'
			RETURN;
		END  
		IF EXISTS (SELECT TOP 1 1 FROM [dbo].[Device] (NOLOCK) WHERE [guid] = @guid AND [uniqueId]=@uniqueId AND [companyGuid]=@companyGuid AND [isDeleted]=0)
		BEGIN
			Set @output = -3
			SET @fieldname = 'UniqueidAlreadyExist'
			RETURN;
		END  
		
		
		SET @newid = @guid
		BEGIN TRAN
			IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[Device] (NOLOCK) where [guid] = @guid AND [uniqueId] = @uniqueId and companyGuid = @companyGuid AND isdeleted = 0)
			BEGIN	
				INSERT INTO [dbo].[Device]
			           ([guid]
			           ,[companyGuid]
			           ,[greenhouseGuid]
			           ,[templateGuid]
					   ,[parentDeviceGuid]
			           ,[type]
			           ,[uniqueId]
			           ,[name]
					   ,[note]
					   ,[tag]
					   ,[image]
					   ,[isProvisioned]
			           ,[isActive]
					   ,[isDeleted]
			           ,[createdDate]
			           ,[createdBy]
			           ,[updatedDate]
			           ,[updatedBy]
						)
			     VALUES
			           (@guid
			           ,@companyGuid
			           ,@greenHouseGuid
			           ,@templateGuid
					   ,@parentDeviceGuid
			           ,@type
			           ,@uniqueId
			           ,@name
			           ,@note
			           ,@tag
			           ,@image
					   ,@isProvisioned
					   ,1
			           ,0
			           ,@dt
			           ,@invokingUser				   
					   ,@dt
					   ,@invokingUser				   
				       );
			END
			ELSE
			BEGIN
				UPDATE [dbo].[Device]
				SET	[greenhouseGuid] = @greenHouseGuid
					,[parentDeviceGuid] = @parentDeviceGuid
					,[templateGuid] = @templateGuid
					,[name] = @name
					,[tag] = ISNULL(@tag,[tag])
					,[note] = ISNULL(@note,[note])
					,[isProvisioned] = @isProvisioned
					,[updatedDate] = @dt
					,[updatedBy] = @invokingUser
				WHERE [guid] = @guid AND [uniqueId] = @uniqueId AND [companyGuid] = @companyGuid AND [isDeleted] = 0
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

