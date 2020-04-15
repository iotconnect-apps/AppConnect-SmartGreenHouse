CREATE TABLE [dbo].[Crop] (
    [guid]           UNIQUEIDENTIFIER NOT NULL,
    [companyGuid]    UNIQUEIDENTIFIER NOT NULL,
    [greenHouseGuid] UNIQUEIDENTIFIER NOT NULL,
    [name]           NVARCHAR (500)   NOT NULL,
    [image]          NVARCHAR (200)   NULL,
    [isActive]       BIT              CONSTRAINT [DF__Crop__isactive__5CD6CB2B] DEFAULT ((1)) NOT NULL,
    [isDeleted]      BIT              CONSTRAINT [DF__Crop__isdeleted__5DCAEF64] DEFAULT ((0)) NOT NULL,
    [createdDate]    DATETIME         NOT NULL,
    [createdBy]      UNIQUEIDENTIFIER NOT NULL,
    [updatedDate]    DATETIME         NULL,
    [updatedBy]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Crop__497F6CB4D8233184] PRIMARY KEY CLUSTERED ([guid] ASC)
);

