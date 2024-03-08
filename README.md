# Procedures

## GetContributorsWithBalances Proc
```sql
CREATE PROC GetContributorsWithBalances
AS 
BEGIN
	SELECT c.*, d.TotalDeposits, cn.TotalContributions
	FROM Contributors c
	LEFT JOIN 
		(SELECT d.contributorId AS 'contId', SUM(d.Amount) AS 'TotalDeposits'
		FROM Deposits d 
		GROUP BY d.ContributorId) d
	ON c.Id = d.contId
	LEFT JOIN 
		(SELECT c.contributorId AS 'contId', (SUM(c.Amount) * -1) AS 'TotalContributions'
		FROM Contributions c 
		GROUP BY c.ContributorId) cn
	ON c.Id = cn.contId
END
```

## GetContributorWithBalances Proc
```sql
CREATE PROC GetContributorWithBalances
	@id int
AS 
BEGIN
	SELECT c.*, d.TotalDeposits, cn.TotalContributions
	FROM Contributors c
	LEFT JOIN 
		(SELECT d.contributorId AS 'contId', SUM(d.Amount) AS 'TotalDeposits'
		FROM Deposits d 
		GROUP BY d.ContributorId) d
	ON c.Id = d.contId
	LEFT JOIN 
		(SELECT c.contributorId AS 'contId', (SUM(c.Amount) * -1) AS 'TotalContributions'
		FROM Contributions c 
		GROUP BY c.ContributorId) cn
	ON c.Id = cn.contId
	WHERE c.Id = @id
END
```

 ## GetTotalBalance Proc
```sql
CREATE PROC GetTotalBalance
AS 
BEGIN

SELECT SUM( d.TotalDeposits + c.TotalContributions) AS 'TotalBalance' FROM
		(SELECT d.contributorId AS 'contId', SUM(d.Amount) AS 'TotalDeposits'
		FROM Deposits d 
		GROUP BY d.ContributorId) d
	JOIN 
		(SELECT c.contributorId AS 'contId', (SUM(c.Amount) * -1) AS 'TotalContributions'
		FROM Contributions c 
		GROUP BY c.ContributorId) c
	ON d.contId = c.contId	
END
```



