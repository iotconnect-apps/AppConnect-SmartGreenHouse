CREATE TABLE [dbo].[TelemetrySummary_Daywise] (
    [guid]       UNIQUEIDENTIFIER NOT NULL,
    [deviceGuid] UNIQUEIDENTIFIER NOT NULL,
    [date]       DATE             NOT NULL,
    [attribute]  NVARCHAR (1000)  NULL,
    [min]        INT              NULL,
    [max]        INT              NULL,
    [avg]        INT              NULL,
    [latest]     INT              NULL,
    [sum]        BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

