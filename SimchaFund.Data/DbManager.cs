using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class DbManager
    {
        private readonly string _connectionString;

        public DbManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.*, SUM(c.Amount) AS 'Total', 
                CASE 
	                WHEN SUM(c.Amount) IS NULL
	                THEN 0
	                ELSE COUNT(*)
                END AS 'Count' 
                FROM Simchas s 
                LEFT JOIN Contributions c ON c.SimchaId = s.Id
                Group By s.id, s.Name, s.Date
                ORDER BY s.Date asc";
            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<Simcha> simchas = new();
            while (reader.Read())
            {
                simchas.Add(new()
                {
                    Id = reader.GetValue<int>("Id"),
                    Name = reader.GetValue<string>("Name"),
                    Total = reader.GetValue<decimal>("Total"),
                    Date = reader.GetValue<DateTime>("Date"),
                    ContributerCount = reader.GetValue<int>("Count")
                });
            }
            return simchas;
        }

        public Simcha GetSimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simchas WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new()
            {
                Id = reader.GetValue<int>("Id"),
                Name = reader.GetValue<string>("Name"),
                Date = reader.GetValue<DateTime>("Date")
            };
        }

        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "GetContributorsWithBalances";
            connection.Open();
            var reader = cmd.ExecuteReader();

            List<Contributor> contributors = new();
            while (reader.Read())
            {
                contributors.Add(new()
                {
                    Id = reader.GetValue<int>("Id"),
                    FirstName = reader.GetValue<string>("FirstName"),
                    LastName = reader.GetValue<string>("LastName"),
                    CellNumber = reader.GetValue<string>("CellNumber"),
                    Email = reader.GetValue<string>("Email"),
                    AlwaysInclude = reader.GetValue<bool>("AlwaysInclude"),
                    Balance = reader.GetValue<decimal>("TotalDeposits") + reader.GetValue<decimal>("TotalContributions")
                });
            }
            return contributors;
        }

        public Contributor GetContributor(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "getcontributorwithbalances @id = @contId";
            cmd.Parameters.AddWithValue("@contId", id);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new()
            {
                Id = reader.GetValue<int>("Id"),
                FirstName = reader.GetValue<string>("FirstName"),
                LastName = reader.GetValue<string>("LastName"),
                CellNumber = reader.GetValue<string>("CellNumber"),
                Email = reader.GetValue<string>("Email"),
                Balance = reader.GetValue<decimal>("TotalDeposits") + reader.GetValue<decimal>("TotalContributions")
            };
        }

        public List<Contribution> GetContributions(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SimchaId, Amount, Date FROM Contributions               
                WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", contributorId);
            connection.Open();
            var reader = cmd.ExecuteReader();

            List<Contribution> contributions = new();
            while (reader.Read())
            {
                contributions.Add(new()
                {
                    SimchaId = reader.GetValue<int>("SimchaId"),
                    Amount = reader.GetValue<decimal>("Amount"),
                    Date = reader.GetValue<DateTime>("Date")
                });
            }
            return contributions;
        }

        public List<Deposit> GetDeposits(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Amount, Date FROM Deposits              
                WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", contributorId);
            connection.Open();
            var reader = cmd.ExecuteReader();

            List<Deposit> deposits = new();
            while (reader.Read())
            {
                deposits.Add(new()
                {
                    Amount = reader.GetValue<decimal>("Amount"),
                    Date = reader.GetValue<DateTime>("Date")
                });
            }
            return deposits;
        }

        public int GetTotalContributorCount()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public decimal GetTotalBalance()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "GetTotalBalance";
            connection.Open();
            return (decimal)cmd.ExecuteScalar();
        }

        public decimal GetAmountContributedForSimcha(int contributorId, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Amount FROM Contributions WHERE SimchaId = @simchaId AND ContributorId = @contId";
            cmd.Parameters.AddRangeWithValues(new()
            {
                {"@simchaId", simchaId },
                {"contId", contributorId }
            });
            connection.Open();
            return (decimal)cmd.ExecuteScalar();
        }

        public bool ContributedForSimcha(int contributorId, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ContributorId FROM Contributions WHERE SimchaId = @id";
            cmd.Parameters.AddWithValue("@id", simchaId);
            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if ((int)reader["ContributorId"] == contributorId)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateContributor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors SET
                FirstName = @fName, LastName = @lName, CellNumber = @cell, Email = @email, AlwaysInclude = @include
                WHERE Id = @id";
            cmd.Parameters.AddRangeWithValues(new()
            {
                {"@id", contributor.Id },
                {"@fName", contributor.FirstName },
                {"@lName", contributor.LastName },
                {"@cell", contributor.CellNumber },
                {"@email", contributor.Email == null? DBNull.Value : contributor.Email },
                {"@include", contributor.AlwaysInclude }
            });
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateContributionsForSimcha(int simchaId, List<Contributor> contributors)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            var sql = @"DELETE FROM Contributions WHERE SimchaId = @id 
                      INSERT INTO Contributions VALUES ";
            cmd.Parameters.AddWithValue("@id", simchaId);
            for (int i = 0; i < contributors.Count; i++)
            {
                sql += $"(@simchaId{i}, @contId{i}, @amount{i}, @date{i})";
                if (i < contributors.Count - 1)
                {
                    sql += ", ";
                }

                cmd.Parameters.AddRangeWithValues(new()
                {
                    {$"@simchaId{i}", simchaId},
                    {$"@contId{i}",contributors[i].Id  },
                    {$"@amount{i}", contributors[i].Amount },
                    {$"@date{i}", DateTime.Now }
                });
            }
            cmd.CommandText = sql;
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddSimcha(Simcha simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Simchas VALUES (@name, @date)";
            cmd.Parameters.AddRangeWithValues(new()
            {
                {"@name", simcha.Name },
                {"@date", simcha.Date }
            });
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddContributor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors VALUES (@fName, @lName, @cell, @email, @alwaysInclude)
                SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddRangeWithValues(new()
            {
                {"@fName", contributor.FirstName },
                {"@lName", contributor.LastName },
                {"@cell", contributor.CellNumber },
                {"@email", contributor.Email == null? DBNull.Value : contributor.Email },
                {"@alwaysInclude", contributor.AlwaysInclude }
            });
            connection.Open();
            contributor.Id = (int)(decimal)cmd.ExecuteScalar();
        }

        public void AddDeposit(Deposit deposit)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Deposits VALUES (@id, @amount, @date)";
            cmd.Parameters.AddRangeWithValues(new()
            {
                {"@id", deposit.ContributorId },
                {"@amount", deposit.Amount },
                {"@date", deposit.Date }
            });
            connection.Open();
            cmd.ExecuteNonQuery();

        }

        //this is a visual thing? maybe should be done in web part?
        //private string GetContributorCount(int amountThatContributed)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    using var cmd = connection.CreateCommand();
        //    cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
        //    connection.Open();
        //    var totalCount = (int)cmd.ExecuteScalar();
        //    return $"{amountThatContributed} / {totalCount}";
        //}
    }
}
