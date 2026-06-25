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
    public class PortionService:IPortionService
    {
        private readonly IPortionRepository _portionRepository;
        //private readonly ICategoryRepository _categoryRepository;

       
        public PortionService(IPortionRepository portionRepository)
        {
            _portionRepository = portionRepository;
            //_categoryRepository = categoryRepository;
        }


        public async Task<ServiceResponse<PagedResponse<IEnumerable<Portion>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _portionRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<Portion>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 5026:
                    return new ServiceResponse<PagedResponse<IEnumerable<Portion>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };
                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<Portion>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrio un error inesperado"
                    };
            }
        }



        public async Task<ServiceResponse<Portion>> GetByIdAsync(int id)
        {
            var repoResponse = await _portionRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<Portion>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = repoResponse.Message ?? "Operacion exitosa"

                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 5031:
                        return new ServiceResponse<Portion>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontro registro asociado  al Id proporcionado"

                        };


                    default:
                        return new ServiceResponse<Portion>
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
                return new ServiceResponse<Portion>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"

                };


            }
        }


        public async Task<ServiceResponse<Portion>> GetByNameAsync(string name)
        {
            var result = await _portionRepository.GetByNameAsync(name);
            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<Portion>
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
                case 5039:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontro porcion con ese Nombre proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el producto.";
                    break;
            }

            
            return new ServiceResponse<Portion>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }


        public async Task<ServiceResponse<Portion>> CreateAsync(CreatePortionDto newPortion)
        {
            try
            {
                
                var existingPortion = await _portionRepository.GetByNameAsync(newPortion.PortionName);

                if (existingPortion.Data!.Id != 0 && !existingPortion.Data.PortionName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Portion>
                    {
                        Data = null,
                        IsSuccess = false,////*//
                        MessageCode = MessageCodes.Conflict,
                        Message = "Existe un registro con el nombre proporcionado"
                    };
                }

            
                //var categoryResponse = await _categoryRepository.GetByNameAsync(newProduct.CategoryName);
                //if (categoryResponse.Data == null)
                //{
                //    return new ServiceResponse<Product>
                //    {
                //        Data = null,
                //        IsSuccess = false,
                //        MessageCode = MessageCodes.NotFound,
                //        Message = $"No existe una categoría con el nombre '{newProduct.CategoryName}'"
                //    };
                //}

                
                var portion = new Portion()
                {
                    //CategoryId = categoryResponse.Data.Id,
                    CategoryId = newPortion.CategoryId,
                    PortionName = newPortion.PortionName,
                    PortionPrice = newPortion.PortionPrice,
                    Description = newPortion.Description,
                    Reconsumable = newPortion.Reconsumable,
                    PortionType = newPortion.PortionType,
                    State=newPortion.State,
                };

              
                var result = await _portionRepository.AddAsync(portion);

                return new ServiceResponse<Portion>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro creado con exito",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<Portion>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }
        }



        public async Task<ServiceResponse<Portion>> UpdateAsync(int id, UpdatePortionDto portion)
        {
            try
            {
                //validar que la categoria exista segun Id
                var existingIdPortion = await _portionRepository.GetByIdAsync(id);
                if (existingIdPortion.Data!.Id == 0 && existingIdPortion.Data.PortionName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Portion>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No existe un producto asociado al Id proporcionado"

                    };
                }

                //validar que el nombre enviado para el producto  no coincida con otro nombre existente
                var existingNamePortion = await _portionRepository.GetByNameAsync(portion.PortionName);
                if (existingNamePortion.Data!.PortionName != null && existingNamePortion.Data.Id != id)
                {
                    return new ServiceResponse<Portion>
                    {
                        Data = null,
                        IsSuccess = false,

                        //NOTA: Agregar un nuevo tipo de codigo al Enum MessageCodes 
                        //Este seria conflict

                        MessageCode = MessageCodes.Conflict,
                        Message = "ya existe un producto con el nombre proporcionado. No se debe duplicar el nombre"
                    };
                }
                //mapeo 
                var dataPortion = new Portion()
                {
                    CategoryId = portion.CategoryId, // ///
                    PortionName = portion.PortionName,
                    PortionPrice = portion.PortionPrice,
                    Description = portion.Description,
                    State = portion.State,
                    Reconsumable = portion.Reconsumable,
                    PortionType = portion.PortionType,
                    
                };


                //llamando al metodo de repo
                var result = await _portionRepository.UpdateAsync(id, dataPortion);

                return new ServiceResponse<Portion>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro actualizado con exito",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<Portion>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }
        }





        public async Task<ServiceResponse<Portion>> SetStateAsync(int portionId, bool state)
        {
            var response = new ServiceResponse<Portion>();

            var existingProduct = await _portionRepository.GetByIdAsync(portionId);
            if (existingProduct == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "El producto no existe";
                return response;
            }

 
            var repoResponse = await _portionRepository.SetStateAsync(portionId, state);

            if (repoResponse.Data == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "No se pudo actualizar el estado del producto";
                return response;
            }

            response.Data = repoResponse.Data;
            response.IsSuccess = true;
            response.MessageCode = MessageCodes.Sucess;
            response.Message = state ? "Producto activado" : "Producto desactivado";

            return response;
        }

        public async Task<ServiceResponse<Portion>> SetReconsumableAsync(int portionId, bool state)
        {
            var response = new ServiceResponse<Portion>();

            var existingProduct = await _portionRepository.GetByIdAsync(portionId);
            if (existingProduct == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "El producto no existe";
                return response;
            }


            var repoResponse = await _portionRepository.SetReconsumableAsync(portionId, state);

            if (repoResponse.Data == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "No se pudo actualizar la consumibilidad del producto";
                return response;
            }

            response.Data = repoResponse.Data;
            response.IsSuccess = true;
            response.MessageCode = MessageCodes.Sucess;
            response.Message = state ? "Producto Reconsumible" : "Producto No Reconsumible";

            return response;
        }


    }



}

