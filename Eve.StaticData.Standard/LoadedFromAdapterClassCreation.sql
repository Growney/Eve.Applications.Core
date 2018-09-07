DECLARE @TableName VARCHAR(MAX) = 'invTypes'
DECLARE @Namespace VARCHAR(MAX) = 'inv'

SELECT C.*,CASE WHEN CU.TABLE_CATALOG IS NOT NULL THEN 1 ELSE 0 END AS IsPrimaryKey,ROW_NUMBER() OVER (ORDER BY ORDINAL_POSITION) AS RowNumber
INTO #Columns
FROM INFORMATION_SCHEMA.COLUMNS C
LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS CT ON C.TABLE_NAME = CT.TABLE_NAME AND CT.CONSTRAINT_TYPE = 'PRIMARY KEY'
LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CU ON CT.CONSTRAINT_NAME = CU.CONSTRAINT_NAME AND C.COLUMN_NAME = CU.COLUMN_NAME
WHERE C.TABLE_NAME = @TableName

DECLARE @PrimaryKey VARCHAR(MAX)
DECLARE @Output VARCHAR(MAX)
DECLARE @MaxRow INT
DECLARE @CurRow INT
DECLARE @MinRow INT
DECLARE @NewLine VARCHAR(10) =CHAR(13) + CHAR(10)
DECLARE @DataType VARCHAR(50)
DECLARE @ColumnName VARCHAR(50)
DECLARE @BaseColumnName VARCHAR(50)
DECLARE @CSharpDataType VARCHAR(50)
DECLARE @Default VARCHAR(50)
SELECT @MaxRow = MAX(RowNumber) FROM #Columns
SELECT @MinRow = MIN(RowNumber) FROM #Columns

SELECT @PrimaryKey = COLUMN_NAME
FROM #Columns 
WHERE IsPrimaryKey = 1

SET @Output = 'using Gware.Standard.Storage;' + @NewLine
SET @Output += 'using Gware.Standard.Storage.Adapter;' + @NewLine
SET @Output += @NewLine
SET @Output += 'namespace Eve.Static.Standard.' + @Namespace
SET @Output += @NewLine
SET @Output += '{'
SET @Output += @NewLine
SET @Output += '[StaticData("'+ @TableName + '","' + @PrimaryKey + '")]'
SET @Output += @NewLine
SET @Output += 'public class '+ UPPER(LEFT(@TableName,1))+SUBSTRING(@TableName,2,LEN(@TableName)) + ' : LoadedFromAdapterBase'
SET @Output += @NewLine
SET @Output += '{'

SET @CurRow = @MinRow

WHILE (@CurRow < @MaxRow)
BEGIN
	
	SELECT @ColumnName = UPPER(LEFT(COLUMN_NAME,1))+SUBSTRING(COLUMN_NAME,2,LEN(COLUMN_NAME)),
	@DataType = DATA_TYPE
	FROM #Columns
	WHERE RowNumber = @CurRow

	SET @CSharpDataType = 
	(CASE 
		WHEN @DataType = 'nvarchar' THEN 'string' 
		WHEN @DataType = 'varchar' THEN 'string' 
		WHEN @DataType = 'bit' THEN 'bool'
		WHEN @DataType = 'bigint' THEN 'long'
		ELSE @DataType
	END)
	SET @Output += @NewLine
	SET @Output += 'public ' + @CSharpDataType + ' ' + @ColumnName +  ' { get; private set; }'

	SET @CurRow = @CurRow +1
END

SET @Output += @NewLine
SET @Output += @NewLine

SET @Output += 'protected override void LoadFrom(IDataAdapter adapter)'
SET @Output += @NewLine
SET @Output += '{'

SET @CurRow = @MinRow

WHILE (@CurRow < @MaxRow)
BEGIN
	SELECT @BaseColumnName = COLUMN_NAME, @ColumnName = UPPER(LEFT(COLUMN_NAME,1))+SUBSTRING(COLUMN_NAME,2,LEN(COLUMN_NAME)),
	@DataType = DATA_TYPE
	FROM #Columns
	WHERE RowNumber = @CurRow

	SET @Default = 
	(CASE 
		WHEN @DataType = 'nvarchar' THEN 'string.Empty' 
		WHEN @DataType = 'varchar' THEN 'string.Empty' 
		WHEN @DataType = 'bit' THEN 'false'
		WHEN @DataType = 'bigint' THEN '0L'
		WHEN @DataType = 'float' THEN '0.0f'
		WHEN @DataType = 'int' THEN '0'
		WHEN @DataType = 'decimal' THEN '0.0m'
		ELSE @DataType
	END)
	SET @Output += @NewLine
	SET @Output += @ColumnName + ' = adapter.GetValue("' + @BaseColumnName+'",' + @Default + ');'

	SET @CurRow = @CurRow +1
END

SET @Output += @NewLine
SET @Output += '}'

SET @Output += @NewLine
SET @Output += '}' -- Class
SET @Output += @NewLine
SET @Output += '}' -- Namespace

PRINT (@Output)

DROP TABLE #Columns