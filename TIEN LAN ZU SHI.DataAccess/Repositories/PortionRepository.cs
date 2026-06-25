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
   public  class PortionRepository: IPortionRepository
    {
        private readonly string _connectionString;
        public PortionRepository (IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PagedResponse<IEnumerable<Portion>>>> GetAllAsync(PaginationParams pagination)
        {
            var portion = new List<Portion>();
            var response = new RepositoryResponse<PagedResponse<IEnumerable<Portion>>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("GetProducts", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    int totalRecords = 0;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            portion.Add(new Portion
                            {
                                Id = (int)reader["Id"],
                                CategoryId = (int)reader["CategoryId"],
                                CategoryName = reader["CategoryName"].ToString()!,
                                PortionName = reader["PortionName"].ToString()!,
                                PortionPrice = Convert.ToDecimal(reader["PortionPrice"].ToString()),
                                Description = reader["Description"].ToString()!,
                                State = (bool)reader["State"],
                                PortionType = reader["PortionType"].ToString()!,
                                Reconsumable = (bool)reader["Reconsumable"],
                            });
                        }


                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                            totalRecords = reader.GetInt32(0);
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = new PagedResponse<IEnumerable<Portion>>
                    {
                        Data = portion,
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


        public async Task<RepositoryResponse<Portion>> GetByIdAsync(int id)
        {
            var portionReturned = new Portion();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetProductById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            portionReturned.Id = (int)reader["Id"];
                            portionReturned.CategoryId = (int)reader["CategoryId"];
                            portionReturned.CategoryName=reader["CategoryName"].ToString();
                            portionReturned.PortionName = reader["PortionName"].ToString();
                            portionReturned.PortionPrice = Convert.ToDecimal(reader["PortionPrice"].ToString());
                            portionReturned.Description = reader["Description"].ToString();
                            portionReturned.State = (bool)reader["State"];
                            portionReturned.Reconsumable = (bool)reader["Reconsumable"];
                            portionReturned.PortionType = reader["PortionType"].ToString();
                           


                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Portion>
                    {
                        Data = portionReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Portion>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }
        }


        public async Task<RepositoryResponse<Portion>> AddAsync(Portion portion)
        {
            var portionReturned = new Portion();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpInsertProduct", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", portion.CategoryId);
                    cmd.Parameters.AddWithValue("@PortionName", portion.PortionName);
                    cmd.Parameters.AddWithValue("@PortionPrice", portion.PortionPrice);
                    cmd.Parameters.AddWithValue("@Description", portion.Description);
                    cmd.Parameters.AddWithValue("@State", portion.State);
                    cmd.Parameters.AddWithValue("@Reconsumable", portion.Reconsumable);
                    cmd.Parameters.AddWithValue("@PortionType", portion.PortionType);

                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            portionReturned.Id = (int)reader["Id"];
                            portionReturned.CategoryId = (int)reader["CategoryId"];
                            portionReturned.PortionName = reader["PortionName"].ToString();
                            portionReturned.PortionPrice = Convert.ToDecimal(reader["PortionPrice"].ToString());
                            portionReturned.Description = reader["Description"].ToString();
                            portionReturned.State = (bool)reader["State"];
                            portionReturned.Reconsumable = (bool)reader["Reconsumable"];
                            portionReturned.PortionType = reader["PortionType"].ToString();

                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Portion>
                    {
                        Data = portionReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Portion>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }
        }


        public async Task<RepositoryResponse<Portion>> GetByNameAsync(string name)
        {
            var Portion = new Portion();
            var response = new RepositoryResponse<Portion>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetProductByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync()) 
                    {
                        if (await reader.ReadAsync())
                        {
                            Portion.Id = (int)reader["Id"];
                            Portion.CategoryId = (int)reader["CategoryId"];
                            Portion.CategoryName = reader["CategoryName"].ToString();
                            Portion.PortionName = reader["PortionName"].ToString();
                            Portion.PortionPrice = Convert.ToDecimal(reader["PortionPrice"].ToString());
                            Portion.Description = reader["Description"].ToString();
                            Portion.State = (bool)reader["State"];
                            Portion.Reconsumable = (bool)reader["Reconsumable"];
                            Portion.PortionType = reader["PortionType"].ToString();
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = Portion;
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


        public async Task<RepositoryResponse<Portion>> UpdateAsync(int id,Portion portion)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpUpdateProduct", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@CategoryId", portion.CategoryId);
                    cmd.Parameters.AddWithValue("@PortionName", portion.PortionName);
                    cmd.Parameters.AddWithValue("@PortionPrice", portion.PortionPrice);
                    cmd.Parameters.AddWithValue("@Description", portion.Description);
                    cmd.Parameters.AddWithValue("@State", portion.State);
                    cmd.Parameters.AddWithValue("@Reconsumable", portion.Reconsumable);
                    cmd.Parameters.AddWithValue("@PortionType", portion.PortionType);

                    Portion portionUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                           portionUpdated = new Portion
                            {
                                Id = (int)reader["Id"],
                                CategoryId = (int)reader["CategoryId"],
                                PortionName = reader["PortionName"].ToString(),
                                PortionPrice = Convert.ToDecimal(reader["PortionPrice"].ToString()),
                                Description = reader["Description"].ToString(),
                                State = (bool)reader["State"],
                               Reconsumable = (bool)reader["Reconsumable"],
                               PortionType = reader["PortionType"].ToString(),
                            };
                        }
                    }

                    return new RepositoryResponse<Portion>
                    {
                        Data = portionUpdated, 
                        OperationStatusCode = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Portion>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<Portion>> SetStateAsync(int portionId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateProductsState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", portionId);
                    cmd.Parameters.AddWithValue("@State", state);

                    Portion portionUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            portionUpdated = new Portion
                            {
                                Id = (int)reader["Id"],
                                CategoryId = (int)reader["CategoryId"],
                                PortionName = reader["PortionName"].ToString(),
                                PortionPrice = Convert.ToDecimal(reader["PortionPrice"]),
                                Description = reader["Description"].ToString(),
                                State = (bool)reader["State"],
                                Reconsumable = (bool)reader["Reconsumable"],
                                PortionType = reader["PortionType"].ToString()
                            };
                        }
                    }

                    return new RepositoryResponse<Portion>
                    {
                        Data = portionUpdated,
                        OperationStatusCode = portionUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Portion>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Portion>> SetReconsumableAsync(int portionId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateProductsReconsumable", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", portionId);
                    cmd.Parameters.AddWithValue("@Reconsumable", state);

                    Portion portionUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            portionUpdated = new Portion
                            {
                                Id = (int)reader["Id"],
                                CategoryId = (int)reader["CategoryId"],
                                PortionName = reader["PortionName"].ToString(),
                                PortionPrice = Convert.ToDecimal(reader["PortionPrice"]),
                                Description = reader["Description"].ToString(),
                                State = (bool)reader["State"],
                                Reconsumable = (bool)reader["Reconsumable"],
                                PortionType = reader["PortionType"].ToString(),
                            };
                        }
                    }

                    return new RepositoryResponse<Portion>
                    {
                        Data = portionUpdated,
                        OperationStatusCode = portionUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Portion>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }





    }
}


