CREATE TABLE [dbo].[KitDevice] (
    [guid]             UNIQUEIDENTIFIER NOT NULL,
    [kitGuid]          UNIQUEIDENTIFIER NOT NULL,
    [parentDeviceGuid] UNIQUEIDENTIFIER NULL,
    [uniqueId]         NVARCHAR (500)   NOT NULL,
    [name]             NVARCHAR (500)   NOT NULL,
    [note]             NVARCHAR (1000)  NOT NULL,
    [tagGuid]              UNIQUEIDENTIFIER    NULL,
    [isProvisioned]    BIT              DEFAULT ((1)) NOT NULL,
    [isActive]         BIT              DEFAULT ((1)) NOT NULL,
    [isDeleted]        BIT              DEFAULT ((0)) NOT NULL,
    [createdDate]      DATETIME         NULL,
    [createdBy]        UNIQUEIDENTIFIER NULL,
    [updatedDate]      DATETIME         NULL,
    [updatedBy]        UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

