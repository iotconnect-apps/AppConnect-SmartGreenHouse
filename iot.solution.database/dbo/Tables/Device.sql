CREATE TABLE [dbo].[Device] (
    [guid]             UNIQUEIDENTIFIER NOT NULL,
    [companyGuid]      UNIQUEIDENTIFIER NOT NULL,
    [greenHouseGuid]   UNIQUEIDENTIFIER NOT NULL,
    [templateGuid]     UNIQUEIDENTIFIER NOT NULL,
    [parentDeviceGuid] UNIQUEIDENTIFIER NULL,
    [type]             TINYINT          NULL,
    [uniqueId]         NVARCHAR (500)   NOT NULL,
    [name]             NVARCHAR (500)   NOT NULL,
    [note]             NVARCHAR (1000)  NULL,
    [tag]              NVARCHAR (50)    NULL,
    [image]            NVARCHAR (200)   NULL,
    [isProvisioned]    BIT              CONSTRAINT [DF_Device_isProvision] DEFAULT ((0)) NOT NULL,
    [isActive]         BIT              CONSTRAINT [DF__Device__isactive__5165187F] DEFAULT ((1)) NOT NULL,
    [isDeleted]        BIT              CONSTRAINT [DF__Device__isdelete__52593CB8] DEFAULT ((0)) NOT NULL,
    [createdDate]      DATETIME         NULL,
    [createdBy]        UNIQUEIDENTIFIER NULL,
    [updatedDate]      DATETIME         NULL,
    [updatedBy]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Device__497F6CB4A9E84FFD] PRIMARY KEY CLUSTERED ([guid] ASC)
);

