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