﻿CREATE TABLE [dbo].[IDIntegerList]
(
	[CallID] UNIQUEIDENTIFIER NOT NULL,
	[Id] BIGINT NOT NULL,
	[Value] BIGINT NOT NULL
)
GO
CREATE CLUSTERED INDEX IX_IDIntegerList_CallID ON dbo.[IDIntegerList] (CallID)