﻿IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[configuration] WHERE [configKey] = 'db-version')
BEGIN
	INSERT [dbo].[Configuration] ([guid], [configKey], [value], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) 
	VALUES (N'cf45da4c-1b49-49f5-a5c3-8bc29c1999ea', N'db-version', N'0', 0, GETUTCDATE(), NULL, GETUTCDATE(), NULL)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[configuration] WHERE [configKey] = 'telemetry-last-exectime')
BEGIN
	INSERT [dbo].[Configuration] ([guid], [configKey], [value], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) 
	VALUES (N'465970b2-8bc3-435f-af97-8ca26f2bf383', N'telemetry-last-exectime', N'2020-01-01 12:08:02.380', 0, GETUTCDATE(), NULL, GETUTCDATE(), NULL)
END

DECLARE @DBVersion FLOAT  = 0
SELECT @DBVersion = CONVERT(FLOAT,[value]) FROM dbo.[configuration] WHERE [configKey] = 'db-version'

IF @DBVersion < 1 
BEGIN

	INSERT [dbo].[KitType] ([guid], [name], [code], [tag], [isActive], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) VALUES (N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'Default', N'GNTMP001', N'dell_5000', 1, 0, CAST(N'2020-02-12T13:20:44.217' AS DateTime), N'68aa338c-ebd7-4686-b350-de844c71db1f', NULL, NULL)

	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'69c4620a-570e-4bfd-9fe4-07a7e187f2bc', N'ec76f989-a9c3-4db4-b40d-c19c1472b42c', N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'nutrient.N', N'N', N'env', N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'e0e88203-a98c-403b-a499-465b1d72b9e9', N'ec76f989-a9c3-4db4-b40d-c19c1472b42c', N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'nutrient.P', N'P', N'env', N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'4e4a2c57-c76e-49c1-aaa0-50abb43f22ec', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'pumpCurrentIn', N'pumpCurrentIn', N'pump', N'Pump CurrentIn')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'021cff95-a357-43c7-8eb0-510eb8d60c22', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'moisture', N'moisture', N'env', N'Moisture')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'535fb058-f4f5-4483-bfd6-5531ab6c9824', N'ec76f989-a9c3-4db4-b40d-c19c1472b42c', N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'nutrient.K', N'K', N'env', N'')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'3e1bd2f5-da42-4fff-9a33-5af19d901aa3', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'temp', N'temp', N'env', N'Temperature')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'29f4d723-340d-4775-b89d-a1909375bcac', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'feedpressure', N'feedpressure', N'pump', N'Pump Feed pressure')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'bbe789aa-1662-4811-be2b-a70f2b1f3a4f', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'lightCurrentIn', N'lightCurrentIn', N'light', N'CurrentIn')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'5a89a147-b0f9-4221-b992-b3bb8a711857', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'flowrate', N'FLOWRATE', N'pump', N'Pump Water Flow Rate')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'86fd8530-2485-477a-8ec2-b7ea6d464d1c', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'humidity', N'HUMIDITY', N'env', N'Humidity')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'1d5499bd-c53c-46ec-ae8d-ba817cdf6f04', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'co2', N'CO2', N'env', N'Carbon dioxide')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'ec76f989-a9c3-4db4-b40d-c19c1472b42c', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'nutrient', N'NUTRIENT', N'env', N'Soil Nutrients')
	INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [kittypeGuid], [localName], [code], [tag], [description]) VALUES (N'c6045428-5b59-4b18-9585-d2488a08b5a1', NULL, N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'intst', N'INTST', N'light', N'Light Intensity')

	INSERT [dbo].[KitTypeCommand] ([guid], [kittypeGuid], [name], [command], [requiredParam], [requiredAck], [isOTACommand], [tag]) VALUES (N'8feda114-0b69-4b6d-9d85-61b7bf7e6fb3', N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'Light_ON_OFF', N'light                                                                                               ', 1, 0, 0, N'light                                                                                               ')
	INSERT [dbo].[KitTypeCommand] ([guid], [kittypeGuid], [name], [command], [requiredParam], [requiredAck], [isOTACommand], [tag]) VALUES (N'5403740a-49c7-4003-a262-6ca824115477', N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'Power_ON_OFF', N'power                                                                                               ', 1, 0, 0, N'power                                                                                               ')
	INSERT [dbo].[KitTypeCommand] ([guid], [kittypeGuid], [name], [command], [requiredParam], [requiredAck], [isOTACommand], [tag]) VALUES (N'dcc5a8f8-e6de-4456-abc7-712891f38ec7', N'938b3485-9970-4a8a-9c25-600ff8503dbb', N'Pump_ON_OFF', N'pump                                                                                                ', 1, 0, 0, N'pump                                                                                                ')

	INSERT INTO [dbo].[AdminUser] ([guid],[email],[companyGuid],[firstName],[lastName],[password],[isActive],[isDeleted],[createdDate]) VALUES (NEWID(),'admin@greenhouse.com','895019CF-1D3E-420C-828F-8971253E5784','GreenHouse','admin','Softweb#123',1,0,GETUTCDATE())

	UPDATE [dbo].[Configuration]
	SET [value]  = '1'
	WHERE [configKey] = 'db-version'

END
GO