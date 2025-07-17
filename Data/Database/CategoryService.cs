using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MaqboolFashion.Data.Database
{
    public class CategoryService
    {
        private readonly string _connectionString;

        public CategoryService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MaqboolFashionDB"]?.ConnectionString
                            ?? @"Server=PROBOOK\SQL2022;Database=MaqboolFashionDB;Integrated Security=True;";
        }

        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsActive { get; set; }
        }

        public bool AddCategory(string name, string description)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        @"INSERT INTO Categories (Name, Description, CreatedDate, IsActive) 
                          VALUES (@name, @description, @createdDate, @isActive)",
                        connection);

                    cmd.Parameters.AddWithValue("@name", name ?? "");
                    cmd.Parameters.AddWithValue("@description", description ?? "");
                    cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@isActive", true);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding category: {ex.Message}");
            }
        }

        public bool UpdateCategory(int id, string name, string description)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        @"UPDATE Categories 
                          SET Name = @name, Description = @description 
                          WHERE Id = @id",
                        connection);

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name ?? "");
                    cmd.Parameters.AddWithValue("@description", description ?? "");

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}");
            }
        }

        public bool DeleteCategory(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        "UPDATE Categories SET IsActive = 0 WHERE Id = @id",
                        connection);

                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting category: {ex.Message}");
            }
        }

        public List<Category> GetCategories(int pageNumber = 1, int pageSize = 10, string searchTerm = "")
        {
            if (pageNumber < 1) throw new ArgumentException("Page number must be at least 1.", nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentException("Page size must be at least 1.", nameof(pageSize));

            var categories = new List<Category>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var whereClause = "WHERE IsActive = 1";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        whereClause += " AND (Name LIKE @searchTerm OR Description LIKE @searchTerm)";
                    }

                    var query = $@"
                        SELECT Id, Name, Description, CreatedDate, IsActive
                        FROM Categories 
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
                            categories.Add(new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "",
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "",
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving categories: {ex.Message}");
            }

            return categories;
        }

        public int GetTotalCategoriesCount(string searchTerm = "")
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var whereClause = "WHERE IsActive = 1";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        whereClause += " AND (Name LIKE @searchTerm OR Description LIKE @searchTerm)";
                    }

                    var query = $"SELECT COUNT(*) FROM Categories {whereClause}";
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
                throw new Exception($"Error getting categories count: {ex.Message}");
            }
        }

        public Category GetCategoryById(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand(
                        "SELECT Id, Name, Description, CreatedDate, IsActive FROM Categories WHERE Id = @id",
                        connection);

                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "",
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "",
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving category: {ex.Message}");
            }

            return null;
        }

        public bool CategoryNameExists(string name, int? excludeId = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "SELECT COUNT(*) FROM Categories WHERE Name = @name AND IsActive = 1";

                    if (excludeId.HasValue)
                    {
                        query += " AND Id != @excludeId";
                    }

                    var cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", name ?? "");

                    if (excludeId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@excludeId", excludeId.Value);
                    }

                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking category name: {ex.Message}");
            }
        }
    }
}