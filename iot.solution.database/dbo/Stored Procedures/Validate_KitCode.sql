
/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	
EXEC [dbo].[Validate_KitCode]	
	 @kitCode		= 'kitbyNikunj'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SGH-97 27-01-2020 [Nishit Khakhi]	Added Initial Version to give gateway Count of Hardware Kit
*******************************************************************/
CREATE PROCEDURE [dbo].[Validate_KitCode]
(	@kitCode				nvarchar(50)			
	,@invokinguser			UNIQUEIDENTIFIER	= NULL
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
            SELECT 'Validate_KitCode' AS '@procName' 
            , CONVERT(nvarchar(MAX),@kitCode) AS '@kitCode' 
			, CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETDATE())
    END                    
    
	IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[HardwareKit] AS h WITH (NOLOCK) WHERE [kitCode] = @kitCode AND [isDeleted] = 0 AND [companyGuid] IS NULL)
	BEGIN
	
		SET @output = 1
		SET @fieldname = 'InvalidKitCode'   
    	RETURN;
	END

	SET @output = 1
	SET @fieldname = 'Success'   
    
	BEGIN TRY            
		
		SELECT   
			COUNT(kt.[guid]) AS [gatewayCount]
		FROM [dbo].[HardwareKit] AS h WITH (NOLOCK)
		LEFT JOIN [dbo].[KitDevice] AS kt WITH (NOLOCK) ON kt.[kitGuid] = h.[guid] AND kt.[isDeleted] = 0 AND kt.[parentDeviceGuid] IS NULL
		WHERE h.[kitCode] = @kitCode AND h.[isdeleted] = 0 
		GROUP BY h.[kitCode]
              
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

