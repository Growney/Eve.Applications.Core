

CREATE PROCEDURE [dbo].[pDataLoader]
	@Result NCHAR(50),
	@TableName VARCHAR(500) = NULL,
	@ColumnName VARCHAR(500) = NULL,
	@Value INT = NULL
AS
BEGIN
	IF @Result = 'ForPrimaryKey'
	BEGIN
		DECLARE @SQL VARCHAR(MAX)
		SET @SQL = 'SELECT* FROM [' + @TableName + '] WHERE [' +@ColumnName +'] = ' + CONVERT(VARCHAR(20),@Value)
		EXEC (@SQL)
	END
	IF @Result = 'TypeAttributes'
	BEGIN

		SELECT*
		FROM dgmTypeAttributes
		WHERE typeID = @Value

	END
END
