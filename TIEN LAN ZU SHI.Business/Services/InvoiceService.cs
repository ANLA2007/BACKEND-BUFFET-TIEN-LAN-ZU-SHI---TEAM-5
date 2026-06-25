using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;

namespace TIEN_LAN_ZU_SHI.Business.Services
{
    public  class InvoiceService:IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        //Constructor del Servicio 
        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<ServiceResponse<List<Invoice>>> InvoiceQueue()
        {
            try
            {
                var repoResponse  =  await _invoiceRepository.GetInvoiceQueue();
                if(repoResponse.OperationStatusCode == 0)
                {
                    if(repoResponse.Data!.Count > 0)
                    {
                        return new ServiceResponse<List<Invoice>>
                        {
                            //CONVERSION DE LA COLA A UNA COLECCION TIPO LIST
                            Data = repoResponse.Data!.ToList(),
                            IsSuccess = true,
                            MessageCode = MessageCodes.Sucess,
                            Message = "Operacion exitosa"

                        };
                    }
                    else
                    {
                        return new ServiceResponse<List<Invoice>>
                        {
                            //CONVERSION DE LA COLA A UNA COLECCION TIPO LIST
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NoData,
                            Message = "No hay facturas que imprimir"

                        };
                    }
                }

                return new ServiceResponse<List<Invoice>>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Invoice>>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado"
                };
            }

        }



        public async Task<ServiceResponse<Invoice>> ToPrint()
        {
            try
            {
                //validar antes que la cola este llena
                var getInvoiceQueue = await _invoiceRepository.GetInvoiceQueue();
                if(getInvoiceQueue.Data!.Count < 1)
                {
                    return new ServiceResponse<Invoice>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NoData,
                        Message = "No hay facturas por imprimir"
                    };
                }

                var repoResponse = await _invoiceRepository.ToPrint();
                if(repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<Invoice>
                    {
                        Data = repoResponse.Data!,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message= "Factura impresa"
                    };

                }

                if (repoResponse.OperationStatusCode == 2)
                {
                    return new ServiceResponse<Invoice>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NoData,
                        Message = "No hay facturas por imprimir"
                    };

                }

                return new ServiceResponse<Invoice>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Invoice>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = ex.Message
                };
            }
        }





    }
}
