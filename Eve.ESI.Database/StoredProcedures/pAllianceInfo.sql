CREATE PROCEDURE [dbo].[pAllianceInfo]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
@CreatorCorporationId INT = NULL,
@CreatorId INT = NULL,
@DateFounded DATETIME = NULL,
@ExecutorCorporationId INT = NULL,
@FactionId INT = NULL,
@Name VARCHAR(MAX) = NULL,
@Ticker VARCHAR(MAX) = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO AllianceInfo (CallID,CreatorCorporationId,CreatorId,DateFounded,ExecutorCorporationId,FactionId,Name,Ticker)
	VALUES (@CallID,@CreatorCorporationId,@CreatorId,@DateFounded,@ExecutorCorporationId,@FactionId,@Name,@Ticker)

	SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM AllianceInfo
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM AllianceInfo WHERE CallID = @CallID
END

END
