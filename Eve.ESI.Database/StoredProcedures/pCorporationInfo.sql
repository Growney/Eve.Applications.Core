CREATE PROCEDURE [dbo].[pCorporationInfo]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@AllianceId INT = NULL,
	@CeoId INT = NULL,
	@CreatorId INT = NULL,
	@DateFounded DATETIME = NULL,
	@Description VARCHAR(MAX) = NULL,
	@FactionId INT = NULL,
	@HomeStationId INT = NULL,
	@MemberCount INT = NULL,
	@Name VARCHAR(MAX) = NULL,
	@Shares BIGINT = NULL,
	@TaxRate FLOAT = NULL,
	@Ticker VARCHAR(MAX) = NULL,
	@Url VARCHAR(MAX) = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO CorporationInfo (CallID,AllianceId,CeoId,CreatorId,DateFounded,Description,FactionId,HomeStationId,MemberCount,Name,Shares,TaxRate,Ticker,Url)
	VALUES (@CallID,@AllianceId,@CeoId,@CreatorId,@DateFounded,@Description,@FactionId,@HomeStationId,@MemberCount,@Name,@Shares,@TaxRate,@Ticker,@Url)

	SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM CorporationInfo
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM CorporationInfo WHERE CallID = @CallID
END

END