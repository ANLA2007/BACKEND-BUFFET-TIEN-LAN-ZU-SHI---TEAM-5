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
    public  class InventoryRepository:IInventoryRepository
    {
        private readonly string _connectionString;
        public InventoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PagedResponse<IEnumerable<Inventory>>>> GetAllAsync(PaginationParams pagination)
        {
            var inventory = new List<Inventory>();
            var response = new RepositoryResponse<PagedResponse<IEnumerable<Inventory>>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("GetInventories", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    int totalRecords = 0;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            inventory.Add(new Inventory
                            {
                                Id = (int)reader["Id"],
                                PortionId = (int)reader["PortionId"],
                                Date = Convert.ToDateTime(reader["Date"]),
                                DailyStock = (int)reader["DailyStock"],
                                AccumulatedStock = (int)reader["AccumulatedStock"],
                                SalePrice = Convert.ToDecimal(reader["SalePrice"])
                            });
                        }


                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                            totalRecords = reader.GetInt32(0);
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = new PagedResponse<IEnumerable<Inventory>>
                    {
                        Data = inventory,
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



        public async Task<RepositoryResponse<Inventory>> AddAsync(Inventory inventory)
        {
            var inventoryReturned = new Inventory();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_InsertInventories", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PortionId", inventory.PortionId);
                    cmd.Parameters.AddWithValue("@Date", inventory.Date.Date); 
                    cmd.Parameters.AddWithValue("@DailyStock", inventory.DailyStock);
                    cmd.Parameters.AddWithValue("@AccumulatedStock", inventory.AccumulatedStock);
                    cmd.Parameters.AddWithValue("@SalePrice", inventory.SalePrice);


                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            inventoryReturned.Id = (int)reader["Id"];
                            inventoryReturned.PortionId = (int)reader["PortionId"];
                            inventoryReturned.Date = Convert.ToDateTime(reader["Date"]);
                            inventoryReturned.DailyStock = (int)reader["DailyStock"];
                            inventoryReturned.AccumulatedStock = (int)reader["AccumulatedStock"];
                            inventoryReturned.SalePrice = Convert.ToDecimal(reader["SalePrice"]);
                        }
                    }

     
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Inventory>
                    {
                        Data = inventoryReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Inventory>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }
        }




        public async Task<RepositoryResponse<Inventory>> GetByPortionAndDateAsync(int portionId, DateTime date)
        {
            Inventory inventory = null;

            var response = new RepositoryResponse<Inventory>();

            try
            {
                
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetInventoryByProductAndDate", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PortionId", portionId);
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            inventory = new Inventory
                            {
                                Id = (int)reader["Id"],
                                PortionId = (int)reader["PortionId"],
                                Date = Convert.ToDateTime(reader["Date"]),
                                DailyStock = (int)reader["DailyStock"],
                                AccumulatedStock = (int)reader["AccumulatedStock"],
                                SalePrice = Convert.ToDecimal(reader["SalePrice"])
                            };
                        }
                    }

                   
                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = inventory;
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





        public async Task<RepositoryResponse<Inventory>> GetPortionByIdAsync(int id)
        {
            Inventory? inventory = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetProductByIdFromInventory", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PortionId", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            inventory = new Inventory
                            {
                                Id = (int)reader["Id"],
                                PortionId = (int)reader["PortionId"],
                                DailyStock = (int)reader["DailyStock"],
                                AccumulatedStock = (int)reader["AccumulatedStock"],
                                SalePrice =(decimal) reader["SalePrice"]
                            };
                        }
                    }

                    int returnedValue = (inventory != null) ? 0 : -1;

                    return new RepositoryResponse<Inventory>
                    {
                        Data = inventory,
                        OperationStatusCode = returnedValue
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Inventory>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Inventory>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }



    }
}
