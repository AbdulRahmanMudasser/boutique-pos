using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

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
            public string Caste { get; set; }
            public decimal Cost { get; set; }
            public DateTime JoiningDate { get; set; }
            public decimal CurrentAdvance { get; set; }
            public string ProfileImagePath { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsActive { get; set; }
        }

        public bool AddLabor(string name, string area, string city, string phoneNumber, string cnic,
                           string caste, decimal cost, DateTime joiningDate, string profileImagePath = null, byte[] imageData = null)
        {
            try
            {
                // If imageData is provided, save the image and get the file path
                string finalImagePath = profileImagePath;
                if (imageData != null && imageData.Length > 0)
                {
                    finalImagePath = SaveProfileImage(name, imageData);
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO Labor (Name, Area, City, PhoneNumber, CNIC, Caste, Cost, JoiningDate, 
                          CurrentAdvance, ProfileImagePath, CreatedDate, IsActive) 
                          VALUES (@name, @area, @city, @phoneNumber, @cnic, @caste, @cost, @joiningDate, 
                          @currentAdvance, @profileImagePath, @createdDate, @isActive)",
                        connection);

                    cmd.Parameters.AddWithValue("@name", name ?? "");
                    cmd.Parameters.AddWithValue("@area", area ?? "");
                    cmd.Parameters.AddWithValue("@city", city ?? "");
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber ?? "");
                    cmd.Parameters.AddWithValue("@cnic", cnic ?? "");
                    cmd.Parameters.AddWithValue("@caste", caste ?? "");
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@joiningDate", joiningDate);
                    cmd.Parameters.AddWithValue("@currentAdvance", 0);
                    cmd.Parameters.AddWithValue("@profileImagePath", finalImagePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@isActive", true);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding labor: {ex.Message}");
            }
        }

        public bool UpdateLabor(int id, string name, string area, string city, string phoneNumber,
                              string cnic, string caste, decimal cost, DateTime joiningDate, string profileImagePath = null, byte[] imageData = null)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            try
            {
                // If imageData is provided, save the image and get the file path
                string finalImagePath = profileImagePath;
                if (imageData != null && imageData.Length > 0)
                {
                    finalImagePath = SaveProfileImage(name, imageData);
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(
                        @"UPDATE Labor 
                          SET Name = @name, Area = @area, City = @city, PhoneNumber = @phoneNumber, 
                              CNIC = @cnic, Caste = @caste, Cost = @cost, JoiningDate = @joiningDate,
                              ProfileImagePath = @profileImagePath
                          WHERE Id = @id",
                        connection);

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name ?? "");
                    cmd.Parameters.AddWithValue("@area", area ?? "");
                    cmd.Parameters.AddWithValue("@city", city ?? "");
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber ?? "");
                    cmd.Parameters.AddWithValue("@cnic", cnic ?? "");
                    cmd.Parameters.AddWithValue("@caste", caste ?? "");
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@joiningDate", joiningDate);
                    cmd.Parameters.AddWithValue("@profileImagePath", finalImagePath ?? (object)DBNull.Value);

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

        public List<Labor> GetLabors(int pageNumber = 1, int pageSize = 10, string searchTerm = "")
        {
            if (pageNumber < 1) throw new ArgumentException("Page number must be at least 1.", nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentException("Page size must be at least 1.", nameof(pageSize));

            List<Labor> labors = new List<Labor>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string whereClause = "WHERE IsActive = 1";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        whereClause += " AND (Name LIKE @searchTerm OR Area LIKE @searchTerm OR City LIKE @searchTerm OR CNIC LIKE @searchTerm OR PhoneNumber LIKE @searchTerm)";
                    }

                    string query = $@"
                        SELECT Id, Name, Area, City, PhoneNumber, CNIC, Caste, Cost, JoiningDate, 
                               CurrentAdvance, ProfileImagePath, CreatedDate, IsActive
                        FROM Labor 
                        {whereClause}
                        ORDER BY CreatedDate DESC
                        OFFSET @offset ROWS
                        FETCH NEXT @pageSize ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            labors.Add(new Labor
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "",
                                Area = reader["Area"] != DBNull.Value ? reader["Area"].ToString() : "",
                                City = reader["City"] != DBNull.Value ? reader["City"].ToString() : "",
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "",
                                CNIC = reader["CNIC"] != DBNull.Value ? reader["CNIC"].ToString() : "",
                                Caste = reader["Caste"] != DBNull.Value ? reader["Caste"].ToString() : "",
                                Cost = Convert.ToDecimal(reader["Cost"]),
                                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                                CurrentAdvance = Convert.ToDecimal(reader["CurrentAdvance"]),
                                ProfileImagePath = reader["ProfileImagePath"] != DBNull.Value ? reader["ProfileImagePath"].ToString() : "",
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving labors: {ex.Message}");
            }

            return labors;
        }

        public int GetTotalLaborsCount(string searchTerm = "")
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var whereClause = "WHERE IsActive = 1";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        whereClause += " AND (Name LIKE @searchTerm OR Area LIKE @searchTerm OR City LIKE @searchTerm OR CNIC LIKE @searchTerm OR PhoneNumber LIKE @searchTerm)";
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
                throw new Exception($"Error getting labors count: {ex.Message}");
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
                        @"SELECT Id, Name, Area, City, PhoneNumber, CNIC, Caste, Cost, JoiningDate, 
                          CurrentAdvance, ProfileImagePath, CreatedDate, IsActive 
                          FROM Labor WHERE Id = @id",
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
                                Caste = reader["Caste"] != DBNull.Value ? reader["Caste"].ToString() : "",
                                Cost = Convert.ToDecimal(reader["Cost"]),
                                JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                                CurrentAdvance = Convert.ToDecimal(reader["CurrentAdvance"]),
                                ProfileImagePath = reader["ProfileImagePath"] != DBNull.Value ? reader["ProfileImagePath"].ToString() : "",
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
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

        public bool UpdateAdvance(int laborId, decimal amount, bool isAdvance)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query;

                    if (isAdvance)
                    {
                        // Add to current advance
                        query = "UPDATE Labor SET CurrentAdvance = CurrentAdvance + @amount WHERE Id = @laborId";
                    }
                    else
                    {
                        // Reset advance (final payment)
                        query = "UPDATE Labor SET CurrentAdvance = 0 WHERE Id = @laborId";
                    }

                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@laborId", laborId);
                    cmd.Parameters.AddWithValue("@amount", amount);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating advance: {ex.Message}");
            }
        }

        public string SaveProfileImage(string laborName, byte[] imageData)
        {
            if (string.IsNullOrWhiteSpace(laborName))
                throw new ArgumentException("Labor name cannot be empty.", nameof(laborName));
            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Image data cannot be empty.", nameof(imageData));

            try
            {
                var folderPath = Path.Combine(Application.StartupPath, "ProfileImages");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = $"{laborName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                var filePath = Path.Combine(folderPath, fileName);

                File.WriteAllBytes(filePath, imageData);
                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving profile image: {ex.Message}");
            }
        }
    }
}