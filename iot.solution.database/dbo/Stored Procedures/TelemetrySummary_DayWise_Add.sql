
/*******************************************************************
EXEC [dbo].[TelemetrySummary_DayWise_Add]	

001	sgh-145 18-02-2020 [Nishit Khakhi]	Added Initial Version to Add Telemetry Summary Day wise for Day
*******************************************************************/

CREATE PROCEDURE [dbo].[TelemetrySummary_DayWise_Add]
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
		DELETE FROM [dbo].[TelemetrySummary_Daywise] WHERE CONVERT(DATE,[Date]) BETWEEN @lastExecDate AND @dt
		
		INSERT INTO [dbo].[TelemetrySummary_Daywise]([guid]
		,[deviceGuid]
		,[date]
		,[attribute]
		,[min]
		,[max]
		,[avg]
		,[latest]
		,[sum]
		)
		
		SELECT NEWID(), [guid], [Date], [localName], 0, 0, ValueCount, 0, 0
		FROM (
		-- To Get AVG Value of 'co2','currentin','feedpressure','humidity'
		select D.[guid],A.localName, CONVERT(DATE,A.createdDate) [Date], AVG(CONVERT(BIGINT,attributeValue)) ValueCount
		FROM [IOTConnect].[AttributeValue] A
		INNER JOIN [dbo].[Device] D ON A.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0
		WHERE CONVERT(DATE,A.[createdDate])  BETWEEN @lastExecDate AND @dt AND A.localName IN ('co2','feedpressure','humidity','K','N','P')
		GROUP BY D.[guid],A.localName, CONVERT(DATE,A.createdDate)
		) A
				
		INSERT INTO [dbo].[TelemetrySummary_Daywise]([guid]
		,[deviceGuid]
		,[date]
		,[attribute]
		,[min]
		,[max]
		,[avg]
		,[latest]
		,[sum]
		)
		
		SELECT NEWID(), [guid], [Date], [localName], 0, 0, 0, 0, ValueCount
		FROM (
		-- To Get SUM of 'flowrate'
		select D.[guid],A.localName, CONVERT(DATE,A.createdDate) [DATE], SUM(CONVERT(BIGINT,attributeValue)) ValueCount
		FROM [IOTConnect].[AttributeValue] A
		INNER JOIN [dbo].[Device] D ON A.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0
		WHERE CONVERT(DATE,A.[createdDate])  BETWEEN @lastExecDate AND @dt AND A.localName IN ('flowrate','currentin')
		GROUP BY D.[guid],A.localName, CONVERT(DATE,A.createdDate)
		) A
				
		;WITH CTE
		AS (
		SELECT D.[guid],A.localName, CONVERT(DATE,A.createdDate) [DATE], CONVERT(BIGINT,attributeValue) [value], ROW_NUMBER() OVER (PARTITION BY A.[uniqueId],A.localName ORDER BY A.[createdDate] DESC) [no]
		FROM [IOTConnect].[AttributeValue] A
		INNER JOIN [dbo].[Device] D ON A.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0
		WHERE CONVERT(DATE,A.[createdDate])  BETWEEN @lastExecDate AND @dt AND A.localName IN ('intst','moisture','temp')
		)

		INSERT INTO [dbo].[TelemetrySummary_Daywise]([guid]
		,[deviceGuid]
		,[date]
		,[attribute]
		,[min]
		,[max]
		,[avg]
		,[latest]
		,[sum]
		)
		
		SELECT NEWID(), [guid], [Date], [localName], 0, 0, 0, [value], 0
		FROM (
		-- To Get Latest Values of 'intst','K','moisture','N','P','temp'
		SELECT [guid],[localName],[DATE],[value] from CTE WHERE [no] = 1
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