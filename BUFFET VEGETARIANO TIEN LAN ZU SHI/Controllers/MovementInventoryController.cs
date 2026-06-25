using Microsoft.AspNetCore.Authorization;
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
    public class MovementInventoryController : ControllerBase
    {
        private readonly IMovementInventoryService _movementinventoryService;

        public MovementInventoryController(IMovementInventoryService movementinventoryService)
        {
            _movementinventoryService = movementinventoryService;
        }


        [Authorize(Roles = "Administrador,Cajero")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var serviceResponse = await _movementinventoryService.GetAllAsync(pagination);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var movementinventoryDtoCollection = serviceResponse.Data.Data.Select(c => new MovementInventoryDto
                {
                   Id = c.Id,
                   PortionId = c.PortionId,
                   MovementType = c.MovementType,
                   Quantity = c.Quantity,
                   PreviousDailyStock = c.PreviousDailyStock,
                   NewDailyStock = c.NewDailyStock,
                   PreviousAccumulatedStock = c.PreviousAccumulatedStock,
                   NewAccumulatedStock = c.NewAccumulatedStock,
                   Justification = c.Justification,
                   MovementDate = c.MovementDate,
                    
                });

                var apiResponse = new ApiResponse<IEnumerable<MovementInventoryDto>>
                {
                    Data = movementinventoryDtoCollection,
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

        [Authorize(Roles = "Administrador,Cajero")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateMovementInventoryDto newMovementInventory)
        {
            var serviceResponse =
                await _movementinventoryService.CreateAsync(newMovementInventory);

            if (serviceResponse.IsSuccess)
            {
                var movementDto = new MovementInventoryDto
                {
                    Id = serviceResponse.Data!.Id,
                    PortionId = serviceResponse.Data.PortionId,
                    MovementType = serviceResponse.Data.MovementType,
                    Quantity = serviceResponse.Data.Quantity,
                    PreviousDailyStock = serviceResponse.Data.PreviousDailyStock,
                    NewDailyStock = serviceResponse.Data.NewDailyStock,
                    PreviousAccumulatedStock = serviceResponse.Data.PreviousAccumulatedStock,
                    NewAccumulatedStock = serviceResponse.Data.NewAccumulatedStock,
                    Justification = serviceResponse.Data.Justification,
                    MovementDate = serviceResponse.Data.MovementDate
                };

                return StatusCode(201, movementDto);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = "Recurso no encontrado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };
                    return NotFound(unSuccessfulResponse);

                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El movimiento no pudo registrarse";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };
                    return Conflict(unSuccessfulResponse);

                case MessageCodes.ErrorValidation:
                    unSuccessfulResponse.Code = "400";
                    unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };
                    return BadRequest(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno en la aplicación" };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }


        [Authorize(Roles = "Administrador, Cajero")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {

            if (id <= 0)
            {
                var badRequestResponse = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }
                };
                return BadRequest(badRequestResponse);
            }


            var serviceResponse = await _movementinventoryService.GetPortionByIdAsync(id);


            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var inventoryDto = new MovementInventoryDto
                {
                    Id = serviceResponse.Data!.Id,
                    PortionId = serviceResponse.Data.PortionId,
                    MovementType = serviceResponse.Data.MovementType,
                    Quantity = serviceResponse.Data.Quantity,
                    PreviousDailyStock = serviceResponse.Data.PreviousDailyStock,
                    NewDailyStock = serviceResponse.Data.NewDailyStock,
                    PreviousAccumulatedStock = serviceResponse.Data.PreviousAccumulatedStock,
                    NewAccumulatedStock = serviceResponse.Data.NewAccumulatedStock,
                    Justification = serviceResponse.Data.Justification,
                    MovementDate = serviceResponse.Data.MovementDate
                };
                return Ok(inventoryDto);
            }


            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var notFoundResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontró movimiento asociado al Id proporcionado",
                        Details = new { info = serviceResponse.Message ?? "No se encontró el recurso solicitado" }
                    };
                    return NotFound(notFoundResponse);

                default:
                    var errorResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "500",
                        Message = "Ocurrió un error",
                        Details = new { info = serviceResponse.Message ?? "Error interno no esperado" }
                    };
                    return StatusCode(500, errorResponse);
            }
        }


        [Authorize(Roles = "Cajero,Administrador")]
        [HttpGet("bymovementtype/{movementType}")]
        public async Task<IActionResult> GetByMovementType(string movementType)
        {
            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            if (movementType.IsNullOrEmpty())
            {
                unSuccessfulResponse.Code = "400";
                unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                unSuccessfulResponse.Details = new
                {
                    Error = "El tipo de movimiento no puede ser nulo o vacío"
                };

                return BadRequest(unSuccessfulResponse);
            }

            var serviceResponse = await _movementinventoryService
                .GetByMovementTypeAsync(movementType);

            if (serviceResponse.IsSuccess)
            {
                var movementsDto = serviceResponse.Data!.Select(x => new MovementInventoryDto
                {
                    Id = x.Id,
                    PortionId = x.PortionId,
                    MovementType = x.MovementType,
                    Quantity = x.Quantity,
                    PreviousDailyStock = x.PreviousDailyStock,
                    NewDailyStock = x.NewDailyStock,
                    PreviousAccumulatedStock = x.PreviousAccumulatedStock,
                    NewAccumulatedStock = x.NewAccumulatedStock,
                    Justification = x.Justification,
                    MovementDate = x.MovementDate
                }).ToList();

                return Ok(movementsDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = serviceResponse.Message ??
                                                   "No se encontraron movimientos";
                    unSuccessfulResponse.Details = new
                    {
                        Error = "No existen movimientos para el tipo proporcionado"
                    };

                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = serviceResponse.Message ??
                                                   "Ocurrió un error inesperado";

                    return StatusCode(500, unSuccessfulResponse);
            }
        }
    }
}

