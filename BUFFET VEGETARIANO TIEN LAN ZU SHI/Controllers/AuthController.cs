using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PROYECTO_TIEN_LAN_ZU_SHI.DTOs;
using TIEN_LAN_ZU_SHI.Business.DTOs;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Business.Services;
using TIEN_LAN_ZU_SHI.Core.Common;

namespace PROYECTO_TIEN_LAN_ZU_SHI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
    

        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDto newUser)
        {
            var serviceResponse = await _authService.RegisterAsync(newUser);

            if (serviceResponse.IsSuccess)
            {
               
                var newUserDto = new UserDto
                {
                    Id = serviceResponse.Data!.Id,
                    UserName = serviceResponse.Data!.UserName,
                    Email = serviceResponse.Data!.Email,
                    State = serviceResponse.Data!.State,
                   
                };

                //return CreatedAtAction(
                //nameof(GetByUserName),
                //new { username = newUserDto.UserName },
                //newUserDto
                // );
                return CreatedAtAction(
                nameof(Login),       
                null,                
                newUserDto);


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
                    unSuccessfulResponse.Details = new { info = "El nombre del usuario o correo ya existe" };
                    return Conflict(unSuccessfulResponse);


                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno inesperado" };
                    return BadRequest(unSuccessfulResponse);
            }
        }

        //[HttpGet("byusername/{username}")]
        //public async Task<IActionResult> GetByUserName(string username)
        //{
        //    var unSuccessfulResponse = new UnsuccessfulResponseDto();
        //    if (username.IsNullOrEmpty())
        //    {
        //        unSuccessfulResponse.Code = "400";
        //        unSuccessfulResponse.Message = "El dato proporcionado no es valido";
        //        unSuccessfulResponse.Details = new { Error = "El name no puede ser nulo o vacio" };

        //        return BadRequest(unSuccessfulResponse);

        //    }

        //    var ServiceResponse = await _authService.GetByUserNameAsync(username);

        //    if (ServiceResponse.IsSuccess)
        //    {
        //        var UserDto = new UserDto()
        //        {
        //            Id = ServiceResponse.Data!.Id,
        //            UserName = ServiceResponse.Data.UserName,
        //            //PasswordHash = ServiceResponse.Data.PasswordHash,
        //            Email = ServiceResponse.Data.Email,
        //            State = ServiceResponse.Data.State,
                   
        //        };

        //        return Ok(UserDto);
        //    }

        //    switch (ServiceResponse.MessageCode)
        //    {
        //        case MessageCodes.NotFound:
        //            unSuccessfulResponse.Code = "404";
        //            unSuccessfulResponse.Message = ServiceResponse.Message ?? "No se encontro usuario asociado";
        //            unSuccessfulResponse.Details = new { Error = "No hay registro asociado al valor name proporcionado" };

        //            ////
        //            return NotFound(unSuccessfulResponse);

        //        default:
        //            unSuccessfulResponse.Code = "500";
        //            unSuccessfulResponse.Message = ServiceResponse.Message ?? "Ocurrio un error inesperado";

        //            return StatusCode(500, unSuccessfulResponse);
        //    }
        //}



        //[HttpGet("byemail/{email}")]
        //public async Task<IActionResult> GetByEmail(string email)
        //{
        //    var unSuccessfulResponse = new UnsuccessfulResponseDto();

          
        //    if (email == null || email == "")
        //    {
        //        unSuccessfulResponse.Code = "400";
        //        unSuccessfulResponse.Message = "El dato proporcionado no es válido";
        //        unSuccessfulResponse.Details = new { Error = "El email no puede ser nulo o vacío" };
        //        return BadRequest(unSuccessfulResponse);
        //    }

        
        //    var serviceResponse = await _authService.GetByEmailAsync(email);

        //    if (serviceResponse.IsSuccess)
        //    {
        //        var userDto = new UserDto
        //        {
        //            Id = serviceResponse.Data!.Id,
        //            UserName = serviceResponse.Data.UserName,
        //            Email = serviceResponse.Data.Email,
        //            State = serviceResponse.Data.State
        //        };

        //        return Ok(userDto);
        //    }

        //    switch (serviceResponse.MessageCode)
        //    {
        //        case MessageCodes.NotFound:
        //            unSuccessfulResponse.Code = "404";
        //            unSuccessfulResponse.Message = serviceResponse.Message ?? "No se encontró usuario asociado";
        //            unSuccessfulResponse.Details = new { Error = "No hay registro asociado al email proporcionado" };
        //            return NotFound(unSuccessfulResponse);

        //        default:
        //            unSuccessfulResponse.Code = "500";
        //            unSuccessfulResponse.Message = serviceResponse.Message ?? "Ocurrió un error inesperado";
        //            unSuccessfulResponse.Details = new { info = "Error interno " };
        //            return StatusCode(500, unSuccessfulResponse);
        //    }
        //}


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            //Confiamos las validaciones del Dto al Framework

            var serviceResponse =  await _authService.LoginAsync(loginRequest);

            if (serviceResponse.IsSuccess)
            {
                return Ok(serviceResponse.Data);
            }

            var unSuccessfulResponse =  new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.Unauthorized:
                    unSuccessfulResponse.Code = "401";
                    unSuccessfulResponse.Message = "Error de autenticacion de usuario";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };

                    
                    return  Unauthorized(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "Ocurrio un error inesperado";

                    return StatusCode(500, unSuccessfulResponse);
            }
        }












    }
}
