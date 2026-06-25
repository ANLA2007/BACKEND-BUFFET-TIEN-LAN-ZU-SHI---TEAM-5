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
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PagedResponse<IEnumerable<User>>>> GetAllAsync(PaginationParams pagination)
        {
            var Users = new List<User>();

            var response = new RepositoryResponse<PagedResponse<IEnumerable<User>>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("GetUsers", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageNumber", pagination.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);

                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var userId = (int)reader["Id"];

                            var user = Users.FirstOrDefault(u => u.Id == userId);

                            if (user == null)
                            {
                                user = new User
                                {
                                    Id = userId,
                                    UserName = reader["UserName"].ToString()!,
                                    PasswordHash = reader["PasswordHash"].ToString()!,
                                    Email = reader["Email"].ToString()!,
                                    State = Convert.ToBoolean(reader["State"]),
                                    Roles = new List<string>()
                                };

                                Users.Add(user);
                            }

                            if (reader["RoleName"] != DBNull.Value)
                            {
                                var roleName = reader["RoleName"].ToString()!;

                                if (!user.Roles.Contains(roleName))
                                {
                                    user.Roles.Add(roleName);
                                }
                            }
                        }

                      
                        await reader.NextResultAsync();

                        int totalRecords = 0;

                        if (await reader.ReadAsync())
                        {
                            totalRecords = Convert.ToInt32(reader["TotalRecords"]);
                        }

                        response.Data = new PagedResponse<IEnumerable<User>>
                        {
                            Data = Users,
                            TotalRecords = totalRecords,
                            PageNumber = pagination.PageNumber,
                            PageSize = pagination.PageSize
                        };
                    }

                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    response.OperationStatusCode = returnValue;
                }
            }
            catch (SqlException ex)
            {
                response.Data = null;
                response.OperationStatusCode = ex.Number;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.OperationStatusCode = -1;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<RepositoryResponse<User>> GetByIdAsync(int id)
        {
            var userReturned = new User();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetUserById", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    //using (var reader = await cmd.ExecuteReaderAsync())
                    //{
                    //    if (await reader.ReadAsync())
                    //    {
                    //        userReturned.Id = (int)reader["Id"];
                    //        userReturned.UserName = reader["UserName"].ToString();
                    //        userReturned.PasswordHash = reader["PasswordHash"].ToString();
                    //        userReturned.Email = reader["Email"].ToString();
                    //        userReturned.State = (bool)reader["State"];
                    //        userReturned.Roles = new List<string>();

                    //    }
                    //}
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        userReturned.Roles = new List<string>();

                        while (await reader.ReadAsync())
                        {
                          
                            if (userReturned.Id == 0)
                            {
                                userReturned.Id = (int)reader["Id"]; 
                                userReturned.UserName = reader["UserName"].ToString();
                                userReturned.PasswordHash = reader["PasswordHash"].ToString();
                                userReturned.Email = reader["Email"].ToString();
                                userReturned.State = Convert.ToBoolean(reader["State"]);
                            }

                           
                            if (reader["RoleName"] != DBNull.Value)
                            {
                                var roleName = reader["RoleName"].ToString();
                                if (!string.IsNullOrEmpty(roleName) && !userReturned.Roles.Contains(roleName))
                                {
                                    userReturned.Roles.Add(roleName);
                                }
                            }
                        }

                        var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                        return new RepositoryResponse<User>
                        {
                            Data = userReturned,
                            OperationStatusCode = returnedValue
                        };
                    }



                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }

            catch (Exception ex)
            {
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<User>> AddAsync(User user)
        {
            var userReturned = new User();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpInsertUsers", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@Email", user.Email);

                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userReturned.Id = (int)reader["Id"];
                            userReturned.UserName = reader["UserName"].ToString();
                            userReturned.PasswordHash = reader["PasswordHash"].ToString();
                            userReturned.Email = reader["Email"].ToString();
                            userReturned.State = (bool)reader["State"];


                        }
                    }

                    //capturamos el codigo de retorno
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return new RepositoryResponse<User>
                    {
                        Data = userReturned,
                        OperationStatusCode = returnedValue

                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message

                };

            }

            ///
            catch (Exception ex)
            {
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<User>> GetByNameAsync(string name)
        {
            var User = new User();
            var response = new RepositoryResponse<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetUserByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    //using (var reader = await cmd.ExecuteReaderAsync())
                    //{
                    //    if (await reader.ReadAsync())
                    //    {
                    //        User.Id = (int)reader["Id"];
                    //        User.UserName = reader["UserName"].ToString();
                    //        User.PasswordHash = reader["PasswordHash"].ToString();
                    //        User.Email = reader["Email"].ToString();
                    //        User.State = (bool)reader["State"];
                    //        User.Roles = new List<string>();
                    //    }
                    //}
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        User.Roles = new List<string>();

                        while (await reader.ReadAsync())
                        {
                          
                            if (User.Id == 0)
                            {
                                User.Id = (int)reader["UserId"]; 
                                User.UserName = reader["UserName"].ToString();
                                User.PasswordHash = reader["PasswordHash"].ToString();
                                User.Email = reader["Email"].ToString();
                                User.State = Convert.ToBoolean(reader["State"]);
                            }

                            // Agregar roles si existen
                            if (reader["RoleName"] != DBNull.Value)
                            {
                                var roleName = reader["RoleName"].ToString();
                                if (!User.Roles.Contains(roleName))
                                    User.Roles.Add(roleName);
                            }
                        }

                        var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                        response.Data = User;
                        response.OperationStatusCode = returnedValue;
                        return response;
                    }


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
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<User>> UpdateAsync(int id, User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SpUpdateUsers", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@State", user.State);




                    User userUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userUpdated = new User
                            {
                                Id = (int)reader["Id"],
                                UserName = reader["UserName"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Email = reader["Email"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<User>
                    {
                        Data = userUpdated, ////me///
                        OperationStatusCode = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }






        public async Task<RepositoryResponse<User>> SetStateAsync(int userId, bool state)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_UpdateUserState", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", userId);
                    cmd.Parameters.AddWithValue("@State", state);

                    User userUpdated = null;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userUpdated = new User
                            {
                                Id = (int)reader["Id"],
                                UserName = reader["UserName"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Email = reader["Email"].ToString(),
                                State = (bool)reader["State"]
                            };
                        }
                    }

                    return new RepositoryResponse<User>
                    {
                        Data = userUpdated,
                        OperationStatusCode = userUpdated != null ? 0 : 1
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<UserRole>> AssignUserRoleAsync(int userId, int roleId)
        {
            var userRole = new UserRole();
            var response = new RepositoryResponse<UserRole>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_AssignUserRole", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                  
                    await cmd.ExecuteNonQueryAsync();

                    
                    userRole.UserId = userId;
                    userRole.RoleId = roleId;

                    response.Data = userRole;
                    response.OperationStatusCode = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return response;
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<UserRole>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<UserRole>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<User>> GetByEmailAsync(string email)
        {
            var user = new User();
            var response = new RepositoryResponse<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetUserByEmail", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user.Id = (int)reader["Id"];
                            user.UserName = reader["UserName"].ToString()!;
                            user.PasswordHash = reader["PasswordHash"].ToString()!;
                            user.Email = reader["Email"].ToString()!;
                            user.State = (bool)reader["State"];
                            
                        }
                    }

                    response.Data = user;
                    response.OperationStatusCode = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    return response;
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<User>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }



    }
}



