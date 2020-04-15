CREATE TABLE [dbo].[HardwareKit] (
    [guid]           UNIQUEIDENTIFIER NOT NULL,
    [kitTypeGuid]    UNIQUEIDENTIFIER NOT NULL,
    [kitCode]        NVARCHAR (50)    NOT NULL,
    [companyGuid]    UNIQUEIDENTIFIER NULL,
    [greenHouseGuid] UNIQUEIDENTIFIER NULL,
    [isActive]       BIT              CONSTRAINT [DF__HardwareK__isAct__7226EDCC] DEFAULT ((1)) NOT NULL,
    [isDeleted]      BIT              CONSTRAINT [DF__HardwareK__isDel__731B1205] DEFAULT ((0)) NOT NULL,
    [createdDate]    DATETIME         NULL,
    [createdBy]      UNIQUEIDENTIFIER NULL,
    [updatedDate]    DATETIME         NULL,
    [updatedBy]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Hardware__497F6CB477FFC5E7] PRIMARY KEY CLUSTERED ([guid] ASC)
);



