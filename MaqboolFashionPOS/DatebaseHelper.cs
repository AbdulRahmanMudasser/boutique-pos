using System;
using System.Data.SqlClient;

namespace MaqboolFashionPOS
{
    public class DatabaseHelper
    {
        private readonly string connectionString = @"Data Source=PROBOOK\SQL2022;Initial Catalog=MaqboolFashionPOS;Integrated Security=SSPI;Encrypt=False;TrustServerCertificate=True";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}