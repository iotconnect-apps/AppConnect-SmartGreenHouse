/*******************************************************************
DECLARE @output INT = 0
		,@fieldName				nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[GreenHouse_Statistics_Get]
	 @guid				= '2D442AEA-E58B-4E8E-B09B-5602E1AA545A'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate
 
001	SGH-145 15-04-2020 [Nishit Khakhi]	Added Initial Version to Get GreenHouse Statistics
*******************************************************************/
CREATE PROCEDURE [dbo].[GreenHouse_Statistics_Get]
(	 @guid				UNIQUEIDENTIFIER	
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT
	,@syncDate			DATETIME		  OUTPUT
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
            SELECT 'GreenHouse_Statistics_Get' AS '@procName'
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
		(	SELECT [attribute],D.[greenHouseGuid],AVG([avg]) AS [value] 
			FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)
			INNER JOIN [dbo].[Device] D (NOLOCK) ON T.[deviceGuid] = D.[guid] AND D.[isDeleted] = 0
			WHERE D.[greenHouseGuid] = @guid AND T.[attribute] IN ('temp','humidity','moisture')
			GROUP BY [attribute],D.[greenHouseGuid]
		)
		, CTE_Device
		AS 
		(	SELECT [greenHouseGuid], COUNT([guid]) AS [totalDevice]
			FROM [dbo].[Device] (NOLOCK)
			WHERE [greenHouseGuid] = @guid AND [parentDeviceGuid] IS NOT NULL AND [isDeleted] = 0
			GROUP BY [greenHouseGuid]
		)
		,CTE_EnergyCount
		AS (	SELECT E.[greenHouseGuid]
						,T.[attribute]
						, SUM([sum]) [energyCount]
				FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
				INNER JOIN [dbo].[Device] E (NOLOCK) ON T.[deviceGuid] = E.[guid]
				WHERE E.[greenHouseGuid] = @guid AND [attribute] IN ('pumpCurrentIn','lightCurrentIn','FLOWRATE')
				GROUP BY E.[greenHouseGuid],T.[attribute]
		)
		SELECT [guid]
				, ISNULL(D.[totalDevice],0) AS [totalDevice]
				, CONVERT(DECIMAL(18,2),ISNULL(temp.[value],0)) AS [avgTemp]
				, CONVERT(DECIMAL(18,2),ISNULL(humidity.[value],0)) AS [avgHumidity]
				, CONVERT(DECIMAL(18,2),ISNULL(moisture.[value],0)) AS [avgMoisture]
				, ROUND(ISNULL(EC.[energyCount],0) + ISNULL(light.[energyCount],0),2) AS [totalEnergyCount]
				, ISNULL(ROUND(water.[energycount],2),0) AS [totalWaterUsage]
		FROM [dbo].[GreenHouse] C (NOLOCK) 
		LEFT JOIN CTE_Device D ON C.[guid] = D.[greenHouseGuid]
		LEFT JOIN CTE_attribute temp ON C.[guid] = temp.[greenHouseGuid] AND temp.[attribute] = 'temp' 
		LEFT JOIN CTE_attribute humidity ON C.[guid] = humidity.[greenHouseGuid] AND humidity.[attribute] = 'humidity' 
		LEFT JOIN CTE_attribute moisture ON C.[guid] = moisture.[greenHouseGuid] AND moisture.[attribute] = 'moisture' 
		LEFT JOIN CTE_EnergyCount EC ON C.[guid] = EC.[greenHouseGuid] AND EC.[attribute] = 'pumpCurrentIn'
		LEFT JOIN CTE_EnergyCount Water ON C.[guid] = EC.[greenHouseGuid] AND EC.[attribute] = 'FLOWRATE'
		LEFT JOIN CTE_EnergyCount light ON C.[guid] = light.[greenHouseGuid] AND light.[attribute] = 'lightCurrentIn'
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