
/*******************************************************************

EXEC [dbo].[KitDevice_Validate]	
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
	,@isEdit		= 0
	,@invokinguser  = '200EDCFA-8FF1-4837-91B1-7D5F967F5129'   
	,@version		= 'v1'              


001	sgh-97 03-02-2020 [Nishit Khakhi]	Added Initial Version to Validate Device & Kit
*******************************************************************/

CREATE PROCEDURE [dbo].[KitDevice_Validate]
(	@data				XML
	,@isEdit			BIT					= 0
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
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
            SELECT 'KitDevice_Validate' AS '@procName'
			, CONVERT(nvarchar(MAX),@data) AS '@data'
			, CONVERT(nvarchar(MAX),@isEdit) AS '@isEdit'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
		    FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    
	BEGIN TRY
		DECLARE @orderBy VARCHAR(10), @kitTypeGuid UNIQUEIDENTIFIER

		SELECT TOP 1 @kitTypeGuid = TRY_CONVERT(UNIQUEIDENTIFIER, x.y.value('KitTypeGuid[1]', 'NVARCHAR(50)')) 
		FROM @data.nodes('/KitVerifyRequest') x(y) 

		IF OBJECT_ID ('tempdb..#hardwareKit_data') IS NOT NULL DROP TABLE #hardwareKit_data 
		IF OBJECT_ID ('tempdb..#generator_data') IS NOT NULL DROP TABLE #generator_data 

		CREATE TABLE #hardwareKit_data
		(	[kitGuid]		UNIQUEIDENTIFIER
			,[kitTypeGuid]	UNIQUEIDENTIFIER
			,[kitCode]		NVARCHAR(50)
			,[errorMessage]	NVARCHAR(300)
			,[parentDeviceCount]	BIGINT	
		)

		CREATE TABLE #generator_data
		(	[guid]				UNIQUEIDENTIFIER
			,[kitCode]			NVARCHAR(50)
			,[kitGuid]			UNIQUEIDENTIFIER
			,[parentUniqueId]	NVARCHAR(500)
			,[parentGensetGuid]	UNIQUEIDENTIFIER
			,[uniqueId]			NVARCHAR(500)
			,[name]				NVARCHAR(500)
			,[note]				NVARCHAR(1000)
			,[tag]				NVARCHAR(50)
			,[isProvisioned]	BIT
			,[attributeName]	NVARCHAR(100)
			,[errorMessage]	NVARCHAR(300)
		)

		INSERT INTO #hardwareKit_data
		SELECT ISNULL(H.[guid],NEWID()) AS [kitGuid], A.*, NULL AS [errorMessage], 0 AS [deviceCount] FROM
		(	SELECT DISTINCT @kitTypeGuid AS 'KitTypeGuid',
				x.R.query('../../KitCode').value('.', 'NVARCHAR(50)') AS 'KitCode'
			FROM @data.nodes('/KitVerifyRequest/HardwareKits/HardwareKitRequest/KitDevices/KitDeviceRequest') as x(R)
		) A
		LEFT JOIN [dbo].[HardwareKit] H (NOLOCK) ON A.[kitCode] = H.[kitCode] AND H.[isDeleted] = 0

		INSERT INTO #generator_data
		SELECT DISTINCT NEWID() AS [guid]
			, x.R.query('../../KitCode').value('.', 'NVARCHAR(50)') AS [kitCode]
			, NULL AS [kitGuid]
			, CASE WHEN LEN(x.R.query('./ParentUniqueId').value('.', 'NVARCHAR(500)')) > 0 THEN x.R.query('./ParentUniqueId').value('.', 'NVARCHAR(500)') ELSE NULL END AS [parentUniqueId]
			, NULL AS [parentGensetGuid]
			, x.R.query('./UniqueId').value('.', 'NVARCHAR(500)') AS [uniqueId]
			, x.R.query('./Name').value('.', 'NVARCHAR(500)') AS [name]
			, x.R.query('./Note').value('.', 'NVARCHAR(1000)') AS [note]
			, x.R.query('./Tag').value('.', 'NVARCHAR(50)') AS [tag]
			, 0 AS [isProvisioned]
			, x.R.query('./AttributeName').value('.', 'NVARCHAR(50)') AS [attributeName]
			, NULL AS [errorMessage]
		FROM @data.nodes('/KitVerifyRequest/HardwareKits/HardwareKitRequest/KitDevices/KitDeviceRequest') as x(R)
		
		UPDATE D
		SET [kitGuid] = H.[kitGuid]
		FROM #generator_data D
		INNER JOIN #hardwareKit_data H ON D.[kitCode] = H.[kitCode] 

		UPDATE DD
		SET parentGensetGuid = ISNULL(D1.[guid],D.[guid])
		FROM #generator_data DD
		LEFT JOIN #generator_data D1 ON DD.[parentUniqueId] = D1.[uniqueId] AND DD.[kitGuid] = D1.[kitGuid]
		LEFT JOIN dbo.[KitDevice] D (NOLOCK) ON DD.[parentUniqueId] = D.[uniqueId] AND DD.[kitGuid] = D.[kitGuid]
		WHERE DD.[parentUniqueId] IS NOT NULL
		
		UPDATE hi
		SET [parentDeviceCount] = ISNULL(A.[parentDeviceCount],0)
		FROM #hardwareKit_data hi
		INNER JOIN (SELECT d.[kitGuid], COUNT(d.[guid]) AS [parentDeviceCount]
					FROM #generator_data d
					WHERE d.parentUniqueId IS NULL 
					GROUP BY d.[kitGuid] 
					HAVING COUNT(d.[guid]) > 0
					) A ON A.[kitGuid] = hi.[kitGuid]

		UPDATE H
		SET [errorMessage] = 'Invalid Kit Type'
		FROM #hardwareKit_data H
		LEFT JOIN [dbo].[KitType] K ON H.[kitTypeGuid] = K.[guid]
		WHERE K.[guid] IS NULL
		
		IF (@isedit = 0)
		BEGIN 
			UPDATE H
			SET [errorMessage] = 'Kit Already Exists'
			FROM #hardwareKit_data H
			INNER JOIN [dbo].[HardwareKit] HK (NOLOCK)
				ON H.[kitCode] = HK.[kitCode] AND HK.isDeleted=0 

			UPDATE #hardwareKit_data
			SET [errorMessage] = 'Kit Must Have One Gateway Device'
			WHERE [parentDeviceCount] = 0

			UPDATE #generator_data
			SET	[errorMessage] = 'UniqueId Must Be Unique' 					 
			WHERE [uniqueId] IN (SELECT [uniqueId] FROM #generator_data
			GROUP BY [uniqueId] having count(*) > 1)

			UPDATE dd
			SET	[errorMessage] = 'UniqueId Already Exists' 					 
			FROM #generator_data dd
			INNER JOIN KitDevice D (NOLOCK)
				ON dd.[uniqueId] = D.uniqueid
				AND d.isDeleted=0 
		END

		UPDATE dd
		SET	[errorMessage] = 'Invalid Attribute Name' 					 
		FROM #generator_data dd
		LEFT JOIN KitTypeAttribute K (NOLOCK) 
			ON dd.[attributeName] = K.[localName]
			AND dd.[tag] = K.[tag]
		WHERE K.[guid] IS NULL AND dd.[parentUniqueId] IS NOT NULL 

		SELECT H.[kitCode]
				,H.kitTypeGuid
				,H.[errorMessage] AS [kitError]
				,D.[guid]		
				,D.[parentUniqueId]	
				,D.[uniqueId]			
				,D.[name]				
				,D.[note]				
				,D.[tag]
				,D.[attributeName]
				,D.[errorMessage] AS [deviceError]	
		FROM #hardwareKit_data H
		INNER JOIN #generator_data D ON H.[kitGuid] = D.[kitGuid]

		IF EXISTS(SELECT TOP 1 1 FROM #hardwareKit_data WHERE [errorMessage] IS NOT NULL)
			OR
		   EXISTS(SELECT TOP 1 1 FROM #generator_data WHERE [errorMessage] IS NOT NULL)
		BEGIN
			SELECT 'false' AS isValid
		END
		ELSE
		BEGIN
			SELECT 'true' AS isValid
		END
		
	END TRY
	BEGIN CATCH
		DECLARE @errorReturnMessage nvarchar(MAX)

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