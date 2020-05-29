/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	
	,@syncDate	DATETIME
EXEC [dbo].[Chart_WaterConsumption]	
	@companyguid	= '97055520-B62D-4AD0-BB99-B3D1D7A347D6'
	--@entityguid	= '415C8959-5BFC-4203-A493-F89458AE7736'
	--@guid	= '46B0B123-D4BB-4DE2-ABB2-D15BC854B39D'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName, @syncDate syncDate

001	SGH-145 25-02-2020 [Nishit Khakhi]	Added Initial Version to represent water consumption

*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_WaterConsumption]
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
            SELECT 'Chart_WaterConsumption' AS '@procName' 
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
	 
		DECLARE @startDate DATETIME, @endDate DATETIME

		IF OBJECT_ID ('tempdb..#months') IS NOT NULL BEGIN DROP TABLE #months END
		CREATE TABLE [#months] ([date] DATETIME)

		INSERT INTO [#months]
		SELECT CONVERT(DATE, DATEADD(Month, (T.i - 11), GETUTCDATE())) AS [Date]
		FROM (VALUES (11), (10), (9), (8), (7), (6), (5), (4), (3), (2), (1), (0)) AS T(i) 

		SELECT @startDate = MIN(CONVERT(DATE, [Date] - DAY([Date]) + 1)), @endDate = MAX(CONVERT(DATE,EOMONTH([Date])))
		FROM [#months]

		IF OBJECT_ID ('tempdb..#result') IS NOT NULL BEGIN DROP TABLE #result END
		CREATE TABLE #result ([Year] INT, [Month] INT, [value] DECIMAL(18,2))

		IF @guid IS NOT NULL
		BEGIN
			INSERT INTO #result
			SELECT DATEPART(YY,[date]) [Year], DATEPART(MM,[date]) [Month], AVG([avg]) AS [value]
			FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
				INNER JOIN [dbo].[Device] KD (NOLOCK) ON T.[deviceGuid] = KD.[guid] AND KD.[isDeleted] = 0
				INNER JOIN [dbo].[KitTypeAttribute] KA (NOLOCK) ON KA.[tag] = KD.[tag] -- KA.[localName] = T.[attribute] AND
			WHERE ([date] BETWEEN @startDate AND @endDate) 
				AND ( KD.[guid] = @guid OR KD.[parentDeviceGuid] = @guid ) 
				AND KA.[localName] = 'flowrate' AND T.[attribute] = 'flowrate'
			GROUP BY DATEPART(YY,[date]), DATEPART(MM,[date])
		END
		ELSE IF @entityguid IS NOT NULL
		BEGIN
			INSERT INTO #result
			SELECT DATEPART(YY,[date]) [Year], DATEPART(MM,[date]) [Month], AVG([avg]) AS [value]
			FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
				INNER JOIN [dbo].[Device] KD (NOLOCK) ON T.[deviceGuid] = KD.[guid] AND KD.[isDeleted] = 0
				INNER JOIN [dbo].[KitTypeAttribute] KA (NOLOCK) ON KA.[tag] = KD.[tag] -- KA.[localName] = T.[attribute] AND
			WHERE ([date] BETWEEN @startDate AND @endDate) 
				AND KD.[greenHouseGuid] = @entityguid 
				AND KA.[localName] = 'flowrate' AND T.[attribute] = 'flowrate'
			GROUP BY DATEPART(YY,[date]), DATEPART(MM,[date])
		END
		ELSE IF @companyguid IS NOT NULL
		BEGIN
			INSERT INTO #result
			SELECT DATEPART(YY,[date]) [Year], DATEPART(MM,[date]) [Month], AVG([avg]) AS [value]
			FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
				INNER JOIN [dbo].[Device] KD (NOLOCK) ON T.[deviceGuid] = KD.[guid] AND KD.[isDeleted] = 0
				INNER JOIN [dbo].[KitTypeAttribute] KA (NOLOCK) ON KA.[tag] = KD.[tag] -- KA.[localName] = T.[attribute] AND
			WHERE ([date] BETWEEN @startDate AND @endDate) 
				AND KD.[companyGuid] = @companyguid 
				AND KA.[localName] = 'flowrate' AND T.[attribute] = 'flowrate'
			GROUP BY DATEPART(YY,[date]), DATEPART(MM,[date])
		END

		SELECT SUBSTRING(DATENAME(MONTH, M.[date]), 1, 3) + '-' + FORMAT(M.[date],'yy') AS [month]
		, ISNULL(R.[value],0) [value]
		FROM [#months] M
		LEFT OUTER JOIN #result R ON R.[Month] = DATEPART(MM, M.[date]) AND R.[Year] = DATEPART(YY, M.[date]) 
		ORDER BY  M.[date]

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