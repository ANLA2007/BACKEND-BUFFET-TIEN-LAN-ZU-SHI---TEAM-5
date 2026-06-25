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
    public class RoleRepository:IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PagedResponse<IEnumerable<Role>>>> GetAllAsync(PaginationParams pagination)
        {
            var roles = new List<Role>();
            var response = new RepositoryResponse<PagedResponse<IEnumerable<Role>>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetRoles", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    int totalRecords = 0;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        
                        while (await reader.ReadAsync())
                        {
                            roles.Add(new Role
                            {
                                Id = (int)reader["Id"],
                                RoleName = reader["RoleName"].ToString()!,
                                State = (bool)reader["State"]
                            });
                        }

                      
                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                            totalRecords = reader.GetInt32(0);
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = new PagedResponse<IEnumerable<Role>>
                    {
                        Data = roles,
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

        public async Task<RepositoryResponse<Role>> GetByIdAsync(int id)
        {
            var roleReturned = new Role();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetRoleById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                           roleReturned.Id = (int)reader["Id"];
                           roleReturned.RoleName = reader["RoleName"].ToString();
                            roleReturned.State = (bool)reader["State"];



                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Role>
                    {
                        Data = roleReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }
        }


        public async Task<RepositoryResponse<Role>> AddAsync(Role role)
        {
            var roleReturned = new Role();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_InsertRoles", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                           roleReturned.Id = (int)reader["Id"];
                           roleReturned.RoleName = reader["RoleName"].ToString();
                           roleReturned.State = (bool)reader["State"];



                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<Role>
                    {
                        Data = roleReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }
        }


        public async Task<RepositoryResponse<Role>> GetByNameAsync(string name)
        {
            var Role = new Role();
            var response = new RepositoryResponse<Role>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetRoleByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Role.Id = (int)reader["Id"];
                            Role.RoleName = reader["RoleName"].ToString()!;
                            Role.State = (bool)reader["State"];

                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = Role;
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



        public async Task<RepositoryResponse<Role>> UpdateAsync(int id, Role role)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateRoles", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
                    cmd.Parameters.AddWithValue("@State", role.State);


                    Role roleUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            roleUpdated = new Role
                            {
                                Id = (int)reader["Id"],
                                RoleName = reader["RoleName"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Role>
                    {
                        Data = roleUpdated, ////MODIFIQUE PORQUE ME DABA ERROR///
                        OperationStatusCode = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public async Task<RepositoryResponse<Role>> SetStateAsync(int roleId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateRolesState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", roleId);
                    cmd.Parameters.AddWithValue("@State", state);

                    Role roleUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            roleUpdated = new Role
                            {
                                Id = (int)reader["Id"],
                                RoleName = reader["RoleName"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<Role>
                    {
                        Data = roleUpdated,
                        OperationStatusCode = roleUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<Role>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }




    }
}

