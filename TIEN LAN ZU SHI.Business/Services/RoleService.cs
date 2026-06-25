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
    public class RoleService:IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        //Constructor del Servicio 
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }


        public async Task<ServiceResponse<PagedResponse<IEnumerable<Role>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _roleRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<Role>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 5045:
                    return new ServiceResponse<PagedResponse<IEnumerable<Role>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };
                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<Role>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrio un error inesperado"
                    };
            }
        }

        public async Task<ServiceResponse<Role>> GetByIdAsync(int id)
        {
            var repoResponse = await _roleRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<Role>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = repoResponse.Message ?? "Operacion exitosa"

                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 5046:
                        return new ServiceResponse<Role>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontro registro asociado  al Id proporcionado"

                        };


                    default:
                        return new ServiceResponse<Role>
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
                return new ServiceResponse<Role>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"

                };


            }
        }


        public async Task<ServiceResponse<Role>> GetByNameAsync(string name)
        {
            var result = await _roleRepository.GetByNameAsync(name);
            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<Role>
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
                case 5040:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontro Rol con ese Nombre proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el rol.";
                    break;
            }

            // Retorno final para los casos de error o no encontrado ////
            return new ServiceResponse<Role>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }


        public async Task<ServiceResponse<Role>> CreateAsync(CreateRoleDto newRole)
        {
            try
            {
            
                var existingRole = await _roleRepository.GetByNameAsync(newRole.RoleName);

                if (existingRole.Data != null && !string.IsNullOrEmpty(existingRole.Data.RoleName))
                {
                    return new ServiceResponse<Role>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Conflict,
                        Message = "Existe un registro con el nombre proporcionado"
                    };
                }

                var role = new Role()
                {
                    RoleName = newRole.RoleName,
                   
                   

                };

                //Llamando al metodo repo
                var result = await _roleRepository.AddAsync(role);

                return new ServiceResponse<Role>
                {

                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro creado con exito",
                };


            }
            catch (Exception)
            {
                return new ServiceResponse<Role>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }

        }


        public async Task<ServiceResponse<Role>> UpdateAsync(int id, UpdateRoleDto role)
        {
            try
            {

                var existingIdRole = await _roleRepository.GetByIdAsync(id);
                if (existingIdRole.Data!.Id == 0 && existingIdRole.Data.RoleName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Role>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No existe rol  asociado  al Id proporcionado"

                    };
                }


                var existingNameRole = await _roleRepository.GetByNameAsync(role.RoleName);
                if (existingNameRole.Data!.RoleName != null && existingNameRole.Data.Id != id)
                {
                    return new ServiceResponse<Role>
                    {
                        Data = null,
                        IsSuccess = false,



                        MessageCode = MessageCodes.Conflict,
                        Message = "ya existe un rol con el nombre proporcionado. No se debe duplicar el nombre"
                    };
                }

                var dataRole = new Role()
                {
                    RoleName = role.RoleName,
                    State = role.State,
                   
                };

                //llamando al metodo de repo
                var result = await _roleRepository.UpdateAsync(id, dataRole);

                return new ServiceResponse<Role>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro actualizado con exito",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<Role>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }
        }


        public async Task<ServiceResponse<Role>> SetStateAsync(int roleId, bool state)
        {
            var response = new ServiceResponse<Role>();

            var existingRole = await _roleRepository.GetByIdAsync(roleId);
            if (existingRole == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "El rol no existe";
                return response;
            }


            var repoResponse = await _roleRepository.SetStateAsync(roleId, state);

            if (repoResponse.Data == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "No se pudo actualizar el estado del rol";
                return response;
            }


            response.Data = repoResponse.Data;
            response.IsSuccess = true;
            response.MessageCode = MessageCodes.Sucess;
            response.Message = state ? "Rol activado" : "Rol desactivado";

            return response;
        }
    }
}

