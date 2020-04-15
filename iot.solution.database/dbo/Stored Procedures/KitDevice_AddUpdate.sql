
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)
	
EXEC [dbo].[KitDevice_AddUpdate]
	@data			= '
	<KitVerifyRequest>
	  <HardwareKits>
		<HardwareKitRequest>
		  <KitCode>hwpriya</KitCode>
		  <KitDevices>
			<KitDeviceRequest>
			  <ParentUniqueId />
			  <UniqueId>hwpriya1</UniqueId>
			  <Name>hwpriya1</Name>
			  <Note />
			  <Tag />
			</KitDeviceRequest>
			<KitDeviceRequest>
			  <ParentUniqueId>hwpriya1</ParentUniqueId>
			  <UniqueId>hwpriya2</UniqueId>
			  <Name>hwpriya2</Name>
			  <Note />
			  <Tag>env</Tag>
			  <AttributeName>nutrient.P</AttributeName>
			</KitDeviceRequest>
		  </KitDevices>
		</HardwareKitRequest>
	  </HardwareKits>
	  <KitTypeGuid>938B3485-9970-4A8A-9C25-600FF8503DBB</KitTypeGuid>
	</KitVerifyRequest>'
	,@isEdit =1
	,@invokingUser	= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version		= 'v1'
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT
	
SELECT @output status, @fieldName fieldname

001	SGH-97	23-01-2020	[Nishit Khakhi]	Added Initial Version to Add Update Kit Device
002	SGH-145	06-02-2020	[Nishit Khakhi]	Updated to Delete old kit Device which are not in Data
003	SGH-145	05-03-2020	[Nishit Khakhi]	Updated to store TagGuid based on AttributeName and Tag
*******************************************************************/

create PROCEDURE [dbo].[KitDevice_AddUpdate]
(	@data				XML
	,@isedit			BIT					= 0
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT			OUTPUT
	,@fieldName			NVARCHAR(100)		OUTPUT
	,@culture			NVARCHAR(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)				= '0'
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
            SELECT 'KitDevice_AddUpdate' AS '@procName'
			, CONVERT(nvarchar(MAX),@data) AS '@data'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	SET @output = 1
	SET @fieldName = 'Success'
	
	BEGIN TRY
		DECLARE @kitTypeGuid UNIQUEIDENTIFIER
		
		SELECT TOP 1 @kitTypeGuid = TRY_CONVERT(UNIQUEIDENTIFIER, x.y.value('KitTypeGuid[1]', 'NVARCHAR(50)')) 
		FROM @data.nodes('/KitVerifyRequest') x(y) 

		IF OBJECT_ID ('tempdb..#hardwareKit_data') IS NOT NULL DROP TABLE #hardwareKit_data 
		IF OBJECT_ID ('tempdb..#HardwareKit_InsertedData') IS NOT NULL DROP TABLE #HardwareKit_InsertedData 
		IF OBJECT_ID ('tempdb..#device_data') IS NOT NULL DROP TABLE #device_data 

		CREATE TABLE #HardwareKit_InsertedData
		(	[kitGuid]		UNIQUEIDENTIFIER
			,[kitTypeGuid]	UNIQUEIDENTIFIER
			,[kitCode]		NVARCHAR(50)
		)
		CREATE TABLE #hardwareKit_data
		(	[kitGuid]		UNIQUEIDENTIFIER
			,[kitTypeGuid]	UNIQUEIDENTIFIER
			,[kitCode]		NVARCHAR(50)
		)

		CREATE TABLE #device_data
		(	[guid]				UNIQUEIDENTIFIER
			,[kitCode]			NVARCHAR(50)
			,[kitGuid]			UNIQUEIDENTIFIER
			,[parentUniqueId]	NVARCHAR(500)
			,[parentDeviceGuid]	UNIQUEIDENTIFIER
			,[uniqueId]			NVARCHAR(500)
			,[name]				NVARCHAR(500)
			,[note]				NVARCHAR(1000)
			,[tag]				NVARCHAR(50)
			,[isProvisioned]	BIT
			,[attributeName]	NVARCHAR(100)
		)

		INSERT INTO #hardwareKit_data
		SELECT ISNULL(H.[guid],NEWID()) AS [kitGuid], A.* FROM
		(	
			SELECT DISTINCT 
				@kitTypeGuid AS 'KitTypeGuid',
				x.R.query('../../KitCode').value('.', 'NVARCHAR(50)') AS 'KitCode'
			FROM @data.nodes('/KitVerifyRequest/HardwareKits/HardwareKitRequest/KitDevices/KitDeviceRequest') as x(R)
		) A
		LEFT JOIN [dbo].[HardwareKit] H (NOLOCK) ON A.[kitCode] = H.[kitCode] AND H.[isDeleted] = 0
		
		INSERT INTO #device_data
		SELECT DISTINCT NEWID() AS [guid]
			, x.R.query('../../KitCode').value('.', 'NVARCHAR(50)') AS [kitCode]
			, NULL AS [kitGuid]
			, x.R.query('./ParentUniqueId').value('.', 'NVARCHAR(500)') AS [parentUniqueId]
			, NULL AS [parentDeviceGuid]
			, x.R.query('./UniqueId').value('.', 'NVARCHAR(500)') AS [uniqueId]
			, x.R.query('./Name').value('.', 'NVARCHAR(500)') AS [name]
			, x.R.query('./Note').value('.', 'NVARCHAR(1000)') AS [note]
			, x.R.query('./Tag').value('.', 'NVARCHAR(50)') AS [tag]
			, 0 AS [isProvisioned]
			, x.R.query('./AttributeName').value('.', 'NVARCHAR(50)') AS [attributeName]
		FROM @data.nodes('/KitVerifyRequest/HardwareKits/HardwareKitRequest/KitDevices/KitDeviceRequest') as x(R)
		
		BEGIN TRAN

			UPDATE D
			SET [kitGuid] = H.[kitGuid]
			FROM #device_data D
			INNER JOIN #hardwareKit_data H ON D.[kitCode] = H.[kitCode] 

			UPDATE DD
			SET parentDeviceGuid = ISNULL(D.[guid],D1.[guid])
			FROM #device_data DD
			LEFT JOIN #device_data D1 ON DD.[parentUniqueId] = D1.[uniqueId] AND DD.[kitGuid] = D1.[kitGuid]
			LEFT JOIN dbo.[KitDevice] D (NOLOCK) ON DD.[parentUniqueId] = D.[uniqueId] AND DD.[kitGuid] = D.[kitGuid]
			WHERE DD.[parentUniqueId] IS NOT NULL

			IF ( @isedit != 1 )
			BEGIN 
			INSERT INTO [dbo].[HardwareKit] ([guid],[kitTypeGuid],[kitCode],[isActive],[isDeleted],[createdDate],[createdBy],[updatedDate],[updatedBy])
			OUTPUT inserted.[guid], inserted.[kitTypeGuid], inserted.[kitCode] INTO #HardwareKit_InsertedData
			SELECT 
				TH.[kitGuid]
				,TH.[kitTypeGuid]
				,TH.[kitCode]
				,1
				,0
				,@dt
				,@invokingUser
				,@dt
				,@invokingUser
			FROM #hardwareKit_data TH
			LEFT JOIN [dbo].[HardwareKit] H (NOLOCK) ON TH.[kitCode] = H.[kitCode] AND H.isDeleted = 0
			WHERE H.[guid] IS NULL

			END
			ELSE
			BEGIN
				INSERT INTO #HardwareKit_InsertedData
				SELECT h.[guid]		
					,h.[kitTypeGuid]	
					,h.[kitCode]		
				FROM [dbo].[HardwareKit] h (NOLOCK)
				INNER JOIN #hardwareKit_data hd ON h.[kitCode] = hd.[kitCode]
				WHERE h.[isDeleted] = 0
			END

			-- Delete Device Which are not in Data
			UPDATE KD
			SET [isDeleted] = 1
				,[updatedBy] = @invokingUser
				,[updatedDate] = @dt
			FROM [dbo].[KitDevice] KD
			INNER JOIN #hardwareKit_data H ON KD.[kitGuid] = H.[kitGuid]
			LEFT JOIN #device_data DD ON KD.[uniqueId] = DD.[uniqueId] AND KD.[isDeleted] = 0
			WHERE DD.[guid] IS NULL

			-- Insert New Device
			INSERT INTO [dbo].[KitDevice]
			SELECT
				DD.[guid]
				,DD.[kitGuid]
				,DD.[parentDeviceGuid]
				,DD.[uniqueId]
				,DD.[name]
				,DD.[note]
				,KTA.[guid]
				,DD.[isProvisioned]
				,1
				,0
				,@dt
				,@invokingUser
				,@dt
				,@invokingUser
			FROM #device_data DD
			INNER JOIN #hardwareKit_data H ON DD.kitGuid = H.kitGuid
			LEFT JOIN [dbo].[KitDevice] KD (NOLOCK) ON KD.[kitGuid] = DD.[kitGuid] AND KD.[uniqueId] = DD.[uniqueId] AND KD.[isDeleted] = 0
			LEFT JOIN [dbo].[KitTypeAttribute] KTA (NOLOCK) ON H.[kitTypeGuid] = KTA.[kittypeGuid] AND DD.[attributeName] = KTA.[localName] AND KTA.[tag] = DD.[tag]
			WHERE KD.[guid] IS NULL

			-- Update Existsing Device
			UPDATE KD
			SET [name] = DD.[name]
				,[note] = DD.[note]
				,[isProvisioned] = DD.[isProvisioned]
				,[parentDeviceGuid] = DD.[parentDeviceGuid]
				,[tagGuid] = KTA.[guid]
			FROM [dbo].[KitDevice] KD
			INNER JOIN #device_data DD ON KD.[kitGuid] = DD.[kitGuid] AND KD.[uniqueId] = DD.[uniqueId] AND KD.[isDeleted] = 0
			INNER JOIN #hardwareKit_data H ON DD.kitGuid = H.kitGuid
			LEFT JOIN [dbo].[KitTypeAttribute] KTA (NOLOCK) ON H.[kitTypeGuid] = KTA.[kittypeGuid] AND DD.[attributeName] = KTA.[localName] AND KTA.[tag] = DD.[tag]
			
		COMMIT TRAN
		--SELECT * FROM #hardwareKit_data
		--SELECT * FROM #device_data

		SELECT * FROM #HardwareKit_InsertedData
		
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

