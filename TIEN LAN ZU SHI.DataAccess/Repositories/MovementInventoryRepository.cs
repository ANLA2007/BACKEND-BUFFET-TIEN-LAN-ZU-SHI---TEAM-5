using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;

namespace TIEN_LAN_ZU_SHI.DataAccess.Repositories
{
    public class MovementInventoryRepository : IMovementInventoryRepository
    {
        private readonly string _connectionString;

        public MovementInventoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PagedResponse<IEnumerable<MovementInventory>>>> GetAllAsync(PaginationParams pagination)
        {
            var movementinventory = new List<MovementInventory>();
            var response = new RepositoryResponse<PagedResponse<IEnumerable<MovementInventory>>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("GetMovementInventory", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    int totalRecords = 0;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            movementinventory.Add(new MovementInventory
                            {
                                Id = (int)reader["Id"],
                                PortionId = (int)reader["PortionId"],
                                MovementType = reader["MovementType"].ToString()!,
                                Quantity = (int)reader["Quantity"],
                                PreviousDailyStock = (int)reader["PreviousDailyStock"],
                                NewDailyStock = (int)reader["NewDailyStock"],
                                PreviousAccumulatedStock = (int)reader["PreviousAccumulatedStock"],
                                NewAccumulatedStock = (int)reader["NewAccumulatedStock"],
                                Justification = reader["Justification"] == DBNull.Value ? null : reader["Justification"].ToString(),
                                MovementDate = Convert.ToDateTime(reader["MovementDate"])
                            });
                        }


                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                            totalRecords = reader.GetInt32(0);
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = new PagedResponse<IEnumerable<MovementInventory>>
                    {
                        Data = movementinventory,
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

        public async Task<RepositoryResponse<MovementInventory>> AddAsync(MovementInventory movementInventory)
        {
            MovementInventory? movementReturned = null;

            var response = new RepositoryResponse<MovementInventory>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_InsertMovementInventory", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PortionId", movementInventory.PortionId);
                    cmd.Parameters.AddWithValue("@MovementType", movementInventory.MovementType);
                    cmd.Parameters.AddWithValue("@Quantity", movementInventory.Quantity);
                   // cmd.Parameters.AddWithValue("@Justification", (object?)movementInventory.Justification ?? DBNull.Value);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            movementReturned = new MovementInventory
                            {
                                Id = (int)reader["Id"],
                                PortionId = (int)reader["PortionId"],
                                MovementType = reader["MovementType"].ToString()!,
                                Quantity = (int)reader["Quantity"],
                                PreviousDailyStock = (int)reader["PreviousDailyStock"],
                                NewDailyStock = (int)reader["NewDailyStock"],
                                PreviousAccumulatedStock = (int)reader["PreviousAccumulatedStock"],
                                NewAccumulatedStock = (int)reader["NewAccumulatedStock"],
                                Justification = reader["Justification"] == DBNull.Value ? null : reader["Justification"].ToString(),
                                MovementDate = Convert.ToDateTime(reader["MovementDate"])
                            };
                        }
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = movementReturned;
                    response.OperationStatusCode = returnValue;
                }
            }
            catch (SqlException ex)
            {
                response.Data = null;
                response.OperationStatusCode = ex.Number;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<RepositoryResponse<MovementInventory>> GetPortionByIdAsync(int id)
        {
            MovementInventory? movementinventory = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetMovementInventoryById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PortionId", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            movementinventory = new MovementInventory
                            {
                                Id = (int)reader["Id"],
                                PortionId = (int)reader["PortionId"],
                                MovementType = reader["MovementType"].ToString()!,
                                Quantity = (int)reader["Quantity"],
                                PreviousDailyStock = (int)reader["PreviousDailyStock"],
                                NewDailyStock = (int)reader["NewDailyStock"],
                                PreviousAccumulatedStock = (int)reader["PreviousAccumulatedStock"],
                                NewAccumulatedStock = (int)reader["NewAccumulatedStock"],
                                Justification = reader["Justification"] == DBNull.Value ? null : reader["Justification"].ToString(),
                                MovementDate = Convert.ToDateTime(reader["MovementDate"])
                            };
                        }
                    }

                    int returnedValue = (movementinventory != null) ? 0 : -1;

                    return new RepositoryResponse<MovementInventory>
                    {
                        Data = movementinventory,
                        OperationStatusCode = returnedValue
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<MovementInventory>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<MovementInventory>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<List<MovementInventory>>> GetByMovementTypeAsync(string movementType)
        {
            var movements = new List<MovementInventory>();
            var response = new RepositoryResponse<List<MovementInventory>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetMovementInventoryByType", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MovementType", movementType);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            movements.Add(new MovementInventory
                            {
                                Id = (int)reader["Id"],
                                PortionId = (int)reader["PortionId"],
                                MovementType = reader["MovementType"].ToString()!,
                                Quantity = (int)reader["Quantity"],
                                PreviousDailyStock = (int)reader["PreviousDailyStock"],
                                NewDailyStock = (int)reader["NewDailyStock"],
                                PreviousAccumulatedStock = (int)reader["PreviousAccumulatedStock"],
                                NewAccumulatedStock = (int)reader["NewAccumulatedStock"],
                                Justification = reader["Justification"].ToString()!,
                                MovementDate = (DateTime)reader["MovementDate"]
                            });
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = movements;
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
            catch (Exception ex)
            {
                return new RepositoryResponse<List<MovementInventory>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }



    }
}




