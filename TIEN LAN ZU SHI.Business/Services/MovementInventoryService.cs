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
    public class MovementInventoryService : IMovementInventoryService
    {
        private readonly IMovementInventoryRepository _movementInventoryRepository;
        private readonly IPortionRepository _portionRepository;

        public MovementInventoryService(
            IMovementInventoryRepository movementInventoryRepository,
            IPortionRepository portionRepository)
        {
            _movementInventoryRepository = movementInventoryRepository;
            _portionRepository = portionRepository;
        }

        public async Task<ServiceResponse<PagedResponse<IEnumerable<MovementInventory>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _movementInventoryRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<MovementInventory>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 7000:
                    return new ServiceResponse<PagedResponse<IEnumerable<MovementInventory>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };
                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<MovementInventory>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrio un error inesperado"
                    };
            }
        }


        public async Task<ServiceResponse<MovementInventory>> CreateAsync(
            CreateMovementInventoryDto newMovementInventory)
        {
            try
            {
                var existentPortion =
                    await _portionRepository.GetByIdAsync(newMovementInventory.PortionId);

                if (existentPortion.Data == null)
                {
                    return new ServiceResponse<MovementInventory>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No existe una porción con el Id proporcionado"
                    };
                }

               //// if (newMovementInventory.MovementType == "BAJA" &&
               //     string.IsNullOrWhiteSpace(newMovementInventory.Justification))
               // {
               //     return new ServiceResponse<MovementInventory>
               //     {
               //         Data = null,
               //         IsSuccess = false,
               //         MessageCode = MessageCodes.ErrorValidation,
               //         Message = "La justificación es obligatoria para una baja"
               //     };
               // }//

                var movement = new MovementInventory
                {
                    PortionId = newMovementInventory.PortionId,
                    MovementType = newMovementInventory.MovementType,
                    Quantity = newMovementInventory.Quantity,
                   // Justification = newMovementInventory.Justification
                };

                var result =
                    await _movementInventoryRepository.AddAsync(movement);

                switch (result.OperationStatusCode)
                {
                    case 0:
                        return new ServiceResponse<MovementInventory>
                        {
                            Data = result.Data,
                            IsSuccess = true,
                            MessageCode = MessageCodes.Sucess,
                            Message = "Movimiento registrado correctamente"
                        };

                    case 1:
                        return new ServiceResponse<MovementInventory>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = "La porción no existe en inventario"
                        };

                    case 2:
                        return new ServiceResponse<MovementInventory>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.Conflict,
                            Message = "No hay suficiente stock para realizar la baja"
                        };

                    default:
                        return new ServiceResponse<MovementInventory>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = "Ocurrió un error inesperado"
                        };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<MovementInventory>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrió un error inesperado: " + ex.Message
                };
            }
        }


        public async Task<ServiceResponse<MovementInventory>> GetPortionByIdAsync(int id)
        {
            var repoResponse = await _movementInventoryRepository.GetPortionByIdAsync(id);

            try
            {
                switch (repoResponse.OperationStatusCode)
                {
                    case 0:
                        return new ServiceResponse<MovementInventory>
                        {
                            Data = repoResponse.Data,
                            IsSuccess = true,
                            MessageCode = MessageCodes.Sucess,
                            Message = repoResponse.Message ?? "Operación exitosa"
                        };

                    case -1:
                        return new ServiceResponse<MovementInventory>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontró la porcion"
                        };

                    default:
                        return new ServiceResponse<MovementInventory>
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
                return new ServiceResponse<MovementInventory>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrió un error inesperado"
                };
            }
        }

        public async Task<ServiceResponse<List<MovementInventory>>> GetByMovementTypeAsync(string movementType)
        {
            var result = await _movementInventoryRepository.GetByMovementTypeAsync(movementType);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<List<MovementInventory>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            var messageCode = new MessageCodes();
            var message = string.Empty;

            switch (result.OperationStatusCode)
            {
                case 8050:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontraron movimientos con el tipo proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener los movimientos.";
                    break;
            }

            return new ServiceResponse<List<MovementInventory>>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }

    }
}

