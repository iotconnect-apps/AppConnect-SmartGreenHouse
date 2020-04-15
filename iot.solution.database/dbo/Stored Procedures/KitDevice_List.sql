/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[KitDevice_List]
	 @guid	= 'B9EB822A-0ABE-4470-8EAB-38BF94321A84'
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SGH-97 23-01-2020 [Nishit Khakhi]	Added Initial Version to List Kit Device

*******************************************************************/
CREATE PROCEDURE [dbo].[KitDevice_List]
(	@guid				UNIQUEIDENTIFIER
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
            SELECT 'KitDevice_List' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@guid) AS '@guid'
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
			,[kitGuid]			UNIQUEIDENTIFIER
			,[companyGuid]		UNIQUEIDENTIFIER
			,[kitCode]			NVARCHAR(50)
			,[kitType]			NVARCHAR(200)
			,[templateGuid]		UNIQUEIDENTIFIER
			,[parentDeviceGuid]	UNIQUEIDENTIFIER
			,[parentUniqueId]	NVARCHAR(500)
			,[uniqueId]			NVARCHAR(500)
			,[name]				NVARCHAR(500)
			,[note]				NVARCHAR(1000)
			,[tag]				NVARCHAR(50)
			,[tagGuid]			UNIQUEIDENTIFIER
			,[isProvisioned]	BIT
			,[attributeName]	NVARCHAR(100)
			,[isActive]			BIT
			,[rowNum]			INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = '[name] ASC'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		( SELECT
			D.[guid]
			, D.[kitGuid]
			, HK.[companyGuid]
			, HK.[kitCode]
			, KT.[name] AS [kitTypeName]
			, KT.[guid] AS [templateGuid] 
			, D.[parentDeviceGuid]
			, PD.[uniqueId] AS [parentUniqueId]
			, D.[uniqueId]
			, D.[name]
			, D.[note]
			, KTA.[tag]
			, D.[tagGuid]
			, D.[isProvisioned]
			, KTA.[localName] AS [attributeName]
			, D.[isActive]
			FROM [dbo].[KitDevice] D WITH (NOLOCK) 
			INNER JOIN [dbo].[HardwareKit] HK WITH (NOLOCK) ON D.[kitGuid] = HK.[guid] AND HK.[isDeleted] = 0
			INNER JOIN [dbo].[KitType] KT WITH (NOLOCK) ON HK.[kitTypeGuid] = KT.[guid] AND KT.[isDeleted] = 0
			LEFT JOIN [dbo].[KitDevice] PD WITH (NOLOCK) ON D.[parentDeviceGuid] = PD.[guid] AND PD.[isDeleted] = 0
			LEFT JOIN [dbo].[KitTypeAttribute] KTA WITH (NOLOCK) ON KT.[guid] = KTA.[kittypeGuid] AND D.[tagGuid] = KTA.[guid]
			WHERE D.[kitGuid]=@guid AND D.[isDeleted]=0 '
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (D.[name] LIKE ''%' + @search + '%''
			  OR D.[uniqueId] LIKE ''%' + @search + '%'' 
			  OR H.[kitCode] LIKE ''%' + @search + '%'' 
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_Device
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @guid UNIQUEIDENTIFIER '
			, @orderby	= @orderby			
			, @guid	= @guid			
			
		SET @count = @@ROWCOUNT

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					[guid]				
					,[kitGuid]			
					,[companyGuid]		
					,[kitCode]			
					,[kitType]			
					,[templateGuid]		
					,[parentDeviceGuid]	
					,[parentUniqueId]
					,[uniqueId]			
					,[name]				
					,[note]				
					,[tag]	
					,[tagGuid]			
					,[isProvisioned]	
					,[attributeName]
					,[isActive]			
				FROM #temp_Device D
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					[guid]				
					,[kitGuid]			
					,[companyGuid]		
					,[kitCode]			
					,[kitType]
					,[templateGuid]		
					,[parentDeviceGuid]	
					,[parentUniqueId]
					,[uniqueId]			
					,[name]				
					,[note]				
					,[tag]	
					,[tagGuid]			
					,[isProvisioned]	
					,[attributeName]
					,[isActive]				
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


