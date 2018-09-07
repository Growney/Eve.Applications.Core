
DECLARE @SkillQueue TABLE
(
	[Index] INT IDENTITY(1,1),
	SkillPoints FLOAT,
	PerceptionLevel FLOAT,
	MemoryLevel FLOAT,
	WillpowerLevel FLOAT,
	IntelligenceLevel FLOAT,
	CharismaLevel FLOAT
)

INSERT INTO @SkillQueue(SkillPoints,PerceptionLevel,MemoryLevel,WillpowerLevel,IntelligenceLevel,CharismaLevel)
VALUES
(512000,1,2,0,0,0),
(256000,0,0,1,2,0)

DECLARE @Attributes TABLE
(
	Perception FLOAT,
	Memory FLOAT,
	Willpower FLOAT,
	Intelligence FLOAT,
	Charisma FLOAT
)



DECLARE @Base FLOAT = 17
DECLARE @Max FLOAT = 10
DECLARE @Spare FLOAT = 14

DECLARE @Perception FLOAT = 0
DECLARE @Memory FLOAT = 0
DECLARE @WillPower FLOAT = 0
DECLARE @Intelligence FLOAT = 0
DECLARE @Charisma FLOAT = 0

WHILE(@Perception <= @Max)
BEGIN

	SET @Memory = 0;
	WHILE(@Memory <= @Max AND @Memory <= @Spare - @Perception)
	BEGIN
		
		SET @WillPower = 0;
		WHILE(@WillPower <= @Max AND @WillPower <= @Spare - @Perception - @Memory)
		BEGIN
			SET @Intelligence = 0;
			WHILE(@Intelligence <= @Max AND @Intelligence <= @Spare - @Perception - @Memory - @WillPower)
			BEGIN
				SET @Charisma = 0
				WHILE(@Charisma <= @Max AND @Charisma <= @Spare - @Perception - @Memory - @WillPower - @Intelligence)
				BEGIN
					
					INSERT INTO @Attributes(Perception,Memory,Willpower,Intelligence,Charisma)
					VALUES(@Perception,@Memory,@WillPower,@Intelligence,@Charisma)

					SET @Charisma = @Charisma + 1
				END
				SET @Intelligence = @Intelligence + 1
			END

			SET @WillPower = @WillPower + 1
		END
		SET @Memory = @Memory + 1
	END 


	SET @Perception = @Perception + 1
END



SELECT*,SkillPoints / ((PrimaryValue + @Base) + ((SecondaryValue+ @Base)/2)) AS MinutesToTrain
INTO #IndivudalTimes
FROM
(
SELECT *, 
	CASE 
		WHEN PerceptionLevel = 1 THEN Perception 
		WHEN MemoryLevel = 1 THEN Memory
		WHEN WillpowerLevel = 1 THEN Willpower
		WHEN IntelligenceLevel = 1 THEN Intelligence
		WHEN CharismaLevel = 1 THEN Charisma
	END AS PrimaryValue,
	CASE 
		WHEN PerceptionLevel = 2 THEN Perception 
		WHEN MemoryLevel = 2 THEN Memory
		WHEN WillpowerLevel = 2 THEN Willpower
		WHEN IntelligenceLevel = 2 THEN Intelligence
		WHEN CharismaLevel = 2 THEN Charisma
	END AS SecondaryValue
	FROM @SkillQueue Q
	CROSS APPLY @Attributes A
) D

;WITH CTE_RecursiveQueue AS
(
	SELECT [Index],Perception,Memory,WillPower,Intelligence,Charisma,MinutesToTrain
	FROM #IndivudalTimes
	WHERE [Index] = 1
	UNION ALL
	SELECT N.[Index],N.Perception,N.Memory,N.WillPower,N.Intelligence,N.Charisma,N.MinutesToTrain + Q.MinutesToTrain
	FROM #IndivudalTimes N
	INNER JOIN CTE_RecursiveQueue Q ON N.[Index] -1 = Q.[Index] 
	AND N.Perception = Q.Perception
	AND N.Memory = Q.Memory
	AND N.Willpower = Q.Willpower
	AND N.Intelligence = Q.Intelligence
	AND N.Charisma = Q.Charisma
)
SELECT*
FROM CTE_RecursiveQueue
WHERE [Index] = (SELECT MAX([Index]) FROM @SkillQueue)
ORDER BY MinutesToTrain


DROP TABLE #IndivudalTimes
