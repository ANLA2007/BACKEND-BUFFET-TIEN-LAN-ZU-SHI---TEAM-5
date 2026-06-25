using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using TIEN_LAN_ZU_SHI.Business.DTOs;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;
using TIEN_LAN_ZU_SHI.DataAccess.Repositories;

namespace TIEN_LAN_ZU_SHI.Business.Services
{
   public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
       

        //Constructor del Servicio 

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            
        }

       
        public async Task<ServiceResponse<PagedResponse<IEnumerable<User>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _userRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<User>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 5028:
                    return new ServiceResponse<PagedResponse<IEnumerable<User>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };
                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<User>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrio un error inesperado"
                    };
            }
        }


        public async Task<ServiceResponse<User>> GetByIdAsync(int id)
        {
            var repoResponse = await _userRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<User>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = repoResponse.Message ?? "Operacion exitosa"

                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 5033:
                        return new ServiceResponse<User>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontro registro asociado  al Id proporcionado"

                        };


                    default:
                        return new ServiceResponse<User>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = repoResponse.Message ?? "Ocurrio un error inesperado"

                        };

                }
            }
            catch (Exception)
            {
                return new ServiceResponse<User>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"

                };


            }
        }

        public async Task<ServiceResponse<User>> GetByNameAsync(string name)
        {
            var result = await _userRepository.GetByNameAsync(name);
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
                case 5041:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontro el usuario con ese Nombre proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el cliente.";
                    break;
            }

            // Retorno final para los casos de error o no encontrado ////
            return new ServiceResponse<User>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }


        public async Task<ServiceResponse<User>> CreateAsync(CreateUserDto newUser)
        {
            try
            {
            
                var existingUser = await _userRepository.GetByNameAsync(newUser.UserName);

                if (existingUser.Data!.Id != 0 && !existingUser.Data.UserName.IsNullOrEmpty())
                {
                    return new ServiceResponse<User>
                    {
                        Data = null,
                        IsSuccess = false,///*//
                        MessageCode = MessageCodes.Conflict,
                        Message = "Existe un registro con el nombre proporcionado"

                    };

                }
               
                var User = new User()
                {
                    UserName = newUser.UserName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash),
                    Email = newUser.Email,
                    

                };

                //Llamando al metodo repo
                var result = await _userRepository.AddAsync(User);

                return new ServiceResponse<User>
                {

                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro creado con exito",
                };


            }
            catch (Exception)
            {
                return new ServiceResponse<User>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }

        }


        public async Task<ServiceResponse<User>> UpdateAsync(int id, UpdateUserDto user)
        {
            try
            {
                
                var existingIdUser = await _userRepository.GetByIdAsync(id);
                if (existingIdUser.Data!.Id == 0 && existingIdUser.Data.UserName.IsNullOrEmpty())
                {
                    return new ServiceResponse<User>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No existe un usuario asociado al Id proporcionado"

                    };
                }

              
                var existingNameUser = await _userRepository.GetByNameAsync(user.UserName);
                if (existingNameUser.Data!.UserName != null && existingNameUser.Data.Id != id)
                {
                    return new ServiceResponse<User>
                    {
                        Data = null,
                        IsSuccess = false,

                      

                        MessageCode = MessageCodes.Conflict,
                        Message = "ya existe un usuario con el nombre proporcionado. No se debe duplicar el nombre"
                    };
                }
            
                var dataUser = new User()
                {
                    UserName = user.UserName,
                    //PasswordHash = user.PasswordHash,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
                    Email = user.Email,
                    State = user.State,
                };

                //llamando al metodo de repo
                var result = await _userRepository.UpdateAsync(id, dataUser);

                return new ServiceResponse<User>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro actualizado con exito",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<User>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }
        }




        public async Task<ServiceResponse<User>> SetStateAsync(int userId, bool state)
        {
            var response = new ServiceResponse<User>();

            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "El usuario no existe";
                return response;
            }

   
            var repoResponse = await _userRepository.SetStateAsync(userId, state);

            if (repoResponse.Data == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "No se pudo actualizar el estado del usuario";
                return response;
            }

       
            response.Data = repoResponse.Data;
            response.IsSuccess = true;
            response.MessageCode = MessageCodes.Sucess;
            response.Message = state ? "Usuario activado" : "Usuario desactivado";

            return response;
        }



        public async Task<ServiceResponse<UserRole>> AssignUserRoleAsync(int userId, int roleId)
        {
            var result = await _userRepository.AssignUserRoleAsync(userId, roleId);


            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<UserRole>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Rol asignado correctamente"
                };
            }

            var messageCode = MessageCodes.ErrorDataBase;
            var message = "Ocurrió un error al asignar el rol";

            switch (result.OperationStatusCode)
            {
                case 5061:
                    messageCode = MessageCodes.NotFound;
                    message = "Usuario no encontrado";
                    break;
                case 5062:
                    messageCode = MessageCodes.NotFound;
                    message = "Rol no encontrado";
                    break;
                case 5063:
                    messageCode = MessageCodes.Sucess;
                    message = "El rol ya esta asignado a este usuario";
                    break;
                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error inesperado al asignar el rol";
                    break;
            }

            return new ServiceResponse<UserRole>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };


        }


        public async Task<ServiceResponse<User>> GetByEmailAsync(string email)
        {
            var result = await _userRepository.GetByEmailAsync(email);

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




    }

}

