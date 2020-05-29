
/*******************************************************************

EXEC [dbo].[TelemetrySummary_HourWise_Add]	
	
001	sgh-145 18-02-2020 [Nishit Khakhi]	Added Initial Version to Add Telemetry Summary Day wise for Day
*******************************************************************/

CREATE PROCEDURE [dbo].[TelemetrySummary_HourWise_Add]
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
	DECLARE @dt DATETIME = GETUTCDATE(), @lastExecDate DATETIME
	SELECT 
		TOP 1 @lastExecDate = CONVERT(DATETIME,[value]) 
	FROM [dbo].[Configuration] 
	WHERE [configKey] = 'telemetry-last-exectime' AND [isDeleted] = 0

	BEGIN TRAN		
		DELETE FROM [dbo].[TelemetrySummary_Hourwise] WHERE (CONVERT(DATE,[date]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt))  
		
		INSERT INTO [dbo].[TelemetrySummary_Hourwise]([guid]
		,[deviceGuid]
		,[date]
		,[attribute]
		,[min]
		,[max]
		,[avg]
		,[latest]
		,[sum]
		)
		
		SELECT NEWID(), [guid], DATEADD(HOUR,[HOUR],CAST([Date] AS smalldatetime)), [localName], 0, 0, ValueCount, 0, 0
		FROM (
		-- To Get AVG Value of 'co2','currentin','feedpressure','humidity'
		select D.[guid],KA.[code] AS [localName], CONVERT(DATE,A.createdDate) [Date], DATEPART(HOUR,A.createdDate) [Hour], AVG(CONVERT(DECimal(18, 2),attributeValue)) ValueCount
		FROM [IOTConnect].[AttributeValue] A (NOLOCK)
		INNER JOIN [dbo].[Device] D (NOLOCK) ON A.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0
		INNER JOIN [dbo].[KitTypeAttribute] KA ON A.[localName] = (CASE WHEN CHARINDEX('.', KA.[localName]) > 0 THEN SUBSTRING(KA.[localName], CHARINDEX('.', KA.[localName]) + 1 , DATALENGTH(KA.[localName])) ELSE KA.[localName] END) AND D.[tag] = KA.[tag]
		WHERE (CONVERT(DATE,A.[createdDate]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt)) AND KA.[code] IN ('INTST','NUTRIENT','CO2','FLOWRATE','feedpressure','temp','moisture','humidity','K','N','P')
		GROUP BY D.[guid],KA.[code], CONVERT(DATE,A.createdDate), DATEPART(HOUR,A.createdDate)
		) A
				
		INSERT INTO [dbo].[TelemetrySummary_Hourwise]([guid]
		,[deviceGuid]
		,[date]
		,[attribute]
		,[min]
		,[max]
		,[avg]
		,[latest]
		,[sum]
		)
		
		SELECT NEWID(), [guid], DATEADD(HOUR,[HOUR],CAST([Date] AS smalldatetime)), [localName], 0, 0, 0, 0, ValueCount
		FROM (
		-- To Get SUM of 'flowrate'
		select D.[guid],KA.[code] AS [localName], CONVERT(DATE,A.createdDate) [DATE], DATEPART(HOUR,A.createdDate) [Hour], SUM(CONVERT(DECimal(18, 2),attributeValue)) ValueCount
		FROM [IOTConnect].[AttributeValue] A (NOLOCK)
		INNER JOIN [dbo].[Device] D (NOLOCK) ON A.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0
		INNER JOIN [dbo].[KitTypeAttribute] KA ON A.[localName] = (CASE WHEN CHARINDEX('.', KA.[localName]) > 0 THEN SUBSTRING(KA.[localName], CHARINDEX('.', KA.[localName]) + 1 , DATALENGTH(KA.[localName])) ELSE KA.[localName] END) AND D.[tag] = KA.[tag]
		WHERE (CONVERT(DATE,A.[createdDate]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt)) AND KA.[code] IN ('pumpCurrentIn','lightCurrentIn')
		GROUP BY D.[guid],KA.[code], CONVERT(DATE,A.createdDate), DATEPART(HOUR,A.createdDate)
		) A
				
	COMMIT TRAN	

	END TRY	
	BEGIN CATCH
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