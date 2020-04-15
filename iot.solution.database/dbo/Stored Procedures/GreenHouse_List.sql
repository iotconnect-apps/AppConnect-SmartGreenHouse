/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[GreenHouse_List]
	 @companyGuid	= '45A84216-8765-450B-BEE1-F607AF5163D2'
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	sgh-1	04-12-2019	[Nishit Khakhi]	Added Initial Version to List GreenHouse
002	SGH-145	11/02/2020	[Nishit Khakhi]	Updated to return count of Crop and Kit	
*******************************************************************/
CREATE PROCEDURE [dbo].[GreenHouse_List]
(
	@companyGuid		UNIQUEIDENTIFIER
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
	,@enableDebugInfo		CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'GreenHouse_List' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
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

		IF OBJECT_ID('tempdb..#temp_greenHouse') IS NOT NULL DROP TABLE #temp_greenHouse

		CREATE TABLE #temp_greenHouse
		(	[guid]						UNIQUEIDENTIFIER
			,[companyGuid]				UNIQUEIDENTIFIER
			,[name]						NVARCHAR(500)
			,[description]				NVARCHAR(500)
			,[address]					NVARCHAR(500)
			,[address2]					NVARCHAR(500)
			,[city]						NVARCHAR(50)
			,[zipCode]					NVARCHAR(10)
			,[stateGuid]				UNIQUEIDENTIFIER NULL
			,[countryGuid]				UNIQUEIDENTIFIER NULL
			,[image]					NVARCHAR(250)
			,[latitude]					NVARCHAR(50)
			,[longitude]				NVARCHAR(50)
			,[cropCount]				BIGINT
			,[kitCount]					BIGINT
			,[isActive]					BIT
			,[rowNum]					INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'name asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		(
			SELECT
			G.[guid]
			, G.[companyGuid]
			, G.[name]
			, G.[description]
			, G.[address] 
			, G.[address2] AS address2
			, G.[city]
			, G.[zipCode]
			, G.[stateGuid]
			, G.[countryGuid]
			, G.[image]
			, G.[latitude]
			, G.[longitude]
			, 0 AS [cropCount]
			, 0 AS [kitCount]
			, G.[isActive]			
			FROM [dbo].[GreenHouse] AS G WITH (NOLOCK) 
			 WHERE G.[companyGuid]=@companyGuid AND G.[isDeleted]=0 '
			  + ' and G.[Guid] not in (select greenHouseGuid from [dbo].[Company] where [Guid]=@companyGuid) '
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (G.name LIKE ''%' + @search + '%''
			  OR G.address LIKE ''%' + @search + '%''
			  OR G.address2 LIKE ''%' + @search + '%''
			  OR G.zipCode LIKE ''%' + @search + '%''
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_greenHouse
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER '
			, @orderby		= @orderby			
			, @companyGuid	= @companyGuid			
			
		SET @count = @@ROWCOUNT
		
		;with CTE_Crop
		AS
		(	SELECT C.[greenHouseGuid],COUNT(1) AS [totalCount] FROM [dbo].[Crop] C
			INNER JOIN #temp_greenHouse GH ON C.[greenHouseGuid] = GH.[guid]
			WHERE C.[companyGuid] = @companyGuid AND C.[isActive] = 1 AND C.[isDeleted] = 0 
			GROUP BY C.[greenHouseGuid]
		),
		CTE_HardwareKit
		AS
		(	SELECT GH.[guid], COUNT(H.[guid]) AS [totalCount] 
			FROM [dbo].[HardwareKit] H (NOLOCK)
			INNER JOIN #temp_greenHouse GH ON H.[greenHouseGuid] = GH.[guid]
			WHERE H.[companyGuid] = @companyGuid AND H.[isActive] = 1 AND H.[isDeleted] = 0
			GROUP BY GH.[guid],H.[companyGuid]
		)
		UPDATE t
		SET [cropCount] = ISNULL(CTC.[totalCount],0)
			,[kitCount] = ISNULL(CTH.[totalCount],0)
		FROM #temp_greenHouse t
		LEFT JOIN CTE_Crop CTC ON t.[guid] = CTC.[greenHouseGuid]
		LEFT JOIN CTE_HardwareKit CTH ON t.[guid] = CTH.[guid]

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					G.[guid]
					, G.[name]
					, G.[description]
					, G.[address] 
					, G.[address2] AS address2
					, G.[city]
					, G.[zipCode]
					, G.[stateGuid]
					, G.[countryGuid]
					, G.[image]
					, G.[latitude]
					, G.[longitude]
					, G.[cropCount]
					, G.[kitCount]
					, G.[isActive]					
				FROM #temp_greenHouse G
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					G.[guid]
					, G.[name]
					, G.[description]
					, G.[address] 
					, G.[address2] AS address2
					, G.[city]
					, G.[zipCode]
					, G.[stateGuid]
					, G.[countryGuid]
					, G.[image]
					, G.[latitude]
					, G.[longitude]
					, G.[cropCount]
					, G.[kitCount]
					, G.[isActive]		
				FROM #temp_greenHouse G
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


