
/*******************************************************************
DECLARE 
     @output INT = 0
	,@fieldName				nvarchar(255)
EXEC [dbo].[Device_Lookup]		
	@companyGuid		= '2D442AEA-E58B-4E8E-B09B-5602E1AA545A'
	,@greenHouseGuid	= 'FE9DFC47-1EDC-467E-881A-260B5DBAE0ED'	
	,@parentDeviceGuid  = 'B2BFB919-0A68-41A3-BF90-37B688101EF9'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
               
 SELECT @output status,  @fieldName AS fieldName    
 
 001	sgh-1 05-12-2019 [Nishit Khakhi]	Added Initial Version to Lookup Device
*******************************************************************/

CREATE PROCEDURE [dbo].[Device_Lookup]
(	 @companyGuid		UNIQUEIDENTIFIER
	,@greenHouseGuid	UNIQUEIDENTIFIER = NULL
	,@parentDeviceGuid	UNIQUEIDENTIFIER = NULL
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT	
	,@culture			NVARCHAR(10)	  = 'en-Us'
	,@enableDebugInfo	CHAR(1)			  = '0'
)
AS
BEGIN
    SET NOCOUNT ON
	
	IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'Device_Lookup' AS '@procName'			
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX),@greenHouseGuid) AS '@greenHouseGuid'
			, CONVERT(nvarchar(MAX),@parentDeviceGuid) AS '@parentDeviceGuid'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    Set @output = 1
    SET @fieldName = 'Success'
	
	IF @parentDeviceGuid IS NULL
	BEGIN
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Device] (NOLOCK) 
									WHERE companyGuid=@companyGuid AND [greenHouseGuid] = ISNULL(@greenHouseGuid,[greenHouseGuid])
										AND [parentDeviceGuid] IS NULL AND [isDeleted] = 0 AND [isDeleted]=0
					   )
		BEGIN
			Set @output = -3
			SET @fieldName = 'DeviceNotFound'
			RETURN;
		END
	END
	ELSE
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Device] (NOLOCK) 
									WHERE companyGuid=@companyGuid AND [greenHouseGuid] = ISNULL(@greenHouseGuid,[greenHouseGuid])
										AND [parentDeviceGuid] = @parentDeviceGuid AND [isDeleted] = 0 AND [isDeleted]=0
					)
		BEGIN
			Set @output = -3
			SET @fieldName = 'DeviceNotFound'
			RETURN;
		END

    BEGIN TRY
		IF @parentDeviceGuid IS NULL
		BEGIN
			SELECT [guid]
			  ,[name]
			FROM [dbo].[Device] (NOLOCK) 
			WHERE [companyguid]=@companyGuid 
				AND [greenHouseGuid] = ISNULL(@greenHouseGuid,[greenHouseGuid])
				AND [parentDeviceGuid] IS NULL
				AND [isDeleted] = 0 
			ORDER BY [name]
		END
		ELSE
		BEGIN
			SELECT [guid]
			  ,[name]
			FROM [dbo].[Device] (NOLOCK) 
			WHERE [companyguid]=@companyGuid 
				AND [greenHouseGuid] = ISNULL(@greenHouseGuid,[greenHouseGuid])
				AND [parentDeviceGuid] = @parentDeviceGuid
				AND [isDeleted] = 0 
			ORDER BY [name]
		END

	END TRY
	BEGIN CATCH
		DECLARE @errorReturnMessage nvarchar(MAX)

		SET @output = 0

		SELECT @errorReturnMessage =
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   +
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as nvarchar), '')  +
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as nvarchar), '') +
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as nvarchar), '') +
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as nvarchar), '') +
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as nvarchar), '') +
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as nvarchar(max)), '')
		RAISERROR (@errorReturnMessage, 11, 1)

		IF (XACT_STATE()) = -1
		BEGIN
			ROLLBACK TRANSACTION
		END
		IF (XACT_STATE()) = 1
		BEGIN
			ROLLBACK TRANSACTION
		END
		RAISERROR (@errorReturnMessage, 11, 1)
	END CATCH
END

