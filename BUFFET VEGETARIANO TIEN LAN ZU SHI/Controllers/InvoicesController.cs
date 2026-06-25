using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROYECTO_TIEN_LAN_ZU_SHI.DTOs;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace PROYECTO_TIEN_LAN_ZU_SHI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;



        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [Authorize(Roles = "Administrador,Cajero")]
        [HttpGet ("PrintQueue")]
        public async Task<IActionResult> PrintQueue()
        {
            var serviceResponse = await _invoiceService.InvoiceQueue();
            if (serviceResponse.IsSuccess)
            {
                var  response = new ApiResponse<List<Invoice>>
                {
                    Data = serviceResponse.Data,
                    Meta = new { TotalElements = serviceResponse.Data!.Count}
                };

                return Ok(response);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NoData:
                    unSuccessfulResponse.Code = "200";
                    unSuccessfulResponse.Message = "No se encontraron registros";
                    unSuccessfulResponse.Details = new { info = "No hay facturas que imprimir" };
                    return Ok( unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = "Error interno en la aplicacion" };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }


        [Authorize(Roles = "Administrador,Cajero")]
        [HttpGet("ToPrint")]
        public async Task<IActionResult> ToPrint()
        {
            var serviceResponse = await _invoiceService.ToPrint();
            if (serviceResponse.IsSuccess)
            {
                var response = new ApiResponse<Invoice>
                {
                    Data = serviceResponse.Data!,
                    Meta = new { message = serviceResponse.Message }
                };

                return Ok(response);
            }

            var unSuccessfulResponse = new UnsuccessfulResponseDto();

            switch (serviceResponse.MessageCode)
            {
                case MessageCodes.NoData:
                    unSuccessfulResponse.Code = "200";
                    unSuccessfulResponse.Message = "No se encontraron registros";
                    unSuccessfulResponse.Details = new { info = "No hay facturas que imprimir" };
                    return Ok(unSuccessfulResponse);

                default:
                    unSuccessfulResponse.Code = "500";
                    unSuccessfulResponse.Message = "Ocurrió un error inesperado";
                    unSuccessfulResponse.Details = new { info = "Error interno en la aplicacion" };
                    return StatusCode(500, unSuccessfulResponse);
            }
        }
    }
}
