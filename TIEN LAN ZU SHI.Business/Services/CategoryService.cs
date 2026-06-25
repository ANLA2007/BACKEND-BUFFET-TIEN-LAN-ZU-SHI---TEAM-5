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
    public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        //Constructor del Servicio 
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

      
        public async Task<ServiceResponse<PagedResponse<IEnumerable<Category>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _categoryRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<Category>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 5024:
                    return new ServiceResponse<PagedResponse<IEnumerable<Category>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };
                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<Category>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrio un error inesperado"
                    };
            }
        }


        public async Task<ServiceResponse<Category>> GetByIdAsync(int id)
        {
            var repoResponse = await _categoryRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<Category>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = repoResponse.Message ?? "Operacion exitosa"

                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 5032:
                        return new ServiceResponse<Category>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontro registro asociado  al Id proporcionado"

                        };


                    default:
                        return new ServiceResponse<Category>
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
                return new ServiceResponse<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"

                };


            }
        }



        public async Task<ServiceResponse<Category>> GetByNameAsync(string name)
        {
            var result = await _categoryRepository.GetByNameAsync(name);
            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<Category>
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
                case 5038:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontro categoria con ese Nombre proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener la categoría.";
                    break;
            }

            // Retorno final para los casos de error o no encontrado //parte que falta del video //
            return new ServiceResponse<Category>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }


        public async Task<ServiceResponse<Category>> CreateAsync(CreateCategoryDto newCategory)
        {
            try
            {
                //validar si existe registre (categoria) con nombre similar al que se desea crear
                var existingCategory = await _categoryRepository.GetByNameAsync(newCategory.CategoryName);

                if (existingCategory.Data!.Id != 0 && !existingCategory.Data.CategoryName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Category>
                    {
                        Data = null,
                        IsSuccess = false, ///.//   Error validation///
                        MessageCode = MessageCodes.Conflict,
                        Message = "Existe un registro con el nombre proporcionado"

                    };

                }
                // Mapeo de clase  Create CategoryDto a clase CAtegory , para enviar al repo
                var category = new Category()
                {
                    CategoryName = newCategory.CategoryName,
                    Description = newCategory.Description,
                };

                //Llamando al metodo repo
                var result = await _categoryRepository.AddAsync(category);

                return new ServiceResponse<Category>
                {

                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro creado con exito",
                };


            }
            catch (Exception)
            {
                return new ServiceResponse<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }

        }




        public async Task<ServiceResponse<Category>> UpdateAsync(int id, UpdateCategoryDto category)
        {
            try
            {
                //validar que la categoria exista segun Id
                var existingIdCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingIdCategory.Data!.Id == 0 && existingIdCategory.Data.CategoryName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Category>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No existe una categoria asociada al Id proporcionado"

                    };
                }

                //validar que el nombre enviado para la categoria no coincida con otro nombre existente
                var existingNameCategory = await _categoryRepository.GetByNameAsync(category.CategoryName);
                if (existingNameCategory.Data!.CategoryName != null && existingNameCategory.Data.Id != id)
                {
                    return new ServiceResponse<Category>
                    {
                        Data = null,
                        IsSuccess = false,

                        //NOTA: Agregar un nuevo tipo de codigo al Enum MessageCodes 
                        //Este seria conflict

                        MessageCode = MessageCodes.Conflict,
                        Message = "ya existe una categoria con el nombre proporcionado. No se debe duplicar el nombre"
                    };
                }
                //mapeo de la clase updatecategoryDto, para enviar al repo
                var dataCategory = new Category()
                {
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    State = category.State,
                };

                //llamando al metodo de repo
                var result = await _categoryRepository.UpdateAsync(id, dataCategory);

                return new ServiceResponse<Category>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro actualizado con exito",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }
        }



        public async Task<ServiceResponse<Category>> SetStateAsync(int categoryId, bool state)
        {
            var response = new ServiceResponse<Category>();

            // Validar que la categoría exista
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "La categoría no existe";
                return response;
            }

            // Llamar al repositorio para actualizar el estado
            var repoResponse = await _categoryRepository.SetStateAsync(categoryId, state);

            if (repoResponse.Data == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "No se pudo actualizar el estado de la categoría";
                return response;
            }

            // Construir la respuesta exitosa
            response.Data = repoResponse.Data;
            response.IsSuccess = true;
            response.MessageCode = MessageCodes.Sucess;
            response.Message = state ? "Categoría activada" : "Categoría desactivada";

            return response;
        }

    }
}
