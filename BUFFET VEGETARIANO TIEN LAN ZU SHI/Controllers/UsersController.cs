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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        //Endpoints
        [Authorize(Roles = "Administrador,Cajero")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var serviceResponse = await _userService.GetAllAsync(pagination);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var userDtoCollection = serviceResponse.Data.Data.Select(c => new UserDto
                {
                    Id = c.Id,
                    UserName=c.UserName,
                    Email=c.Email,
                    State = c.State,
                    
                });

                var apiResponse = new ApiResponse<IEnumerable<UserDto>>
                {
                    Data = userDtoCollection,
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

        [Authorize(Roles = "Administrador")]
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

            var serviceResponse = await _userService.GetByIdAsync(id);

            if (serviceResponse.IsSuccess)
            {
             
                var userDto = new UserDto
                {
                    Id = serviceResponse.Data!.Id,
                    UserName = serviceResponse.Data!.UserName,
                    //PasswordHash = serviceResponse.Data!.PasswordHash,
                    Email = serviceResponse.Data!.Email,
                    State = serviceResponse.Data!.State,
                    Roles = serviceResponse.Data!.Roles

                 



                };

                return Ok(userDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var unsuccessfulResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontro usuario asociada al Id proporcionado",
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

        [Authorize(Roles = "Administrador")]
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

            var ServiceResponse = await _userService.GetByNameAsync(name);

            if (ServiceResponse.IsSuccess)
            {
                var UserDto = new UserDto()
                {
                    Id = ServiceResponse.Data!.Id,
                    UserName = ServiceResponse.Data.UserName,
                   //PasswordHash = ServiceResponse.Data.PasswordHash,
                    Email = ServiceResponse.Data.Email,
                    State =ServiceResponse.Data.State,
                    Roles = ServiceResponse.Data.Roles,

                   


                };

                return Ok(UserDto);
            }

            switch (ServiceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "No se encontro usuario asociado";
                    unSuccessfulResponse.Details = new { Error = "No hay registro asociado al valor name proporcionado" };

                    ////
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = ServiceResponse.Message ?? "Ocurrio un error inesperado";

                    return StatusCode(500, unSuccessfulResponse);
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateUserDto userDto)
        {

            var serviceResponse = await _userService.CreateAsync(userDto);

            if (serviceResponse.IsSuccess)
            {

                var newUserDto = new UserDto
                {
                    Id = serviceResponse.Data!.Id,
                    UserName = serviceResponse.Data!.UserName,
                    //PasswordHash = serviceResponse.Data!.PasswordHash,
                    Email = serviceResponse.Data!.Email,
                    State =serviceResponse.Data!.State
                 
                };


                return CreatedAtAction(
                    nameof(GetById),
                    new { Id = newUserDto.Id },
                   newUserDto
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
                    unSuccessfulResponse.Details = new { info = "No se puede duplicar el nombre del usuario" };
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
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dataUser)
        {
      

            var serviceResponse = await _userService.UpdateAsync(id, dataUser);

            if (serviceResponse.IsSuccess)
            {
                
                var updatedUser = new UserDto
                {
                    Id = serviceResponse.Data!.Id,
                    UserName = serviceResponse.Data!.UserName,
                    //PasswordHash = serviceResponse.Data!.PasswordHash,
                    Email = serviceResponse.Data!.Email,
                    State = serviceResponse.Data!.State
                  
                };

                return Ok(updatedUser);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "No se encontró usuario con el Id proporcionado";
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
            var serviceResponse = await _userService.SetStateAsync(id, state);

            if (serviceResponse.IsSuccess)
            {
                return Ok(new
                {
                    Id = serviceResponse.Data!.Id,
                    FirstName = serviceResponse.Data.UserName,
                    //PasswordHash= serviceResponse.Data.PasswordHash,
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




        [Authorize(Roles = "Administrador")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignUserRoleAsync([FromBody] UserRoleDto userRole)
        {
            var serviceResponse = await _userService.AssignUserRoleAsync(userRole.UserId, userRole.RoleId);

            if (serviceResponse.IsSuccess)
            {
                var userRoleDto = new UserRoleDto
                {
                    UserId = serviceResponse.Data!.UserId,
                    RoleId = serviceResponse.Data.RoleId
                };

                
                return Ok(userRoleDto);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "No se encontró el usuario o rol";
                    unSuccessfulResponse.Details = new { info = "Revise el UserId y RoleId proporcionado" };
                    return NotFound(unSuccessfulResponse);

                case MessageCodes.Sucess:
                    unSuccessfulResponse.Code = "200";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "El rol ya está asignado a este usuario";
                    unSuccessfulResponse.Details = new { info = "El rol ya existe para este usuario" };
                    return Ok(unSuccessfulResponse);


                case MessageCodes.ErrorValidation:
                    unSuccessfulResponse.Code = "400";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "Los datos proporcionados no son válidos";
                    unSuccessfulResponse.Details = new { info = "Revisa los campos proporcionados" };
                    return BadRequest(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno de la aplicación" };
                    return BadRequest(unSuccessfulResponse);
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpGet("byemail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var unSuccessfulResponse = new UnsuccessfulResponseDto();


            if (email == null || email == "")
            {
                unSuccessfulResponse.Code = "400";
                unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                unSuccessfulResponse.Details = new { Error = "El email no puede ser nulo o vacío" };
                return BadRequest(unSuccessfulResponse);
            }


            var serviceResponse = await _userService.GetByEmailAsync(email);

            if (serviceResponse.IsSuccess)
            {
                var userDto = new UserDto
                {
                    Id = serviceResponse.Data!.Id,
                    UserName = serviceResponse.Data.UserName,
                    Email = serviceResponse.Data.Email,
                    State = serviceResponse.Data.State
                };

                return Ok(userDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "No se encontró usuario asociado";
                    unSuccessfulResponse.Details = new { Error = "No hay registro asociado al email proporcionado" };
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = "Error interno " };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }






    }


}


