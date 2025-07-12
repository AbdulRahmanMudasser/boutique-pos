using MaqboolFashion.Data.Security;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace MaqboolFashion.Data.Database
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService()
        {
            // Try config first, then fallback
            _connectionString = ConfigurationManager.ConnectionStrings["MaqboolFashionDB"]?.ConnectionString
                            ?? @"Server=PROBOOK\SQL2022;Database=MaqboolFashionDB;Integrated Security=True;";
        }

        public bool CheckUserExists(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Email = @email",
                    connection);

                cmd.Parameters.AddWithValue("@email", email);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool RegisterUser(string firstName, string lastName, string email, string password)
        {
            var (hash, salt) = PasswordHasher.CreateHash(password);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Salt) " +
                    "VALUES (@firstName, @lastName, @email, @hash, @salt)",
                    connection);

                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@salt", salt);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}