/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[Device_ListByGreenHouse]
	 @greenHouseGuid	= 'BC2C4B40-B169-4707-B60D-2AA599468EC7'
	,@pageSize			= 10
	,@pageNumber		= 1
	,@orderby			= NULL
	,@count				= @count OUTPUT
	,@invokingUser		= 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version			= 'v1'
	,@output			= @output	OUTPUT
	,@fieldName			= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SGH-1	07-02-2019 [Nishit Khakhi]	Added Initial Version to List Device By GreenHouse
002	SGH-145 05-03-2020 [Nishit Khakhi]	Updated to count child device count
*******************************************************************/
CREATE PROCEDURE [dbo].[Device_ListByGreenHouse]
(	@greenHouseGuid		UNIQUEIDENTIFIER
	,@search			VARCHAR(100)		= NULL
	,@pageSize			INT
	,@pageNumber		INT
	,@orderby			VARCHAR(100)		= NULL
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			VARCHAR(10)
	,@culture			VARCHAR(10)			= 'en-Us'
	,@output			SMALLINT			OUTPUT
	,@fieldName			VARCHAR(255)		OUTPUT
	,@count				INT OUTPUT
	,@enableDebugInfo		CHAR(1)			= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'Device_ListByGreenHouse' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@greenHouseGuid) AS '@greenHouseGuid'
            	, CONVERT(VARCHAR(MAX),@search) AS '@search'
				, CONVERT(VARCHAR(MAX),@pageSize) AS '@pageSize'
				, CONVERT(VARCHAR(MAX),@pageNumber) AS '@pageNumber'
				, CONVERT(VARCHAR(MAX),@orderby) AS '@orderby'
				, CONVERT(VARCHAR(MAX),@version) AS '@version'
            	, CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETDATE())
    END
    
    BEGIN TRY

		SELECT
		 @output = 1
		,@count = -1

		IF OBJECT_ID('tempdb..#temp_Device') IS NOT NULL DROP TABLE #temp_Device

		CREATE TABLE #temp_Device
		(	[guid]				UNIQUEIDENTIFIER
			,[companyGuid]		UNIQUEIDENTIFIER
			,[greenHouseGuid]	UNIQUEIDENTIFIER
			,[greenHouseName]	NVARCHAR(500)
			,[templateGuid]		UNIQUEIDENTIFIER
			,[parentDeviceGuid]	UNIQUEIDENTIFIER
			,[type]				TINYINT
			,[uniqueId]			NVARCHAR(500)
			,[name]				NVARCHAR(500)
			,[note]				NVARCHAR(1000)
			,[tag]				NVARCHAR(50)
			,[image]			NVARCHAR(200)
			,[isProvisioned]	BIT
			,[isActive]			BIT
			,[count]			BIGINT
			,[rowNum]			INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'name asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		( SELECT
			D.[guid]
			, D.[companyGuid]
			, D.[greenHouseGuid]
			, G.[name] AS [greenHouseName]
			, D.[templateGuid]
			, D.[parentDeviceGuid]
			, D.[type]
			, D.[uniqueId]
			, D.[name]
			, D.[note]
			, D.[tag]
			, D.[image]
			, D.[isProvisioned]
			, D.[isActive]
			, 0 AS [count]						
			FROM [dbo].[Device] D WITH (NOLOCK) 
			INNER JOIN [dbo].[GreenHouse] G WITH (NOLOCK) ON D.[greenHouseGuid] = G.[guid]
			 WHERE D.[greenHouseGuid]=@greenHouseGuid AND D.[parentDeviceGuid] IS NULL AND D.[isDeleted]=0 '
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (D.name LIKE ''%' + @search + '%''
			  OR D.[uniqueId] LIKE ''%' + @search + '%'' 
			  OR G.[name] LIKE ''%' + @search + '%'' 
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_Device
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @greenHouseGuid UNIQUEIDENTIFIER '
			, @orderby			= @orderby			
			, @greenHouseGuid	= @greenHouseGuid			
			
		SET @count = @@ROWCOUNT

		;WITH Devices
		AS 
		(	SELECT KD.[parentDeviceGuid],COUNT(KD.[guid]) totalDevices 
			FROM [dbo].[Device] KD
			WHERE KD.[greenHouseGuid] = @greenHouseGuid AND KD.[parentDeviceGuid] IS NOT NULL
				AND KD.[isActive] = 1 AND KD.[isDeleted] = 0 GROUP BY KD.[parentDeviceGuid]
		)
		UPDATE T
		SET [count] = ISNULL(KT.totalDevices,0)
		FROM #temp_Device T
		LEFT JOIN Devices KT (NOLOCK) ON T.[guid] = KT.[parentDeviceGuid]
		
		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					D.[guid]
					, D.[companyGuid]
					, D.[greenHouseGuid]
					, D.[greenHouseName]
					, D.[templateGuid]
					, D.[parentDeviceGuid]
					, D.[type]
					, D.[uniqueId]
					, D.[name]
					, D.[note]
					, D.[tag]
					, D.[image]
					, D.[isProvisioned]
					, D.[isActive]	
					, D.[count]					
				FROM #temp_Device D
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					D.[guid]
					, D.[companyGuid]
					, D.[greenHouseGuid]
					, D.[greenHouseName]
					, D.[templateGuid]
					, D.[parentDeviceGuid]
					, D.[type]
					, D.[uniqueId]
					, D.[name]
					, D.[note]
					, D.[tag]
					, D.[image]
					, D.[isProvisioned]
					, D.[isActive]		
					, D.[count]				
				FROM #temp_Device D
			END
	   
        SET @output = 1
		SET @fieldName = 'Success'
	END TRY	
	BEGIN CATCH	
		DECLARE @errorReturnMessage VARCHAR(MAX)

		SET @output = 0

		SELECT @errorReturnMessage = 
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   + 
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as VARCHAR), '')  + 
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as VARCHAR), '') + 
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as VARCHAR), '') + 
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as VARCHAR), '') + 
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as VARCHAR), '') + 
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as VARCHAR(max)), '')
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


