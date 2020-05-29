/*******************************************************************
DECLARE @output INT = 0
		,@fieldName				nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[Device_Statistics_Get]
	 @guid				= 'C79E52F2-6D68-419A-8E26-CD2752A834A6'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
	,@syncDate			= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName , @syncDate syncDate    
 
001	SGH-145 15-04-2020 [Nishit Khakhi]	Added Initial Version to Get GreenHouse Statistics
*******************************************************************/
CREATE PROCEDURE [dbo].[Device_Statistics_Get]
(	 @guid				UNIQUEIDENTIFIER	
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT	
	,@syncDate			DATETIME			OUTPUT
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
            SELECT 'Device_Statistics_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'			
	        , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    Set @output = 1
    SET @fieldName = 'Success'

    BEGIN TRY
		SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')

		;WITH CTE_attribute
		AS 
		(	SELECT [attribute],D.[parentDeviceGuid],AVG([avg]) AS [value] 
			FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
			INNER JOIN [dbo].[Device] D (NOLOCK) ON T.[deviceGuid] = D.[guid] AND D.[isDeleted] = 0
			WHERE [parentDeviceGuid] = @guid AND T.[attribute] IN ('temp','humidity','moisture')
			GROUP BY [attribute],D.[parentDeviceGuid]
		)
		, CTE_Device
		AS 
		(	SELECT [parentDeviceGuid], ISNULL(COUNT([guid]),0) AS [totalDevice]
			FROM [dbo].[Device] (NOLOCK)
			WHERE [parentDeviceGuid] = @guid AND [isDeleted] = 0
			GROUP BY [parentDeviceGuid]
		)
		,CTE_EnergyCount
		AS (	SELECT E.[parentDeviceGuid]
						,T.[attribute]
						, SUM([sum]) [energyCount]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Device] E (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE [parentDeviceGuid] = @guid AND [attribute] IN ('pumpCurrentIn','lightCurrentIn','FLOWRATE')
				GROUP BY E.[parentDeviceGuid],T.[attribute]
		)
		SELECT c.[guid]
				, ISNULL(D.[totalDevice],0) AS [totalDevice]
				, CONVERT(DECIMAL(18,2),ISNULL(temp.[value],0)) AS [avgTemp]
				, CONVERT(DECIMAL(18,2),ISNULL(humidity.[value],0)) AS [avgHumidity]
				, CONVERT(DECIMAL(18,2),ISNULL(moisture.[value],0)) AS [avgMoisture]
				, ROUND(ISNULL(EC.[energyCount],0) + ISNULL(light.[energyCount],0),2) AS [totalEnergyCount]
				, ISNULL(ROUND(water.[energycount],2),0) AS [totalWaterUsage]
		FROM [dbo].[Device] C (NOLOCK) 
		LEFT JOIN CTE_Device D ON C.[guid] = D.[parentDeviceGuid]
		LEFT JOIN CTE_attribute temp ON C.[guid] = temp.[parentDeviceGuid] AND temp.[attribute] = 'temp' 
		LEFT JOIN CTE_attribute humidity ON C.[guid] = humidity.[parentDeviceGuid] AND humidity.[attribute] = 'humidity' 
		LEFT JOIN CTE_attribute moisture ON C.[guid] = moisture.[parentDeviceGuid] AND moisture.[attribute] = 'moisture' 
		LEFT JOIN CTE_EnergyCount EC ON C.[guid] = EC.[parentDeviceGuid] AND EC.[attribute] = 'pumpCurrentIn'
		LEFT JOIN CTE_EnergyCount Water ON C.[guid] = Water.[parentDeviceGuid] AND Water.[attribute] = 'FLOWRATE'
		LEFT JOIN CTE_EnergyCount light ON C.[guid] = light.[parentDeviceGuid] AND light.[attribute] = 'lightCurrentIn'
		WHERE C.[guid]=@guid AND C.[isDeleted]=0
		
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