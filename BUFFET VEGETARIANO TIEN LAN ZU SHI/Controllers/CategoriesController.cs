using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PROYECTO_TIEN_LAN_ZU_SHI.DTOs;
using TIEN_LAN_ZU_SHI.Business.DTOs;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Business.Services;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace PROYECTO_TIEN_LAN_ZU_SHI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        //Constructor del controlador
        //Inyeccion de dependencia del CategoryService
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //Endpoints
        [Authorize(Roles = "Cajero , Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var serviceResponse = await _categoryService.GetAllAsync(pagination);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var categoryDtoCollection = serviceResponse.Data.Data.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName=c.CategoryName,
                    Description=c.Description,
                    State =c.State,
                    
                  
                });

                var apiResponse = new ApiResponse<IEnumerable<CategoryDto>>
                {
                    Data = categoryDtoCollection,
                    Meta = new
                    {
                        totalRecords = serviceResponse.Data.TotalRecords,
                        totalPages = serviceResponse.Data.TotalPages,
                        pageNumber = serviceResponse.Data.PageNumber,
                        pageSize = serviceResponse.Data.PageSize,
                        message = serviceResponse.Message
                    }
                };
                return Ok(apiResponse);
            }

            var unsuccessfulResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NoData:
                    unsuccessfulResponse.Code = "200";
                    unsuccessfulResponse.Message = "No se encontraron registros";
                    unsuccessfulResponse.Details = new { info = "Temporalmente no hay registros en la BD" };
                    return Ok(unsuccessfulResponse);
                default:
                    unsuccessfulResponse.Code = "500";
                    unsuccessfulResponse.Message = "Ocurrio un error inesperado";
                    unsuccessfulResponse.Details = new { info = "Error interno en la aplicacion" };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }


        [Authorize(Roles = "Cajero , Administrador")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //validar que el id cumpla con el formato esperado
            if (id <= 0 || id == null)
            {
                var response = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }

                };

                return BadRequest(response);
            }

            var serviceResponse = await _categoryService.GetByIdAsync(id);

            if (serviceResponse.IsSuccess)
            {
                //Mapeo de la categoria recibida
                var categoryDto = new CategoryDto
                {
                    Id = serviceResponse.Data!.Id,
                    CategoryName = serviceResponse.Data!.CategoryName,
                    Description = serviceResponse.Data!.Description,
                };

                return Ok(categoryDto);
            }

            switch (serviceResponse.MessageCode)
            {
                //400// cambio
                case MessageCodes.NotFound:
                    var unsuccessfulResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontro una categoria asociada al Id proporcionado",
                        Details = new { info = serviceResponse.Message ?? "No se encontro el recurso solicitado" }
                    };

                    return NotFound(unsuccessfulResponse);

                default:
                    unsuccessfulResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "500",
                        Message = "Ocurrio un error",
                        Details = new { info = serviceResponse.Message ?? "Error interno no esperado" }
                    };

                    return StatusCode(500, unsuccessfulResponse);
            }
        }


        [Authorize(Roles = "Cajero , Administrador")]
        [HttpGet("byname/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var unSuccessfulResponse = new UnsuccessfulResponseDto();
            if (name.IsNullOrEmpty())
            {
                unSuccessfulResponse.Code = "400";
                unSuccessfulResponse.Message = "El dato proporcionado no es valido";
                unSuccessfulResponse.Details = new { Error = "El name no puede ser nulo o vacio" };

                return BadRequest(unSuccessfulResponse);

            }

            var ServiceResponse = await _categoryService.GetByNameAsync(name);

            if (ServiceResponse.IsSuccess)
            {
                var CategoryDto = new CategoryDto()
                {
                    Id = ServiceResponse.Data!.Id,
                    CategoryName = ServiceResponse.Data.CategoryName,
                    Description = ServiceResponse.Data.Description,
                    State = ServiceResponse.Data.State,
                };

                return Ok(CategoryDto);
            }

            switch (ServiceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "No se encontro categoria  asociada";
                    unSuccessfulResponse.Details = new { Error = "No hay registro asociado al valor name proporcionado" };

                    //no se miro bien en el video duda//
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "Ocurrio un error inesperado";

                    return StatusCode(500, unSuccessfulResponse);
            }
        }
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateCategoryDto categoryDto)
        {
            
            var serviceResponse = await _categoryService.CreateAsync(categoryDto);

            if (serviceResponse.IsSuccess)
            {
             
                var newCategoryDto = new CategoryDto
                {
                    Id = serviceResponse.Data!.Id,
                    CategoryName = serviceResponse.Data!.CategoryName,
                    Description = serviceResponse.Data!.Description,
                    State = serviceResponse.Data!.State,
                };

      
                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = newCategoryDto.Id },
                   newCategoryDto
                   );
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.ErrorValidation:
                    unSuccessfulResponse.Code = "400";
                    unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Revisa los campos enviados" };
                    return BadRequest(unSuccessfulResponse);

                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El registro no pudo guardarse por un conflicto";
                    unSuccessfulResponse.Details = new { info = "No se puede duplicar el nombre de la categoría" };
                    return Conflict(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrion un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno inesperado" };

                    return BadRequest(unSuccessfulResponse);



            }
        }



        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dataCategory)
        {
            // Nota: las validaciones de formato al campo CategoryName, Description e IsActive
            // del UpdateCategoryDto las realiza el framework considerando las DataAnnotations.

            // Se invoca al servicio confiando en que el framework ya validó el DTO
            var serviceResponse = await _categoryService.UpdateAsync(id, dataCategory);

            if (serviceResponse.IsSuccess)
            {
                // Mapeo de la categoría recibida a formato CategoryDto
                var updatedCategory = new CategoryDto
                {
                    Id = serviceResponse.Data!.Id,
                    CategoryName = serviceResponse.Data!.CategoryName,
                    Description = serviceResponse.Data!.Description,
                    State= serviceResponse.Data!.State,
                };

                // En este punto se enviará una respuesta exitosa de la solicitud (registro actualizado)
                return Ok(updatedCategory);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "No se encontró categoría con el Id proporcionado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Recurso no encontrado" };
                    return StatusCode(404, unSuccessfulResponse);

                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El registro no pudo guardarse por un conflicto";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Hubo conflicto en la actualización" };
                    return StatusCode(409, unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno inesperado" };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPatch("{id}/state")]
        public async Task<IActionResult> SetStateAsync(int id, [FromQuery] bool state)
        {
            // Llama al servicio que maneja la lógica de activación/desactivación
            var serviceResponse = await _categoryService.SetStateAsync(id, state);

            if (serviceResponse.IsSuccess)
            {
                // Devuelve la categoría actualizada con mensaje
                return Ok(new
                {
                    Id = serviceResponse.Data!.Id,
                    CategoryName = serviceResponse.Data.CategoryName,
                    State = serviceResponse.Data.State,
                    Message = serviceResponse.Message
                });
            }
           
           
            var unSuccessfulResponse = new UnsuccessfulResponseDto
            {
                Code = serviceResponse.MessageCode == MessageCodes.ErrorValidation ? "400" : "500",
                Message = serviceResponse.Message,
                Details = new { info = "Error al cambiar el estado" }
            };

            return BadRequest(unSuccessfulResponse);
        }







    }

}

