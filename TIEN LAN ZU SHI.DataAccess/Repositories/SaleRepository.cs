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
    public class SaleRepository:ISaleRepository
    {
        private readonly string _connectionString;

        public SaleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<SaleTransaction>> InsertAsync(Sale master,IEnumerable<SaleDetail> details)
        {
            var transaction = new SaleTransaction();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_InsertSale", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Definir Parametros
                    cmd.Parameters.AddWithValue("@CustomerId", master.CustomerId);
                    cmd.Parameters.AddWithValue("@DateTime", master.DateTime);

                    //crear un objeto de tipo DataTable
                    var detailsTable = new DataTable();
                    detailsTable.Columns.Add("PortionId", typeof(int));
                    detailsTable.Columns.Add("Quantity", typeof(int));
                    detailsTable.Columns.Add("PortionType", typeof(string));


                    //Agegando los registros al DataTable
                    foreach (var item in details)
                    {
                        detailsTable.Rows.Add(item.PortionId, item.Quantity,item.PortionType);
                    }
                    //Pasar el DataTable como parametro del comando SQL Command
                    SqlParameter detailParm = cmd.Parameters.AddWithValue("@SaleDetails", detailsTable);
                    detailParm.SqlDbType = SqlDbType.Structured;
                    detailParm.TypeName = "SaleDetailsType";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            transaction.Master = new Sale
                            {
                                Id = (int)reader["Id"],
                                CustomerId = (int)reader["CustomerId"],
                                DateTime = (DateTime)reader["DateTime"],
                                Total = (decimal)reader["Total"]
                            };
                        }

                        await reader.NextResultAsync();

                        //crear un objeto de tipo list para capturar los detalles
                        var detailsList = new List<SaleDetail>();
                        while (await reader.ReadAsync())
                        {
                            detailsList.Add(new SaleDetail
                            {
                                Id = (int)reader["Id"],
                                SaleId= (int)reader["SaleId"],
                                PortionId = (int)reader["PortionId"],
                                Quantity = (int)reader["Quantity"],
                                SalePrice = (decimal)reader["SalePrice"],
                                LineTotal = (decimal)reader["LineTotal"],
                                PortionType = reader["PortionType"] == DBNull.Value ? "" : (string)reader["PortionType"],
                            });
                        }

                        transaction.Details = detailsList;
                    }

                    return new RepositoryResponse<SaleTransaction>
                    {
                        Data = transaction,
                        OperationStatusCode = 0,
                        Message = "Operacion exitosa"
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<SaleTransaction>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }





        public async Task<RepositoryResponse<List<SaleTransaction>>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = new List<SaleTransaction>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_SalesByDateRange", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            
                            var transaction = new SaleTransaction
                            {
                                Master = new Sale
                                {
                                    Id = (int)reader["SaleId"],
                                    CustomerId = (int)reader["CustomerId"],
                                    DateTime = (DateTime)reader["DateTime"],
                                    Total = (decimal)reader["SaleTotal"]
                                }
                            };

                         
                            transaction.Details = new List<SaleDetail>
                            {
                             new SaleDetail
                              {
                              Id = (int)reader["SaleDetailId"],
                              SaleId = (int)reader["SaleId"],
                              PortionId = (int)reader["PortionId"],
                              Quantity = (int)reader["Quantity"],
                              SalePrice = (decimal)reader["SalePrice"],
                              LineTotal = (decimal)reader["LineTotal"],
                              PortionType = reader["PortionType"] == DBNull.Value ? "" : (string)reader["PortionType"],
                               }
                           };

                            transactions.Add(transaction);
                        }
                    }

                    return new RepositoryResponse<List<SaleTransaction>>
                    {
                        Data = transactions,
                        OperationStatusCode = transactions.Any() ? 0 : 1, 
                        Message = transactions.Any() ? "Operación exitosa" : "No se encontraron ventas en el rango de fechas"
                    };

                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<List<SaleTransaction>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }



        public async Task<RepositoryResponse<SaleTransaction>> GetBySaleIdAsync(int saleId)
        {
            var transaction = new SaleTransaction();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetSaleById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                     
                        if (await reader.ReadAsync())
                        {
                            transaction.Master = new Sale
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                DateTime = Convert.ToDateTime(reader["DateTime"]),
                                Total = Convert.ToDecimal(reader["Total"])
                            };
                        }

                      
                        await reader.NextResultAsync();

                        var detailsList = new List<SaleDetail>();
                        while (await reader.ReadAsync())
                        {
                            detailsList.Add(new SaleDetail
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SaleId = Convert.ToInt32(reader["SaleId"]),
                                PortionId = Convert.ToInt32(reader["PortionId"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                SalePrice = Convert.ToDecimal(reader["SalePrice"]),
                                LineTotal = Convert.ToDecimal(reader["LineTotal"]),
                                PortionType = reader["PortionType"] == DBNull.Value ? "" : (string)reader["PortionType"],
                            });
                        }

                       
                        transaction.Details = detailsList;
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<SaleTransaction>
                    {
                        Data = transaction,
                        OperationStatusCode = returnedValue
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<SaleTransaction>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<SaleTransaction>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<PagedResponse<IEnumerable<SaleTransaction>>>> GetAllAsync(PaginationParams pagination)
        {
            var sales = new List<SaleTransaction>();
            var salesDict = new Dictionary<int, SaleTransaction>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetAllSales", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    int totalRecords = 0;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var id = Convert.ToInt32(reader["Id"]);

                            var transaction = new SaleTransaction
                            {
                                Master = new Sale
                                {
                                    Id = id,
                                    CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                    DateTime = Convert.ToDateTime(reader["DateTime"]),
                                    Total = Convert.ToDecimal(reader["Total"])
                                },
                                Details = new List<SaleDetail>()
                            };

                            sales.Add(transaction);
                            salesDict[id] = transaction;
                        }


                        if (await reader.NextResultAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                            }
                        }

                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var detail = new SaleDetail
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    SaleId = Convert.ToInt32(reader["SaleId"]),
                                    PortionId = Convert.ToInt32(reader["PortionId"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    SalePrice = Convert.ToDecimal(reader["SalePrice"]),
                                    LineTotal = Convert.ToDecimal(reader["LineTotal"]),
                                    PortionType = reader["PortionType"] == DBNull.Value ? "" : (string)reader["PortionType"],
                                };

                                if (salesDict.TryGetValue(detail.SaleId, out var transaction))
                                {
                                    transaction.Details = transaction.Details.Append(detail);
                                }
                            }
                        }

                        var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                        return new RepositoryResponse<PagedResponse<IEnumerable<SaleTransaction>>>
                        {
                            Data = new PagedResponse<IEnumerable<SaleTransaction>>
                            {
                                Data = sales,
                                TotalRecords = totalRecords,
                                PageNumber = pagination.PageNumber,
                                PageSize = pagination.PageSize
                            },
                            OperationStatusCode = returnedValue
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<PagedResponse<IEnumerable<SaleTransaction>>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }
        public async Task<RepositoryResponse<List<SaleDetail>>> GetDetailsBySaleIdAsync(int saleId)
        {
            var details = new List<SaleDetail>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetSaleDetailsBySaleId", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            details.Add(new SaleDetail
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SaleId = Convert.ToInt32(reader["SaleId"]),
                                PortionId = Convert.ToInt32(reader["PortionId"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                SalePrice = Convert.ToDecimal(reader["SalePrice"]),
                                LineTotal = Convert.ToDecimal(reader["LineTotal"]),
                                PortionType = reader["PortionType"] == DBNull.Value ? "" : (string)reader["PortionType"],

                            });
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<List<SaleDetail>>
                    {
                        Data = details,
                        OperationStatusCode = returnedValue
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<List<SaleDetail>>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<List<SaleDetail>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }






    }
}
