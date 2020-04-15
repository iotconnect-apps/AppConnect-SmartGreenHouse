
/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName	NVARCHAR(255)	
EXEC [dbo].[Crop_List]	
	 @companyguid		= '895019CF-1D3E-420C-828F-8971253E5784'
	,@greenHouseguid	= '5330D171-DA1E-4554-A868-87D222CD5D35'
	,@search			= NULL
	,@pagesize			= -1
	,@pageNumber		= -1	
	
	,@count				= @count		OUTPUT	
	,@invokinguser		= 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version			= 'v1'              
	,@output			= @output		OUTPUT
	,@fieldname			= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SGH-1 11-12-2019 [Nishit Khakhi]	Added Initial Version to List Role
*******************************************************************/
CREATE PROCEDURE [dbo].[Crop_List]
(	@companyguid			UNIQUEIDENTIFIER	= NULL
	,@greenHouseguid		UNIQUEIDENTIFIER	= NULL
	,@search				nvarchar(100)		= NULL	
	,@pagesize				INT
	,@pagenumber			INT
	,@count					INT					OUTPUT
	,@orderby				nvarchar(100)		= NULL
	,@invokinguser			UNIQUEIDENTIFIER
	,@version				nvarchar(10)              
	,@output				SMALLINT			OUTPUT
	,@fieldname				nvarchar(255)		OUTPUT
	,@culture				nvarchar(10)		= 'en-Us'	
	,@enabledebuginfo		CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Crop_List' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, CONVERT(nvarchar(MAX),@greenHouseguid) AS '@greenHouseguid' 
			, CONVERT(nvarchar(MAX),@search) AS '@search' 
			, CONVERT(nvarchar(MAX),@pagesize) AS '@pagesize' 
			, CONVERT(nvarchar(MAX),@pagenumber) AS '@pagenumber' 
			, CONVERT(nvarchar(MAX),@orderby) AS '@orderby' 
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETDATE())
    END                    
    
    BEGIN TRY            
		
		IF OBJECT_ID('tempdb..#temp_Crop') IS NOT NULL DROP TABLE #temp_Crop
		
		CREATE TABLE #temp_Crop
		(
			[guid]					UNIQUEIDENTIFIER
			,[companyguid]			UNIQUEIDENTIFIER
			,[greenHouseGuid]		UNIQUEIDENTIFIER
			,[greenHouseName]		NVARCHAR(500)
			,[cropName]				NVARCHAR(500)
			,[image]				NVARCHAR(200)		 
			,[isactive]				BIT
			,[createdby]			UNIQUEIDENTIFIER
			,[createdDate]			DATETIME
			,[updatedBy]			UNIQUEIDENTIFIER
			,[updatedDate]			DATETIME
			,[row_num]				INT
		)
			
		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'cropName asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
		SELECT 
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS row_num
		FROM
		(
			SELECT   
			c.[guid], 
			c.[companyGuid], 
			c.[greenHouseGuid],
			g.[name] AS [greenHouseName],
			c.[name] AS [cropName], 
			c.[image], 
			c.[isActive], 
			c.[createdBy], 
			c.[createdDate], 
			c.[updatedBy],
			c.[updatedDate]
			FROM [dbo].[Crop] AS c WITH (NOLOCK)
			INNER JOIN [dbo].[GreenHouse] AS g WITH (NOLOCK) ON c.[greenHouseGuid] = g.[guid] AND g.[isDeleted] = 0
			WHERE (c.[companyGuid] = @companyguid OR c.[greenHouseGuid] = @greenHouseGuid) AND c.[isdeleted] = 0 '								
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (c.[name] LIKE ''%' + @search + '%'') 
			OR (g.[name] LIKE ''%' + @search + '%'') '
			 END +
		') data '
		

		INSERT INTO #temp_Crop
		EXEC sp_executesql
			  @Sql
			, N'@companyguid UNIQUEIDENTIFIER, @greenhouseGuid UNIQUEIDENTIFIER'
			, @companyguid		= @companyguid
		    , @greenhouseGuid		= @greenhouseGuid
		    
		SET @count = @@ROWCOUNT
		
		--PRINT @Sql
		IF @pagesize = -1 
		BEGIN
			SELECT 
			[guid]					
			,[companyguid] AS [companyGuid]
			,[greenHouseGuid] AS [greenHouseGuid]
			,[greenHouseName] AS [greenHouseName]
			,[cropName]	AS [cropName]
			,[image] AS [image]
			,[isactive]	AS [isActive]	
			,[createdby] AS [createdBy]
			,[createdDate] AS [createdDate]
			,[updatedBy] AS [updatedBy]
			,[updatedDate] AS [updatedDate]
			FROM #temp_Crop
		END
		ELSE
		BEGIN
		SELECT 
			[guid]					
			,[companyguid] AS [companyGuid]
			,[greenHouseGuid] AS [greenHouseGuid]
			,[greenHouseName] AS [greenHouseName]
			,[cropName]	AS [cropName]
			,[image] AS [image]
			,[isactive]	AS [isActive]	
			,[createdby] AS [createdBy]
			,[createdDate] AS [createdDate]
			,[updatedBy] AS [updatedBy]
			,[updatedDate] AS [updatedDate]
		FROM #temp_Crop
		WHERE row_num BETWEEN ((@pagenumber - 1) * @pagesize) + 1 AND (@pagesize * @pagenumber)			
	   END

        SET @output = 1
		SET @fieldname = 'Success'   
              
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

