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
    public  class CustomerRepository:ICustomerRepository
    {
        private readonly string _connectionString;
        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PagedResponse<IEnumerable<Customer>>>> GetAllAsync(PaginationParams pagination)
        {
            var customer = new List<Customer>();
            var response = new RepositoryResponse<PagedResponse<IEnumerable<Customer>>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("GetCustomers", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    int totalRecords = 0;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            customer.Add(new Customer
                            {
                                Id = (int)reader["Id"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                State = (bool)reader["State"],
                            });
                            
                        }


                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                            totalRecords = reader.GetInt32(0);
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = new PagedResponse<IEnumerable<Customer>>
                    {
                        Data = customer,
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


        public async Task<RepositoryResponse<Customer>> GetByIdAsync(int id)
        {
            var customerReturned = new Customer();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetCustomerById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerReturned.Id = (int)reader["Id"];
                            customerReturned.FirstName = reader["FirstName"].ToString();
                            customerReturned.LastName = reader["LastName"].ToString();
                            customerReturned.Email = reader["Email"].ToString();
                            customerReturned.State = (bool)reader["State"];

                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }


            ////
            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }



        public async Task<RepositoryResponse<Customer>> AddAsync(Customer customer)
        {
            var customerReturned = new Customer();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpInsertCustomers", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerReturned.Id = (int)reader["Id"];
                            customerReturned.FirstName = reader["FirstName"].ToString();
                            customerReturned.LastName = reader["LastName"].ToString();
                            customerReturned.Email = reader["Email"].ToString();
                            customerReturned.State = (bool)reader["State"];

                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }

            ///
            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }




        public async Task<RepositoryResponse<Customer>> GetByNameAsync(string name)
        {
            var Customer = new Customer();
            var response = new RepositoryResponse<Customer>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetCustomerByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Customer.Id = (int)reader["Id"];
                            Customer.FirstName = reader["FirstName"].ToString();
                            Customer.LastName = reader["LastName"].ToString();
                            Customer.Email = reader["Email"].ToString();
                            Customer.State = (bool)reader["State"];
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = Customer;
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

           ////
            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<Customer>> UpdateAsync(int id, Customer customer)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpUpdateCustomers", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@State", customer.State);

                    Customer customerUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerUpdated = new Customer
                            {
                                Id = (int)reader["Id"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerUpdated, ///me///
                        OperationStatusCode = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }



        public async Task<RepositoryResponse<Customer>> SetStateAsync(int customerId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateCustomerState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    cmd.Parameters.AddWithValue("@State", state);

                    Customer customerUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customerUpdated = new Customer
                            {
                                Id = (int)reader["Id"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Customer>
                    {
                        Data = customerUpdated,
                        OperationStatusCode = customerUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Customer>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }






    }
}
