using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;

namespace TIEN_LAN_ZU_SHI.DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;
      
        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PagedResponse<IEnumerable<Category>>>> GetAllAsync(PaginationParams pagination)
        {
            var category = new List<Category>();
            var response = new RepositoryResponse<PagedResponse<IEnumerable<Category>>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("GetCategories", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    int totalRecords = 0;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            category.Add(new Category
                            {
                                Id = (int)reader["Id"],
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString(),
                                State = (bool)reader["State"],
                            });

                        }


                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                            totalRecords = reader.GetInt32(0);
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = new PagedResponse<IEnumerable<Category>>
                    {
                        Data = category,
                        PageNumber = pagination.PageNumber,
                        PageSize = pagination.PageSize,
                        TotalRecords = totalRecords
                    };
                    response.OperationStatusCode = returnValue;
                }
            }
            catch (SqlException ex)
            {
                response.Data = null;
                response.OperationStatusCode = ex.Number;
            }

            return response;
        }

        public async Task<RepositoryResponse<Category>> GetByIdAsync(int id)
        {
            var categoryReturned = new Category();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetCategoryById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            categoryReturned.Id = (int)reader["Id"];
                            categoryReturned.CategoryName = reader["CategoryName"].ToString();
                            categoryReturned.Description = reader["Description"].ToString();
                            categoryReturned.State = (bool)reader["State"];

                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Category>
                    {
                        Data = categoryReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Category>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }
        }


        public async Task<RepositoryResponse<Category>> AddAsync(Category category)
        {
            var categoryReturned = new Category();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpInsertCategories", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    cmd.Parameters.AddWithValue("@Description", category.Description);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            categoryReturned.Id = (int)reader["Id"];
                            categoryReturned.CategoryName = reader["CategoryName"].ToString();
                            categoryReturned.Description = reader["Description"].ToString();
                            categoryReturned.State = (bool)reader["State"];

                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Category>
                    {
                        Data = categoryReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Category>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }
        }


        public async Task<RepositoryResponse<Category>> GetByNameAsync(string name)
        {
            var Category = new Category();
            var response = new RepositoryResponse<Category>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetCategoryByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Category.Id = (int)reader["Id"];
                            Category.CategoryName = reader["CategoryName"].ToString()!;
                            Category.Description = reader["Description"].ToString();
                            Category.State = (bool)reader["State"];
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = Category;
                    response.OperationStatusCode = returnedValue;

                    return response;
                }
            }
            catch (SqlException ex)
            {
                response.Data = null;
                response.OperationStatusCode = ex.Number;
                return response;
            }
        }



        public async Task<RepositoryResponse<Category>> UpdateAsync(int id, Category category)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpUpdateCategories", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    cmd.Parameters.AddWithValue("@Description", category.Description);
                    cmd.Parameters.AddWithValue("@State", category.State);

                    Category categoryUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            categoryUpdated = new Category
                            {
                                Id = (int)reader["Id"],
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Category>
                    {
                        Data = categoryUpdated, ////MODIFIQUE PORQUE ME DABA ERROR///
                        OperationStatusCode = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Category>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }






        public async Task<RepositoryResponse<Category>> SetStateAsync(int categoryId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateCategoryState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", categoryId);
                    cmd.Parameters.AddWithValue("@State", state);

                    Category categoryUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            categoryUpdated = new Category
                            {
                                Id = (int)reader["Id"],
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Category>
                    {
                        Data = categoryUpdated,
                        OperationStatusCode = categoryUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Category>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }






    }
}



