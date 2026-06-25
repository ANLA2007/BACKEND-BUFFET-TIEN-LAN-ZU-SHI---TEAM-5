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
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IPortionRepository _PortionRepository;

        //Constructor del Servicio 
        public InventoryService(IInventoryRepository inventoryRepository, IPortionRepository portionRepository)
        {
            _inventoryRepository = inventoryRepository;
            _PortionRepository = portionRepository;
        }

     
        public async Task<ServiceResponse<PagedResponse<IEnumerable<Inventory>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _inventoryRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<Inventory>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 5027:
                    return new ServiceResponse<PagedResponse<IEnumerable<Inventory>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };
                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<Inventory>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrio un error inesperado"
                    };
            }
        }

        public async Task<ServiceResponse<Inventory>> CreateAsync(CreateInventoryDto newInventory)
        {
            try
            {
                //Validar la existencia del Producto en el catalogo
                var existentPortion = await _PortionRepository.GetByIdAsync(newInventory.PortionId);

                if (existentPortion.Data == null)
                {
                    return new ServiceResponse<Inventory>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.Conflict,
                        Message = "No existe porcion con el id proporcionado"
                    };
                }

                //Traer los datos más recientes ddel producto en el inventario
                var dataInventoryPortion = await _inventoryRepository.GetPortionByIdAsync(newInventory.PortionId);

                //Si el producto existe en el inventario capturar su AccumulatedStock
                var AccumulatedStock = 0;
                if (dataInventoryPortion.Data! == null)
                    AccumulatedStock = 0;
                else
                {
                    AccumulatedStock = dataInventoryPortion.Data!.AccumulatedStock;

                    //Validamos si ya tiene stock registrado hoy
                    var existentDate = await _inventoryRepository.GetByPortionAndDateAsync(
                        newInventory.PortionId,
                        newInventory.Date
                    );

                    if (existentDate.Data != null)
                    {
                        return new ServiceResponse<Inventory>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.Conflict,
                            Message = "Ya existe un registro de inventario para esta porcion en la fecha proporcionada"
                        };
                    }
                }
                    

                var inventory = new Inventory()
                {
                    PortionId = newInventory.PortionId,
                    Date = newInventory.Date,
                    DailyStock = newInventory.DailyStock,
                    SalePrice = newInventory.SalePrice,
                    AccumulatedStock = 0
                };

                //Sumamos el DaylyStock si el producto es Reconsumible
                if (existentPortion.Data!.Reconsumable == true)
                {
                    inventory.AccumulatedStock = AccumulatedStock + newInventory.DailyStock;

                    if (inventory.AccumulatedStock > 30)
                    {
                        return new ServiceResponse<Inventory>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.Conflict,
                            Message = $"El stock acumulado de la porcion con Id {newInventory.PortionId} superará el limite de stock acumulado (30)"
                        };
                    }
                }
                  
                else
                    inventory.AccumulatedStock = newInventory.DailyStock;

              
                //Invocamos al repo               
                var result = await _inventoryRepository.AddAsync(inventory);

                return new ServiceResponse<Inventory>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Inventario registrado"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Inventory>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrió un error inesperado al registrar el inventario: " + ex.Message
                };
            }
        }

        public async Task<ServiceResponse<Inventory>> GetByPortionAndDateAsync(int portionId, DateTime date)
        {
            try
            {

                var result = await _inventoryRepository.GetByPortionAndDateAsync(portionId, date);

                if (result.Data != null)
                {

                    return new ServiceResponse<Inventory>
                    {
                        Data = result.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = "Inventario encontrado"
                    };
                }
                else
                {

                    return new ServiceResponse<Inventory>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No se encontró inventario para esta porcion y fecha"
                    };
                }
            }
            catch (Exception ex)
            {

                return new ServiceResponse<Inventory>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrió un error al consultar el inventario: " + ex.Message
                };
            }
        }

        public async Task<ServiceResponse<Inventory>> GetPortionByIdAsync(int id)
        {
            var repoResponse = await _inventoryRepository.GetPortionByIdAsync(id);

            try
            {
                switch (repoResponse.OperationStatusCode)
                {
                    case 0: 
                        return new ServiceResponse<Inventory>
                        {
                            Data = repoResponse.Data,
                            IsSuccess = true,
                            MessageCode = MessageCodes.Sucess,
                            Message = repoResponse.Message ?? "Operación exitosa"
                        };

                    case -1: 
                        return new ServiceResponse<Inventory>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontró la porcion"
                        };

                    default: 
                        return new ServiceResponse<Inventory>
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
                return new ServiceResponse<Inventory>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrió un error inesperado"
                };
            }
        }

    }
}
