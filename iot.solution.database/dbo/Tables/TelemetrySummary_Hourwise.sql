CREATE TABLE [dbo].[TelemetrySummary_Hourwise] (
    [guid]       UNIQUEIDENTIFIER NOT NULL,
    [deviceGuid] UNIQUEIDENTIFIER NOT NULL,
    [date]       DATETIME         NOT NULL,
    [attribute]  NVARCHAR (1000)  NULL,
    [min]        INT              NULL,
    [max]        INT              NULL,
    [avg]        INT              NULL,
    [latest]     INT              NULL,
    [sum]        BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

