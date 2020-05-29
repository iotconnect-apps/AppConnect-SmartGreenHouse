/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)
	,@syncDate	DATETIME
EXEC [dbo].[Chart_SoilNutrition]	
	-- @companyguid	= '415C8959-5BFC-4203-A493-F89458AE7736'
	--@entityguid	= '415C8959-5BFC-4203-A493-F89458AE7736'
	@guid	= '46B0B123-D4BB-4DE2-ABB2-D15BC854B39D'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName, @syncDate syncDate

001	SGH-145 26-02-2020 [Nishit Khakhi]	Added Initial Version to represent Soil Nutrition 

*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_SoilNutrition]
(	@companyguid		UNIQUEIDENTIFIER	= NULL	
	,@entityguid		UNIQUEIDENTIFIER	= NULL	
	,@guid				UNIQUEIDENTIFIER	= NULL	
	,@invokinguser		UNIQUEIDENTIFIER	= NULL
	,@version			nvarchar(10)              
	,@output			SMALLINT			OUTPUT
	,@fieldname			nvarchar(255)		OUTPUT
	,@syncDate			DATETIME			OUTPUT
	,@culture			nvarchar(10)		= 'en-Us'	
	,@enabledebuginfo	CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Chart_SoilNutrition' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, CONVERT(nvarchar(MAX),@entityguid) AS '@entityguid' 
			, CONVERT(nvarchar(MAX),@guid) AS '@guid' 
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
    BEGIN TRY     

		IF OBJECT_ID ('tempdb..#result') IS NOT NULL BEGIN DROP TABLE #result END
		CREATE TABLE #result ([DATE] DATETIME, [guid] UNIQUEIDENTIFIER, [N] DECIMAL(18,2), [K] DECIMAL(18,2), [P] DECIMAL(18,2))

		DECLARE @startDate DATETIME, @endDate DATETIME
		SET @startDate = GETUTCDATE()-7
		SET @endDate = GETUTCDATE()
		IF @guid IS NOT NULL
		BEGIN
			INSERT INTO #result
			SELECT * FROM 
			(	SELECT [DATE] ,[deviceGuid] AS [guid],[attribute], AVG([avg]) 'Total'
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
				INNER JOIN [dbo].[Device] KD (NOLOCK) ON T.[deviceGuid] = KD.[guid] AND KD.[isDeleted] = 0
				INNER JOIN [dbo].[KitTypeAttribute] KA (NOLOCK) ON KA.[code] = T.[attribute] AND KA.[tag] = KD.[tag]
				WHERE ([date] BETWEEN @startDate AND @endDate) AND ( KD.[guid] = @guid OR KD.[parentDeviceGuid] = @guid ) AND KA.[code] IN ('N','K','P')
				GROUP BY [DATE],[deviceGuid],[attribute]
			) A 
			PIVOT(
						SUM (Total) 
						FOR [attribute] IN (
							[N], 
							[K],
							[P] 
							)
					) AS pivot_table;   
			
		END
		ELSE IF @entityguid IS NOT NULL
		BEGIN
			INSERT INTO #result
			SELECT * FROM 
			(	SELECT [DATE],KD.[greenHouseGuid] AS [guid],[attribute], AVG([avg]) 'Total'
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
				INNER JOIN [dbo].[Device] KD (NOLOCK) ON T.[deviceGuid] = KD.[guid] AND KD.[isDeleted] = 0
				INNER JOIN [dbo].[KitTypeAttribute] KA (NOLOCK) ON KA.[code] = T.[attribute] AND KA.[tag] = KD.[tag]
				WHERE ([date] BETWEEN @startDate AND @endDate) AND KD.[greenHouseGuid] = @entityguid AND KA.[code] IN ('N','K','P')
				GROUP BY [DATE],KD.[greenHouseGuid],[attribute]
			) A 
			PIVOT(
						SUM (Total) 
						FOR [attribute] IN (
							[N], 
							[K],
							[P] 
							)
					) AS pivot_table;  
		END
		ELSE IF @companyguid IS NOT NULL
		BEGIN
			INSERT INTO #result
			SELECT * FROM 
			(	SELECT [DATE], KD.[greenHouseGuid] AS [guid],[attribute], AVG([avg]) 'Total'
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
				INNER JOIN [dbo].[Device] KD (NOLOCK) ON T.[deviceGuid] = KD.[guid] AND KD.[isDeleted] = 0
				INNER JOIN [dbo].[KitTypeAttribute] KA (NOLOCK) ON KA.[code] = T.[attribute] AND KA.[tag] = KD.[tag]
				WHERE ([date] BETWEEN @startDate AND @endDate) AND KD.[companyGuid] = @companyguid AND KA.[code] IN ('N','K','P')
				GROUP BY[DATE], KD.[greenHouseGuid], [attribute]
			) A 
			PIVOT(
						SUM (Total) 
						FOR [attribute] IN (
							[N], 
							[K],
							[P] 
							)
					) AS pivot_table; 

		END

		SELECT CONCAT(DATENAME(day, DATEADD(DAY, (T.i - 6), GETUTCDATE())), ' - ', FORMAT( DATEADD(DAY, (T.i - 6), GETUTCDATE()), 'ddd')) AS 'Day'
		, RES.[guid], ISNULL(RES.[N],0) AS [N], ISNULL(RES.[K],0) AS [K], ISNULL(RES.[P],0) AS [P]
		FROM (VALUES (6), (5), (4), (3), (2), (1), (0)) AS T(i)
		LEFT OUTER JOIN ( SELECT * FROM [#result]) RES ON RES.[DATE] = DATEADD(DAY, (T.i - 6), CAST(GETUTCDATE() AS Date))
		ORDER BY DATEADD(DAY, (T.i - 6), GETUTCDATE())


        SET @output = 1
		SET @fieldname = 'Success'  
		SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
              
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