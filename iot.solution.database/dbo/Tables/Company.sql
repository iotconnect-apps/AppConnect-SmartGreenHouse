CREATE TABLE [dbo].[Company] (
    [guid]           UNIQUEIDENTIFIER NOT NULL,
    [greenHouseGuid] UNIQUEIDENTIFIER NOT NULL,
    [parentGuid]     UNIQUEIDENTIFIER NULL,
    [adminUserGuid]  UNIQUEIDENTIFIER NULL,
    [name]           NVARCHAR (100)   NOT NULL,
    [cpId]           NVARCHAR (200)   NOT NULL,
    [contactNo]      NVARCHAR (25)    NULL,
    [address]        NVARCHAR (500)   NULL,
    [countryGuid]    UNIQUEIDENTIFIER NULL,
    [timezoneGuid]   UNIQUEIDENTIFIER NULL,
    [image]          NVARCHAR (250)   NULL,
    [isActive]       BIT              CONSTRAINT [DF__Company__isactiv__37A5467C] DEFAULT ((1)) NOT NULL,
    [isDeleted]      BIT              CONSTRAINT [DF__Company__isdelet__38996AB5] DEFAULT ((0)) NOT NULL,
    [createdDate]    DATETIME         NOT NULL,
    [createdBy]      UNIQUEIDENTIFIER NOT NULL,
    [updatedDate]    DATETIME         NULL,
    [updatedBy]      UNIQUEIDENTIFIER NULL,
    [stateGuid]      UNIQUEIDENTIFIER NULL,
    [city]           NVARCHAR (50)    NULL,
    [postalCode]     NVARCHAR (30)    NULL,
    CONSTRAINT [PK__Company__497F6CB411118A97] PRIMARY KEY CLUSTERED ([guid] ASC)
);

