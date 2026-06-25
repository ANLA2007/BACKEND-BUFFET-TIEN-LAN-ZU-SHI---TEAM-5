using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using TIEN_LAN_ZU_SHI.Business.DTOs;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;
using TIEN_LAN_ZU_SHI.DataAccess.Repositories;

namespace TIEN_LAN_ZU_SHI.Business.Services
{
    public class CustomerService:ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        //Constructor del Servicio 
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

      
        public async Task<ServiceResponse<PagedResponse<IEnumerable<Customer>>>> GetAllAsync(PaginationParams pagination)
        {
            var result = await _customerRepository.GetAllAsync(pagination);

            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<PagedResponse<IEnumerable<Customer>>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Operacion exitosa"
                };
            }

            switch (result.OperationStatusCode)
            {
                case 5025:
                    return new ServiceResponse<PagedResponse<IEnumerable<Customer>>>
                    {
                        Data = null,
                        IsSuccess = true,
                        MessageCode = MessageCodes.NoData,
                        Message = "No se encontraron registros"
                    };
                default:
                    return new ServiceResponse<PagedResponse<IEnumerable<Customer>>>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.ErrorDataBase,
                        Message = "Ocurrio un error inesperado"
                    };
            }
        }


        public async Task<ServiceResponse<Customer>> GetByIdAsync(int id)
        {
            var repoResponse = await _customerRepository.GetByIdAsync(id);

            try
            {
                if (repoResponse.OperationStatusCode == 0)
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = repoResponse.Data,
                        IsSuccess = true,
                        MessageCode = MessageCodes.Sucess,
                        Message = repoResponse.Message ?? "Operacion exitosa"

                    };
                }

                switch (repoResponse.OperationStatusCode)
                {
                    case 5034:
                        return new ServiceResponse<Customer>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.NotFound,
                            Message = repoResponse.Message ?? "No se encontro registro asociado  al Id proporcionado"

                        };


                    default:
                        return new ServiceResponse<Customer>
                        {
                            Data = null,
                            IsSuccess = false,
                            MessageCode = MessageCodes.ErrorDataBase,
                            Message = repoResponse.Message ?? "Ocurrio un error inesperado"

                        };

                }
            }
            catch (Exception)
            {
                return new ServiceResponse<Customer>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = repoResponse.Message ?? "Ocurrio un error inesperado"

                };


            }
        }


        public async Task<ServiceResponse<Customer>> GetByNameAsync(string name)
        {
            var result = await _customerRepository.GetByNameAsync(name);
            if (result.OperationStatusCode == 0)
            {
                return new ServiceResponse<Customer>
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
                case 5040:
                    messageCode = MessageCodes.NotFound;
                    message = "No se encontro cliente con ese Nombre proporcionado";
                    break;

                default:
                    messageCode = MessageCodes.ErrorDataBase;
                    message = "Error en la base de datos al obtener el cliente.";
                    break;
            }

            // Retorno final para los casos de error o no encontrado ////
            return new ServiceResponse<Customer>
            {
                Data = null,
                IsSuccess = false,
                MessageCode = messageCode,
                Message = message
            };
        }


        public async Task<ServiceResponse<Customer>> CreateAsync(CreateCustomerDto newCustomer)
        {
            try
            {
                //validar si existe registre (categoria) con nombre similar al que se desea crear
                var existingCustomer = await _customerRepository.GetByNameAsync(newCustomer.FirstName);

                if (existingCustomer.Data!.Id != 0 && !existingCustomer.Data.FirstName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = null,
                        IsSuccess = false,///*//
                        MessageCode = MessageCodes.Conflict,
                        Message = "Existe un registro con el nombre proporcionado"

                    };

                }
                // Mapeo de clase  Create CategoryDto a clase CAtegory , para enviar al repo
                var customer = new Customer()
                {
                    FirstName = newCustomer.FirstName,
                    LastName = newCustomer.LastName,
                    Email = newCustomer.Email,
                  
                };

                //Llamando al metodo repo
                var result = await _customerRepository.AddAsync(customer);

                return new ServiceResponse<Customer>
                {

                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro creado con exito",
                };


            }
            catch (Exception)
            {
                return new ServiceResponse<Customer>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }

        }


        public async Task<ServiceResponse<Customer>> UpdateAsync(int id, UpdateCustomerDto customer)
        {
            try
            {
                
                var existingIdCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingIdCustomer.Data!.Id == 0 && existingIdCustomer.Data.FirstName.IsNullOrEmpty())
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = null,
                        IsSuccess = false,
                        MessageCode = MessageCodes.NotFound,
                        Message = "No existe un cliente asociado  al Id proporcionado"

                    };
                }

     
                var existingNameCustomer = await _customerRepository.GetByNameAsync(customer.FirstName);
                if (existingNameCustomer.Data!.FirstName != null && existingNameCustomer.Data.Id != id)
                {
                    return new ServiceResponse<Customer>
                    {
                        Data = null,
                        IsSuccess = false,

                        MessageCode = MessageCodes.Conflict,
                        Message = "ya existe un cliente con el nombre proporcionado. No se debe duplicar el nombre"
                    };
                }
               
                var dataCustomer = new Customer()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email =customer.Email,
                    State = customer.State,
                };

                //llamando al metodo de repo
                var result = await _customerRepository.UpdateAsync(id, dataCustomer);

                return new ServiceResponse<Customer>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    MessageCode = MessageCodes.Sucess,
                    Message = "Registro actualizado con exito",
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<Customer>
                {
                    Data = null,
                    IsSuccess = false,
                    MessageCode = MessageCodes.ErrorDataBase,
                    Message = "Ocurrio un error inesperado",
                };
            }
        }


        public async Task<ServiceResponse<Customer>> SetStateAsync(int customerId, bool state)
        {
            var response = new ServiceResponse<Customer>();

            var existingCustomer = await _customerRepository.GetByIdAsync(customerId);
            if (existingCustomer == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "El cliente no existe";
                return response;
            }

           
            var repoResponse = await _customerRepository.SetStateAsync(customerId, state);

            if (repoResponse.Data == null)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.MessageCode = MessageCodes.ErrorValidation;
                response.Message = "No se pudo actualizar el estado del cliente";
                return response;
            }

            
            response.Data = repoResponse.Data;
            response.IsSuccess = true;
            response.MessageCode = MessageCodes.Sucess;
            response.Message = state ? "Cliente activado" : "Cliente desactivado";

            return response;
        }

    }

}

