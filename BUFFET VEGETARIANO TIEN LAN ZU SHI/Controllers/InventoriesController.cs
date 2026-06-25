using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;


        public InventoriesController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        //Endpoints
        [Authorize(Roles = "Administrador, Cajero")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var serviceResponse = await _inventoryService.GetAllAsync(pagination);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var inventoryDtoCollection = serviceResponse.Data.Data.Select(c => new InventoryDto
                {
                    Id = c.Id,
                    PortionId = c.PortionId,
                    Date =c.Date,
                    DailyStock = c.DailyStock,
                    AccumulatedStock = c.AccumulatedStock,
                    SalePrice = c.SalePrice,
                });

                var apiResponse = new ApiResponse<IEnumerable<InventoryDto>>
                {
                    Data = inventoryDtoCollection,
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
        [HttpGet("{productId}/{date}")]
        public async Task<IActionResult> GetByProductAndDate(int productId, DateTime date)
        {
            var serviceResponse = await _inventoryService.GetByPortionAndDateAsync(productId, date);

            if (serviceResponse.IsSuccess)
            {
                var inventoryDto = new InventoryDto
                {
                    Id = serviceResponse.Data.Id,
                    PortionId = serviceResponse.Data.PortionId,
                    Date = serviceResponse.Data.Date,
                    DailyStock = serviceResponse.Data.DailyStock,
                    AccumulatedStock = serviceResponse.Data.AccumulatedStock,
                    SalePrice= serviceResponse.Data.SalePrice,
                };

                var apiResponse = new ApiResponse<InventoryDto>
                {
                    Data = inventoryDto,
                    Meta = new { message = serviceResponse.Message }
                };

                return Ok(apiResponse);
            }

            var unsuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unsuccessfulResponse.Code = "404";
                    unsuccessfulResponse.Message = "No se encontró inventario";
                    unsuccessfulResponse.Details = new { info = $"No hay inventario para el producto {productId} en la fecha {date.ToShortDateString()}" };
                    return NotFound(unsuccessfulResponse);

                default:
                    unsuccessfulResponse.Code = "500";
                    unsuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unsuccessfulResponse.Details = new { info = "Error interno en la aplicación" };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }

        [Authorize(Roles = "Administrador,Cajero")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateInventoryDto newInventory)
        {
            var serviceResponse = await _inventoryService.CreateAsync(newInventory);

            if (serviceResponse.IsSuccess)
            {
                var inventoryDto = new InventoryDto
                {
                    Id = serviceResponse.Data!.Id,
                    PortionId = serviceResponse.Data!.PortionId,
                    Date = serviceResponse.Data!.Date,
                    DailyStock = serviceResponse.Data!.DailyStock,
                    AccumulatedStock = serviceResponse.Data!.AccumulatedStock,
                    SalePrice = serviceResponse.Data!.SalePrice,
                };

             
                return CreatedAtAction(
                    nameof(GetByProductAndDate),
                    new
                    {
                        productId = inventoryDto.PortionId,
                        date = inventoryDto.Date.ToString("yyyy-MM-dd")
                    },
                    inventoryDto
                );
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.Conflict:
                    unSuccessfulResponse.Code = "409";
                    unSuccessfulResponse.Message = "El registro no pudo guardarse por un conflicto";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };
                    return Conflict(unSuccessfulResponse);

                case MessageCodes.ErrorValidation:
                    unSuccessfulResponse.Code = "400";
                    unSuccessfulResponse.Message = "El dato proporcionado no es válido";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Revise los campos enviados" };
                    return BadRequest(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message ?? "Error interno en la aplicacion" };
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

            
            var serviceResponse = await _inventoryService.GetPortionByIdAsync(id);

            
            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var inventoryDto = new InventoryDto
                {
                    Id = serviceResponse.Data.Id,
                    PortionId = serviceResponse.Data.PortionId,
                    DailyStock = serviceResponse.Data.DailyStock,
                    AccumulatedStock = serviceResponse.Data.AccumulatedStock,
                    SalePrice = serviceResponse.Data.SalePrice
                };
                return Ok(inventoryDto);
            }

           
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var notFoundResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontró inventario asociado al Id proporcionado",
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
    }
}

