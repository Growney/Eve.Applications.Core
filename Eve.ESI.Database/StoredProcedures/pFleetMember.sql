CREATE PROCEDURE [dbo].[pFleetMember]
	@Result NCHAR(50),
	@CallID UNIQUEIDENTIFIER,
	@CharacterId INT = NULL,
	@JoinTime DATETIME = NULL,
	@Role INT = NULL,
	@RoleName VARCHAR(MAX) = NULL,
	@ShipTypeId INT = NULL,
	@SolarSystemId INT = NULL,
	@SquadId BIGINT = NULL,
	@StationId BIGINT = NULL,
	@TakesFleetWarp BIT = NULL,
	@WingId BIGINT = NULL
AS
BEGIN

IF @Result = 'Save'
BEGIN
	INSERT INTO FleetMember (CallID,CharacterId,JoinTime,Role,RoleName,ShipTypeId,SolarSystemId,SquadId,StationId,TakesFleetWarp,WingId)
	VALUES (@CallID,@CharacterId,@JoinTime,@Role,@RoleName,@ShipTypeId,@SolarSystemId,@SquadId,@StationId,@TakesFleetWarp,@WingId)

SELECT 0 AS [Value]
END
IF @Result = 'All'
BEGIN
	SELECT* FROM FleetMember
END
IF @Result = 'ForRequest'
BEGIN
	SELECT* FROM FleetMember WHERE CallID = @CallID
END

END
