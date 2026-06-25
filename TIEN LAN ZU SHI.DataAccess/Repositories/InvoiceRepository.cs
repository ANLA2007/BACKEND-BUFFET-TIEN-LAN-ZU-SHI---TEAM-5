using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;

namespace TIEN_LAN_ZU_SHI.DataAccess.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        //DEFINIR LA INSTANCIA DE LA COLA CONCURRENTE 
        private readonly ConcurrentQueue<Invoice> _invoicesQueue = new ConcurrentQueue<Invoice>();


        private readonly string _connectionString;

        public InvoiceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        }

        public async Task<RepositoryResponse<ConcurrentQueue<Invoice>>> GetInvoiceQueue()
        {
            try
            {
                //VACIAR LA COLA
                _invoicesQueue.Clear();

                //Establecemos la conexion a la BD
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand("USP_GetUnPrintedInvoices", connection);
                    cmd.CommandType = CommandType.StoredProcedure;


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var invoice = new Invoice();

                            invoice.InvoiceId = (int)reader["InvoiceId"];
                            invoice.CreatedDate = (DateTime)reader["CreatedDate"];
                            invoice.SaleId = (int)reader["SaleId"];
                            invoice.Total = (decimal)reader["Total"];
                            invoice.IsPrinted = (bool)reader["IsPrinted"];

                            //AGREGAMOS UN ITEM "INVOICE" A LA COLA
                            _invoicesQueue.Enqueue(invoice);
                        }
                    }

                    return new RepositoryResponse<ConcurrentQueue<Invoice>>
                    {
                        Data = _invoicesQueue,
                        OperationStatusCode = 0,
                        Message = "Operacion exitosa"
                    };


                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<ConcurrentQueue<Invoice>>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<ConcurrentQueue<Invoice>>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }


        public async Task<RepositoryResponse<Invoice>> ToPrint()
        {
            var invoicePrinted = new Invoice();

            try
            {
               

                //IDENTIFICAR EL ITEM TOPE DE LA COLA

                if (_invoicesQueue.TryPeek(out Invoice? itemPeek))
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        SqlCommand cmd = new SqlCommand("USP_ToPrintInvoice", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue(@"InvoiceId",itemPeek.InvoiceId);
                        cmd.Parameters.AddWithValue(@"Date", DateTime.Now);


                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var invoice = new Invoice();

                                invoicePrinted.InvoiceId = (int)reader["InvoiceId"];
                                invoicePrinted.CreatedDate = (DateTime)reader["CreatedDate"];
                                invoicePrinted.SaleId = (int)reader["SaleId"];
                                invoicePrinted.Total = (decimal)reader["Total"];
                                invoicePrinted.IsPrinted = (bool)reader["IsPrinted"];

                                
                            }
                        }

                        return new RepositoryResponse<Invoice>
                        {
                            Data = invoicePrinted,
                            OperationStatusCode = 0,
                            Message = "Operacion exitosa"

                        };

                    }
                }
                else
                {
                    return new RepositoryResponse<Invoice>
                    {
                        Data = null,
                        OperationStatusCode = 2,
                        Message="No hay elemnetos en la cola"
                      

                    };
                }       
            }
            catch (SqlException ex) 
            {
                return new RepositoryResponse<Invoice>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
            catch(Exception ex)
            {
                return new RepositoryResponse<Invoice>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }

    }
}
        

    

