using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace MaqboolFashion.Data.Database
{
    public class LaborService
    {
        private readonly string _connectionString;

        public LaborService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MaqboolFashionDB"]?.ConnectionString
                            ?? @"Server=PROBOOK\SQL2022;Database=MaqboolFashionDB;Integrated Security=True;";
        }

        public class Labor
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Area { get; set; }
            public string City { get; set; }
            public string PhoneNumber { get; set; }
            public string CNIC { get; set; }
            public decimal Cost { get; set; }
            public DateTime JoiningDate { get; set; }
            public decimal CurrentAdvance { get; set; }
            public string ProfileImagePath { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public bool AddLabor(string name, string area, string city, string phoneNumber, string cnic, decimal cost, DateTime joiningDate, string profileImagePath = "")
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        @"INSERT INTO Labor (Name, Area, City, PhoneNumber, CNIC, Cost, JoiningDate, CurrentAdvance, ProfileImagePath, IsActive, CreatedDate) 
                          VALUES (@name, @area, @city, @phoneNumber, @cnic, @cost, @joiningDate, @currentAdvance, @profileImagePath, @isActive, @createdDate)",
                        connection);

                    cmd.Parameters.AddWithValue("@name", name ?? "");
                    cmd.Parameters.AddWithValue("@area", area ?? "");
                    cmd.Parameters.AddWithValue("@city", city ?? "");
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber ?? "");
                    cmd.Parameters.AddWithValue("@cnic", cnic ?? "");
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@joiningDate", joiningDate);
                    cmd.Parameters.AddWithValue("@currentAdvance", 0); // Initially 0
                    cmd.Parameters.AddWithValue("@profileImagePath", profileImagePath ?? "");
                    cmd.Parameters.AddWithValue("@isActive", true);
                    cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding labor: {ex.Message}");
            }
        }

        public bool UpdateLabor(int id, string name, string area, string city, string phoneNumber, string cnic, decimal cost, DateTime joiningDate, string profileImagePath = "")
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        @"UPDATE Labor 
                          SET Name = @name, Area = @area, City = @city, PhoneNumber = @phoneNumber, 
                              CNIC = @cnic, Cost = @cost, JoiningDate = @joiningDate, ProfileImagePath = @profileImagePath 
                          WHERE Id = @id",
                        connection);

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name ?? "");
                    cmd.Parameters.AddWithValue("@area", area ?? "");
                    cmd.Parameters.AddWithValue("@city", city ?? "");
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber ?? "");
                    cmd.Parameters.AddWithValue("@cnic", cnic ?? "");
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@joiningDate", joiningDate);
                    cmd.Parameters.AddWithValue("@profileImagePath", profileImagePath ?? "");

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating labor: {ex.Message}");
            }
        }

        public bool DeleteLabor(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        "UPDATE Labor SET IsActive = 0 WHERE Id = @id",
                        connection);

                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting labor: {ex.Message}");
            }
        }

        public List<Labor> GetLabor(int pageNumber = 1, int pageSize = 10, string searchTerm = "")
        {
            if (pageNumber < 1) throw new ArgumentException("Page number must be at least 1.", nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentException("Page size must be at least 1.", nameof(pageSize));

            var laborList = new List<Labor>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var whereClause = "WHERE IsActive = 1";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        whereClause += " AND (Name LIKE @searchTerm OR Area LIKE @searchTerm OR City LIKE @searchTerm OR PhoneNumber LIKE @searchTerm OR CNIC LIKE @searchTerm)";
                    }

                    var query = $@"
                        SELECT Id, Name, Area, City, PhoneNumber, CNIC, Cost, JoiningDate, CurrentAdvance, ProfileImagePath, IsActive, CreatedDate
                        FROM Labor 
                        {whereClause}
                        ORDER BY CreatedDate DESC
                        OFFSET @offset ROWS
                        FETCH NEXT @pageSize ROWS ONLY";

                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            laborList.Add(new Labor
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "",
                                Area = reader["Area"] != DBNull.Value ? reader["Area"].ToString() : "",
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : "",
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "",
                                CNIC = reader["CNIC"] != DBNull.Value ? reader["CNIC"].ToString() : "",
                                Cost = reader["Cost"] != DBNull.Value ? Convert.ToDecimal(reader["Cost"]) : 0,
                                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                                CurrentAdvance = reader["CurrentAdvance"] != DBNull.Value ? Convert.ToDecimal(reader["CurrentAdvance"]) : 0,
                                ProfileImagePath = reader["ProfileImagePath"] != DBNull.Value ? reader["ProfileImagePath"].ToString() : "",
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving labor: {ex.Message}");
            }

            return laborList;
        }

        public int GetTotalLaborCount(string searchTerm = "")
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var whereClause = "WHERE IsActive = 1";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        whereClause += " AND (Name LIKE @searchTerm OR Area LIKE @searchTerm OR City LIKE @searchTerm OR PhoneNumber LIKE @searchTerm OR CNIC LIKE @searchTerm)";
                    }

                    var query = $"SELECT COUNT(*) FROM Labor {whereClause}";
                    var cmd = new SqlCommand(query, connection);

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                    }

                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting labor count: {ex.Message}");
            }
        }

        public Labor GetLaborById(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        "SELECT Id, Name, Area, City, PhoneNumber, CNIC, Cost, JoiningDate, CurrentAdvance, ProfileImagePath, IsActive, CreatedDate FROM Labor WHERE Id = @id",
                        connection);

                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Labor
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "",
                                Area = reader["Area"] != DBNull.Value ? reader["Area"].ToString() : "",
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : "",
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "",
                                CNIC = reader["CNIC"] != DBNull.Value ? reader["CNIC"].ToString() : "",
                                Cost = reader["Cost"] != DBNull.Value ? Convert.ToDecimal(reader["Cost"]) : 0,
                                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                                CurrentAdvance = reader["CurrentAdvance"] != DBNull.Value ? Convert.ToDecimal(reader["CurrentAdvance"]) : 0,
                                ProfileImagePath = reader["ProfileImagePath"] != DBNull.Value ? reader["ProfileImagePath"].ToString() : "",
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving labor: {ex.Message}");
            }

            return null;
        }

        public bool CNICExists(string cnic, int? excludeId = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT COUNT(*) FROM Labor WHERE CNIC = @cnic AND IsActive = 1";

                    if (excludeId.HasValue)
                    {
                        query += " AND Id != @excludeId";
                    }

                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@cnic", cnic ?? "");

                    if (excludeId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@excludeId", excludeId.Value);
                    }

                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking CNIC: {ex.Message}");
            }
        }

        public bool PhoneNumberExists(string phoneNumber, int? excludeId = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT COUNT(*) FROM Labor WHERE PhoneNumber = @phoneNumber AND IsActive = 1";

                    if (excludeId.HasValue)
                    {
                        query += " AND Id != @excludeId";
                    }

                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber ?? "");

                    if (excludeId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@excludeId", excludeId.Value);
                    }

                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking phone number: {ex.Message}");
            }
        }

        public bool UpdateCurrentAdvance(int laborId, decimal newAdvanceAmount)
        {
            if (laborId <= 0) throw new ArgumentException("Labor ID must be a positive integer.", nameof(laborId));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        "UPDATE Labor SET CurrentAdvance = @currentAdvance WHERE Id = @laborId",
                        connection);

                    cmd.Parameters.AddWithValue("@laborId", laborId);
                    cmd.Parameters.AddWithValue("@currentAdvance", newAdvanceAmount);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating current advance: {ex.Message}");
            }
        }

        // Validation methods
        public static bool IsValidCNIC(string cnic)
        {
            if (string.IsNullOrWhiteSpace(cnic))
                return false;

            // Remove any dashes or spaces
            cnic = cnic.Replace("-", "").Replace(" ", "");

            // Check if it's exactly 13 digits
            return cnic.Length == 13 && Regex.IsMatch(cnic, @"^\d{13}$");
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Remove any special characters
            phoneNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

            // Check if it's 10 or 11 digits (Pakistani phone numbers)
            return phoneNumber.Length >= 10 && phoneNumber.Length <= 11 && Regex.IsMatch(phoneNumber, @"^\d+$");
        }

        public static string FormatCNIC(string cnic)
        {
            if (string.IsNullOrWhiteSpace(cnic))
                return "";

            // Remove any existing formatting
            cnic = cnic.Replace("-", "").Replace(" ", "");

            // Format as XXXXX-XXXXXXX-X
            if (cnic.Length == 13)
            {
                return $"{cnic.Substring(0, 5)}-{cnic.Substring(5, 7)}-{cnic.Substring(12, 1)}";
            }

            return cnic;
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return "";

            // Remove any special characters
            phoneNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

            // Format Pakistani phone numbers
            if (phoneNumber.Length == 11 && phoneNumber.StartsWith("03"))
            {
                return $"{phoneNumber.Substring(0, 4)}-{phoneNumber.Substring(4, 7)}";
            }
            else if (phoneNumber.Length == 10)
            {
                return $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 7)}";
            }

            return phoneNumber;
        }
    }
}