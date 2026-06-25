using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Business.DTOs;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;
using TIEN_LAN_ZU_SHI.DataAccess.Repositories;

namespace TIEN_LAN_ZU_SHI.Business.Services
{
    public class SaleService : ISaleService
    {
        //Inyeccion de dependencias
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerService _customerService;
        private readonly IInventoryService _inventoryService;

        public SaleService(ISaleRepository saleRepository,
            ICustomerService customerService, IInventoryService inventoryService)
        {
            _saleRepository = saleRepository;
            _customerService = customerService;
            _inventoryService = inventoryService;
        }

        public async Task<ServiceResponse<SaleResponseDto>> InsertAsync(CreateSaleDto dto)
        {
            try
            {
                //validar que existe cliente con el id
                var existentCustomer = await _customerService.GetByIdAsync(dto.CustomerId);
                if (existentCustomer.Data! == null)
                {
                    return new ServiceResponse<SaleResponseDto>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorValidation,
                        Message = "No existe cliente con el id proporcionado"

                    };
                }

                
                foreach (var detail in dto.Details)
                {
                    //validar que cada producto del detalle exista en el inventario
                    var existentPortion = await _inventoryService.GetPortionByIdAsync(detail.PortionId);
                    if (existentPortion.Data == null)
                    {
                        return new ServiceResponse<SaleResponseDto>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorValidation,
                            Message = $"la porcion con id{detail.PortionId} no existe en el inventario"
                        };
                    }

                    //valida si hay suficientes unidades en el inventario para venta
                    if (existentPortion.Data!.DailyStock < detail.Quantity)
                    {
                        return new ServiceResponse<SaleResponseDto>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = $"No existen suficientes unidades en el inventario para la porcion con id {detail.PortionId}"
                        };
                    }
                    //para el stock acumulado //agregamos//
                    if (existentPortion.Data!.AccumulatedStock < detail.Quantity)
                    {
                        return new ServiceResponse<SaleResponseDto>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = $"No existen suficientes unidades en AccumulatedStock para la porcion con id {detail.PortionId}"
                        };
                    }

                }

                //Mapeo del Dto a la clase base del  Maestro de la venta
                var saleMaster = new Sale
                {
                    CustomerId = dto.CustomerId,
                    DateTime = DateTime.Now,
                    Total = 0
                };

                //Mapeo del Dto a la clase base del Detalle de la venta
                //Mapeo del Dto a la clase base del Detalle de la venta
                var saleDetails = dto.Details.Select(dt => new SaleDetail
                {
                    PortionId = dt.PortionId,
                    Quantity = dt.Quantity,
                    PortionType = dt.PortionType,
                }).ToList();


                //Invocamos al metodo del Repo para Insertar la Venta
                var repoResponse = await _saleRepository.InsertAsync(saleMaster,saleDetails);

                //validar la respuesta del repo
                if(repoResponse.OperationStatusCode == 0)
                {
                    //Mapeo de respuesta al Dto de respuesta
                    var dataResponse = new SaleResponseDto();

                    //Mapeo de la parte maestra
                    dataResponse.Id = repoResponse.Data!.Master.Id;
                    dataResponse.CustomerId = repoResponse.Data!.Master.CustomerId;
                    dataResponse.DateTime = repoResponse.Data!.Master.DateTime;   
                    dataResponse.Total = repoResponse.Data!.Master.Total;

                    //Mapeo del detalle
                    dataResponse.Details = repoResponse.Data!.Details.Select( dt => new SaleResponseDetailDto
                    {
                        PortionId= dt.PortionId,
                        Quantity = dt.Quantity,
                        SalePrice = dt.SalePrice,
                        LineTotal = dt.LineTotal,
                        PortionType=dt.PortionType,
                    }).ToList();

                    //retornamos la respuesta del servicio
                    return new ServiceResponse<SaleResponseDto>
                    {
                        Data = dataResponse,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = "venta registrada exitosamente"
                    };

                }
                else
                {
                    return new ServiceResponse<SaleResponseDto>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Hubo un error al registrar la venta"
                    };

                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SaleResponseDto>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado"
                };

            }
        }



        public async Task<ServiceResponse<List<SaleResponseDto>>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var repoResponse = await _saleRepository.GetSalesByDateRangeAsync(startDate, endDate);

                if (repoResponse.OperationStatusCode == -1 || repoResponse.Data == null)
                {
                    return new ServiceResponse<List<SaleResponseDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrió un error al obtener las ventas"
                    };
                }

                if (repoResponse.OperationStatusCode == 1 || !repoResponse.Data.Any())
                {
                    return new ServiceResponse<List<SaleResponseDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No se encontraron ventas en el rango proporcionado"
                    };
                }

                var salesDtoList = new List<SaleResponseDto>();

                foreach (var transaction in repoResponse.Data)
                {
                    var saleDto = new SaleResponseDto
                    {
                        Id = transaction.Master.Id,
                        CustomerId = transaction.Master.CustomerId,
                        DateTime = transaction.Master.DateTime,
                        Total = transaction.Master.Total,
                        Details = new List<SaleResponseDetailDto>()
                    };

                    foreach (var detail in transaction.Details)
                    {
                        saleDto.Details.Add(new SaleResponseDetailDto
                        {
                            PortionId = detail.PortionId,
                            Quantity = detail.Quantity,
                            SalePrice = detail.SalePrice,
                            LineTotal = detail.LineTotal,
                            PortionType = detail.PortionType,
                        });
                    }

                    salesDtoList.Add(saleDto);
                }

                return new ServiceResponse<List<SaleResponseDto>>
                {
                    Data = salesDtoList,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Ventas obtenidas exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SaleResponseDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = $"Ocurrió un error inesperado: {ex.Message}"
                };
            }
        }




        public async Task<ServiceResponse<SaleTransaction>> GetBySaleIdAsync(int saleId)
        {
            try
            {
                var repoResponse = await _saleRepository.GetBySaleIdAsync(saleId);

                if (repoResponse.OperationStatusCode == 0 && repoResponse.Data != null)
                {
                    return new ServiceResponse<SaleTransaction>
                    {
                        Data = repoResponse.Data, 
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = "Venta obtenida exitosamente."
                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 5070:
                        return new ServiceResponse<SaleTransaction>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontraron detalles para la venta proporcionada"
                        };

                    default:
                        return new ServiceResponse<SaleTransaction>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = repoResponse.Message ?? "Ocurrió un error inesperado"
                        };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SaleTransaction>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrió un error inesperado"
                };
            }
        }


        public async Task<ServiceResponse<PagedResponse<IEnumerable<SaleTransaction>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _saleRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<SaleTransaction>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Ventas obtenidas exitosamente."
                };
            }

            switch (result.OperationStatusCode)
            {
                case 5080:
                    return new ServiceResponse<PagedResponse<IEnumerable<SaleTransaction>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros de ventas."
                    };

                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<SaleTransaction>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrió un error inesperado."
                    };
            }
        }

        public async Task<ServiceResponse<List<SaleDetail>>> GetDetailsBySaleIdAsync(int saleId)
        {
            try
            {
                var repoResponse = await _saleRepository.GetDetailsBySaleIdAsync(saleId);

                if (repoResponse.OperationStatusCode == 0 && repoResponse.Data != null)
                {
                    return new ServiceResponse<List<SaleDetail>>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = "Detalles obtenidos exitosamente."
                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 5090:
                        return new ServiceResponse<List<SaleDetail>>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontraron detalles para la venta proporcionada"
                        };

                    default:
                        return new ServiceResponse<List<SaleDetail>>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = repoResponse.Message ?? "Ocurrió un error inesperado"
                        };
                }
            }
            catch (Exception)
            {
                return new ServiceResponse<List<SaleDetail>>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrió un error inesperado"
                };
            }
        }







    }

}

