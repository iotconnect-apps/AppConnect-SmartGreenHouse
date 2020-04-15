CREATE TABLE [dbo].[AdminRule] (
    [guid]              UNIQUEIDENTIFIER NOT NULL,
    [kittypeGuid]       UNIQUEIDENTIFIER NOT NULL,
    [ruleType]          SMALLINT         CONSTRAINT [DF__AdminRule__ruleT__636EBA21] DEFAULT ((1)) NOT NULL,
    [name]              NVARCHAR (100)   NULL,
    [description]       NVARCHAR (1000)  NULL,
    [attributeGuid]     NVARCHAR (1000)  NULL,
    [conditionText]     NVARCHAR (1000)  NULL,
    [conditionValue]    NVARCHAR (1000)  NULL,
    [severityLevelGuid] UNIQUEIDENTIFIER NULL,
    [notificationType]  BIGINT           NULL,
    [commandText]       NVARCHAR (500)   NULL,
    [commandValue]      NVARCHAR (100)   NULL,
    [isActive]          BIT              CONSTRAINT [DF__AdminRule__isAct__65570293] DEFAULT ((1)) NOT NULL,
    [isDeleted]         BIT              CONSTRAINT [DF__AdminRule__isDel__664B26CC] DEFAULT ((0)) NOT NULL,
    [createdDate]       DATETIME         NOT NULL,
    [createdBy]         UNIQUEIDENTIFIER NOT NULL,
    [updatedDate]       DATETIME         NULL,
    [updatedBy]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__AdminRul__497F6CB426B05957] PRIMARY KEY CLUSTERED ([guid] ASC)
);



