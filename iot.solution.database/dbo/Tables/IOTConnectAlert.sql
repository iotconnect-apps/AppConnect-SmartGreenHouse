CREATE TABLE [dbo].[IOTConnectAlert] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
    [message]     NVARCHAR (500)   NULL,
    [companyGuid] UNIQUEIDENTIFIER NULL,
    [condition]   NVARCHAR (2000)  NULL,
    [deviceGuid]  UNIQUEIDENTIFIER NULL,
    [entityGuid]  UNIQUEIDENTIFIER NULL,
    [eventDate]   DATETIME         NULL,
    [uniqueId]    NVARCHAR (500)   NULL,
    [audience]    NVARCHAR (2000)  NULL,
    [eventId]     NVARCHAR (50)    NULL,
    [refGuid]     UNIQUEIDENTIFIER NULL,
    [severity]    NVARCHAR (200)   NULL,
    [ruleName]    NVARCHAR (200)   NULL,
    [data]        NVARCHAR (4000)  NULL
);

