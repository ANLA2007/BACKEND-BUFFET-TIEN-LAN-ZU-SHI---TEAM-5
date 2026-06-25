namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class InventoryDto
    {
        public required int Id { get; set; }
        public required int PortionId { get; set; }
        public DateTime Date { get; set; }
        public int DailyStock { get; set; }
        public int AccumulatedStock { get; set; }
        public decimal SalePrice {  get; set; }

        ////Omitir la propiedad Justification es solo para los productos que se dan de baja
        //public string? Justification { get; set; }
    }
}
