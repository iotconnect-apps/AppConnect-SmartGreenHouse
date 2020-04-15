CREATE TABLE [dbo].[KitTypeAttribute] (
    [guid]                        UNIQUEIDENTIFIER NOT NULL,
    [parentTemplateAttributeGuid] UNIQUEIDENTIFIER NULL,
    [kittypeGuid]                 UNIQUEIDENTIFIER NOT NULL,
    [localName]                   NVARCHAR (100)   NOT NULL,
    [code]                        NVARCHAR (50)    NOT NULL,
    [tag]                         NVARCHAR (50)    NULL,
    [description]                 NVARCHAR (100)   NULL,
    CONSTRAINT [PK__KitTypeA__497F6CB4AFF441C6] PRIMARY KEY CLUSTERED ([guid] ASC)
);



