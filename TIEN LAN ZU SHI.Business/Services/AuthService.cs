using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TIEN_LAN_ZU_SHI.Business.DTOs;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;
using TIEN_LAN_ZU_SHI.DataAccess.Repositories;

namespace TIEN_LAN_ZU_SHI.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        //private readonly IUserRepository _userRepository;


        public AuthService(IAuthRepository authRepository , IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            //_userRepository = userRepository;
        }

        private string GenerateTokenJWT(User user, IEnumerable<string> roles)
        {
            //1. Acceder a la configuracion del JwtSetting que esta en appsettings.json
            // que contiene la clave secreta, el publicador y la audiencia

            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audiencie = _configuration["JwtSettings:Audiencie"];

            //2. Crear los claims (afirmaciones) o  clave del token
            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //3.Agregar los roles a los claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //4.Encriptacion de la llave secreta codificando antes en Bytes
            //Mecanismo de firma de las credenciales
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credencials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            //5.Definir el tiempo de expiración del token
            var expires = DateTime.UtcNow.AddHours(3);

            //6.Crear el token
            var token = new JwtSecurityToken(
                issuer:issuer,
                audience:audiencie,
                claims:claims,
                signingCredentials:credencials,
                expires:expires             
                );

            //7.Serializar el token
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        public async Task<ServiceResponse<User>> RegisterAsync(RegisterUserDto newUser)
        {
            try
            {

                var existingUser = await _authRepository.GetByUserNameAsync(newUser.UserName);

                if (existingUser.OperationStatusCode == 0 && existingUser.Data != null && !string.IsNullOrEmpty(existingUser.Data.UserName))
                {
                    return new ServiceResponse<User>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Conflict,
                        Message = "Ya existe un usuario con el nombre proporcionado"
                    };
                }



                var userEntity = new User
                {
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                    State = true 
                };

             
                var repoResponse = await _authRepository.RegisterAsync(userEntity);

               
                switch (repoResponse.OperationStatusCode)
                {
                    case 0:
                        return new ServiceResponse<User>
                        {
                            Data = repoResponse.Data,
                            IsSuccess = true,
                            MessageCode = MessageCodes.Sucess,
                            Message = "Usuario registrado correctamente."
                        };

                    case 5050:
                        return new ServiceResponse<User>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.Conflict,
                            Message = "El usuario o correo  ya existe."
                        };

                    default:
                        return new ServiceResponse<User>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = "Ocurrió un error inesperado al registrar el usuario."
                        };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<User>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = $"Ocurrió un error inesperado: {ex.Message}"
                };
            }
        }


        public async Task<ServiceResponse<User>> GetByUserNameAsync(string username)
        {
            var result = await _authRepository.GetByUserNameAsync(username);
            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<User>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            var messageCode = new MessageCodes();
            var message = string.Empty;

            switch (result.OperationStatusCode)
            {
                case 5051:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontro el usuario con ese Name proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el usuario.";
                    break;
            }

   
            return new ServiceResponse<User>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }

        
        public async Task<ServiceResponse<User>> GetByEmailAsync(string email)
        {
            var result = await _authRepository.GetByEmailAsync(email);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<User>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operación exitosa"
                };
            }

            var messageCode = MessageCodes.ErrorDataBase;
            var message = "Error al obtener el usuario";

            switch (result.OperationStatusCode)
            {
                case 5052:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontró usuario con el email proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el usuario";
                    break;
            }

            return new ServiceResponse<User>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }


        public async Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                //validar si existe el usuario con el nombre proporcionado
                var existentUser = await _authRepository.GetByUserNameAsync(loginRequest.UserName);

                if (existentUser.Data!.Id == 0 && existentUser.Data!.UserName.IsNullOrEmpty())
                {
                    return new ServiceResponse<LoginResponseDto>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Unauthorized,
                        Message = "No existe usuario registrado con el username proporcionado"
                    };
                }

                //validar el password
                var isValidPassword = BCrypt.Net.BCrypt.Verify(loginRequest.Password, existentUser.Data.PasswordHash);

                if (!isValidPassword)
                {
                    return new ServiceResponse<LoginResponseDto>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Unauthorized,
                        Message = "El password no coincide con el registrado"
                    };
                }

                //Solicitar los roles asociados al usuario 
                var roles = await _authRepository.GetRolesByUserIdAsync(existentUser.Data!.Id);

                //Generacion del JWT token
                var token = GenerateTokenJWT(existentUser.Data!, roles.Data!);

                //Mapeo  de los datos que se enviaran en  la respuesta
                var loginResponse = new LoginResponseDto
                {
                    Id = existentUser.Data!.Id,
                    UserName = existentUser.Data!.UserName,
                    Email = existentUser.Data!.Email,
                    Token = token,
                    Roles = roles.Data!
                };

                return new ServiceResponse<LoginResponseDto>
                {
                    Data = loginResponse,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Login correcto"
                };

            }
            catch (Exception)
            {
                return new ServiceResponse<LoginResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio algo inesperado"
                };
            }

        }





        
    }
}
