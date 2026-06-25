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
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<User>> RegisterAsync(User user)
        {
            var userReturned = new User();
            var response = new RepositoryResponse<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_RegisterUser", connection);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@State", user.State);


                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            userReturned.Id = (int)reader["Id"];
                            userReturned.UserName = reader["UserName"].ToString()!;
                            userReturned.Email = reader["Email"].ToString()!;
                            userReturned.State = (bool)reader["State"];
                        }
                    }


                    var returnValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = userReturned;
                    response.OperationStatusCode = returnValue;

                    return response;
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

        public async Task<RepositoryResponse<User>> GetByUserNameAsync(string name)
        {
            var User = new User();
            var response = new RepositoryResponse<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetByUserName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            User.Id = (int)reader["Id"];
                            User.UserName = reader["UserName"].ToString();
                            User.PasswordHash = reader["PasswordHash"].ToString();
                            User.Email = reader["Email"].ToString();
                            User.State = (bool)reader["State"];
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = User;
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
                return new RepositoryResponse<User>
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


        public async Task<RepositoryResponse<IEnumerable<string>>> GetRolesByUserIdAsync(int userId)
        {
            var roles = new List<string>();
            var response = new RepositoryResponse<IEnumerable<string>>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var cmd = new SqlCommand("USP_GetUserRolesByUserId", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            roles.Add(reader["RoleName"].ToString()!);
                        }
                    }

                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    response.Data = roles;
                    response.OperationStatusCode = returnedValue;
                    response.Message = "Operación exitosa";
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.OperationStatusCode = -1;
                response.Message = ex.Message;
            }


            return response;
        }


       

        
    }
}
