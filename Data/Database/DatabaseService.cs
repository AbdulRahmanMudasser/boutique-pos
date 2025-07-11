using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MaqboolFashion.Data.Database
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            // Get connection string with null check
            var connectionString = ConfigurationManager.ConnectionStrings["MaqboolFashionDB"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Fallback connection string (replace with your actual values)
                connectionString = @"Data Source=PROBOOK\SQL2022;Initial Catalog=MaqboolFashionDB;Integrated Security=SSPI;Encrypt=False;TrustServerCertificate=True";

                // You might want to log this fallback scenario
                Console.WriteLine("Using fallback connection string");
            }

            _connectionString = connectionString;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new SqlCommand("SELECT 1", connection))
                    {
                        var result = (int)await cmd.ExecuteScalarAsync();
                        return result == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (in production, use proper logging)
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }
    }
}