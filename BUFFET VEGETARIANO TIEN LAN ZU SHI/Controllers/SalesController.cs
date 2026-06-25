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
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok();
        }

        [Authorize(Roles = "Cajero , Administrador")]
        [HttpPost]
        public async Task<IActionResult> Register(CreateSaleDto dto)
        {
            //Las validaciones de formato las hace el framework

            var serviceResponse = await _saleService.InsertAsync(dto);

            if (serviceResponse.IsSuccess)
            {

                var dataResponse = serviceResponse.Data!;

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = dataResponse.Id },
                   dataResponse
                    );
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.ErrorValidation:
                    unSuccessfulResponse.Code = "400";
                    unSuccessfulResponse.Message = "Ocurrio un error en la validacion de datos";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };
                    return BadRequest(unSuccessfulResponse);


                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrion un error inesperado";
                    unSuccessfulResponse.Details = new { info = serviceResponse.Message };

                    return StatusCode(500, unSuccessfulResponse);



            }
        }

        [Authorize(Roles = "Cajero,Administrador")]
        [HttpGet("bydaterange")]
        public async Task<IActionResult> GetSalesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            if (startDate == default || endDate == default || endDate < startDate)
            {
                unSuccessfulResponse.Code = "400";
                unSuccessfulResponse.Message = "Los parámetros proporcionados no son válidos";
                unSuccessfulResponse.Details = new { Error = "startDate y endDate deben ser válidos y endDate >= startDate" };
                return BadRequest(unSuccessfulResponse);
            }

            var serviceResponse = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);

            if (serviceResponse.IsSuccess)
            {
                var salesDtoList = serviceResponse.Data!.Select(sale => new SaleResponseDto
                {
                    Id = sale.Id,
                    CustomerId = sale.CustomerId,
                    DateTime = sale.DateTime,
                    Total = sale.Total,
                    Details = sale.Details.Select(d => new SaleResponseDetailDto
                    {
                        PortionId = d.PortionId,
                        Quantity = d.Quantity,
                        SalePrice = d.SalePrice,
                        LineTotal = d.LineTotal,
                        PortionType = d.PortionType,
                    }).ToList()
                }).ToList();

                return Ok(salesDtoList);
            }

           
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    unSuccessfulResponse.Code = "404";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "No se encontraron ventas en el rango proporcionado";
                    unSuccessfulResponse.Details = new { Info = "No hay registros asociados al rango de fechas" };
                    return NotFound(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = serviceResponse.Message ?? "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { Info = serviceResponse.Message };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }


        [Authorize(Roles = "Cajero , Administrador")]
        [HttpGet("{saleId}/details")]
        public async Task<IActionResult> GetSaleDetailsById(int saleId)
        {
            if (saleId <= 0)
            {
                var response = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id de venta proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }
                };

                return BadRequest(response);
            }

            var serviceResponse = await _saleService.GetBySaleIdAsync(saleId);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
               //mapeo detalles
                var saleDetailsDto = serviceResponse.Data.Details
                    .Select(sd => new SaleDetailDto
                    {
                        Id = sd.Id,
                        SaleId = sd.SaleId,
                        PortionId = sd.PortionId,
                        Quantity = sd.Quantity,
                        SalePrice = sd.SalePrice,
                        LineTotal = sd.LineTotal,
                        PortionType = sd.PortionType,
                    })
                    .ToList();

           //mapeo venta
                var saleMasterDto = new
                {
                    Id = serviceResponse.Data.Master.Id,
                    CustomerId = serviceResponse.Data.Master.CustomerId,
                    DateTime = serviceResponse.Data.Master.DateTime,
                    Total = serviceResponse.Data.Master.Total
                };

                return Ok(new { Sale = saleMasterDto, Details = saleDetailsDto });
            }

        
            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var notFoundResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontraron detalles para la venta con el id proporcionado",
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


        [Authorize(Roles = "Cajero , Administrador")]
        [HttpGet("GetAllSales")]
       
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
        {
            var serviceResponse = await _saleService.GetAllAsync(pagination);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var salesDtoCollection = serviceResponse.Data.Data.Select(sr => new SaleResponseDto
                {
                    Id = sr.Master.Id,
                    CustomerId = sr.Master.CustomerId,
                    DateTime = sr.Master.DateTime,
                    Total = sr.Master.Total,

                    Details = sr.Details.Select(d => new SaleResponseDetailDto
                    {
                        PortionId = d.PortionId,
                        Quantity = d.Quantity,
                        SalePrice = d.SalePrice,
                        LineTotal = d.LineTotal,
                        PortionType = d.PortionType,
                    }).ToList()
                });

                var apiResponse = new ApiResponse<IEnumerable<SaleResponseDto>>
                {
                    Data = salesDtoCollection,
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
                    unsuccessfulResponse.Details = new
                    {
                        info = "Temporalmente no hay registros de ventas en la BD"
                    };
                    return Ok(unsuccessfulResponse);

                default:
                    unsuccessfulResponse.Code = "500";
                    unsuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unsuccessfulResponse.Details = new
                    {
                        info = "Error interno en la aplicación"
                    };
                    return StatusCode(500, unsuccessfulResponse);
            }
        }


        [Authorize(Roles = "Cajero , Administrador")]
        [HttpGet("{saleId}/details-list")]
        public async Task<IActionResult> GetSaleDetailById(int saleId)
        {
            if (saleId <= 0)
            {
                var response = new UnsuccessfulResponseDto()
                {
                    Code = "400",
                    Message = "Id de venta proporcionado debe ser mayor a 0",
                    Details = new { info = "Error en el formato del valor enviado" }
                };

                return BadRequest(response);
            }

            var serviceResponse = await _saleService.GetDetailsBySaleIdAsync(saleId);

            if (serviceResponse.IsSuccess && serviceResponse.Data != null)
            {
                var saleDetailsDto = serviceResponse.Data
                    .Select(sd => new SaleDetailDto
                    {
                        Id = sd.Id,
                        SaleId = sd.SaleId,
                        PortionId = sd.PortionId,
                        Quantity = sd.Quantity,
                        SalePrice = sd.SalePrice,
                        LineTotal = sd.LineTotal,
                        PortionType = sd.PortionType,
                    })
                    .ToList();

                return Ok(saleDetailsDto);
            }

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NotFound:
                    var notFoundResponse = new UnsuccessfulResponseDto()
                    {
                        Code = "404",
                        Message = "No se encontraron detalles para la venta con el id proporcionado",
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

