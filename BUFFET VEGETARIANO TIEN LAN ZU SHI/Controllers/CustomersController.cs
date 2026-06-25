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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

     
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        //Endpoints
        [Authorize(Roles = "Administrador, Cajero")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var serviceResponse = await _customerService.GetAllAsync(pagination);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var customerDtoCollection = serviceResponse.Data.Data.Select(c => new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    State = c.State,
                });

                var apiResponse = new ApiResponse<IEnumerable<CustomerDto>>
                {
                    Data = customerDtoCollection,
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

        [Authorize(Roles = "Administrador, Cajero")]
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

            var serviceResponse = await _customerService.GetByIdAsync(id);

            if (serviceResponse.IsSuccess)
            {
                
                var customerDto = new CustomerDto
                {
                    Id = serviceResponse.Data!.Id,
                    FirstName = serviceResponse.Data!.FirstName,
                    LastName = serviceResponse.Data!.LastName,
                    Email = serviceResponse.Data!.Email,
                    State = serviceResponse.Data!.State,
                };

                return Ok(customerDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var unsuccessfulResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontro el cliente asociado  al Id proporcionado",
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


        [Authorize(Roles = "Administrador,Cajero")]
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

            var ServiceResponse = await _customerService.GetByNameAsync(name);

            if (ServiceResponse.IsSuccess)
            {
                var CustomerDto = new CustomerDto()
                {
                    Id = ServiceResponse.Data!.Id,
                    FirstName = ServiceResponse.Data.FirstName,
                    LastName = ServiceResponse.Data.LastName,
                    Email = ServiceResponse.Data.Email,
                };

                return Ok(CustomerDto);
            }

            switch (ServiceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "No se encontro cliente asociado";
                    unSuccessfulResponse.Details = new { Error = "No hay registro asociado al valor name proporcionado" };

                  ////
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "Ocurrio un error inesperado";

                    return StatusCode(500, unSuccessfulResponse);
            }
        }

        [Authorize(Roles = "Cajero,Administrador")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateCustomerDto customerDto)
        {

            var serviceResponse = await _customerService.CreateAsync(customerDto);

            if (serviceResponse.IsSuccess)
            {

                var newCustomerDto = new CustomerDto
                {
                    Id = serviceResponse.Data!.Id,
                    FirstName = serviceResponse.Data!.FirstName,
                    LastName = serviceResponse.Data!.LastName,
                    Email = serviceResponse.Data!.Email,
                    State = serviceResponse.Data!.State,
                };


                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = newCustomerDto.Id },
                   newCustomerDto
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
                    unSuccessfulResponse.Details = new { info = "No se puede duplicar el nombre del cliente" };
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dataCustomer)
        {
            

            var serviceResponse = await _customerService.UpdateAsync(id, dataCustomer);

            if (serviceResponse.IsSuccess)
            {
              
                var updatedCustomer = new CustomerDto
                {
                    Id = serviceResponse.Data!.Id,
                    FirstName = serviceResponse.Data!.FirstName,
                    LastName = serviceResponse.Data!.LastName,
                    Email = serviceResponse.Data!.Email,
                    State = serviceResponse.Data!.State,
                   
                };
                
       
                return Ok(updatedCustomer);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "No se encontró cliente con el Id proporcionado";
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
           
            var serviceResponse = await _customerService.SetStateAsync(id, state);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                
                return Ok(new
                {
                    Id = serviceResponse.Data.Id,
                    FirstName = serviceResponse.Data.FirstName,
                    LastName = serviceResponse.Data.LastName,
                    Email = serviceResponse.Data.Email,
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

