namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class SaleDto
    {
        public required int Id { get; set; }
        public required int CustomerId { get; set; }
        public int UserId { get; set; }
        public DateTime DateTime { get; set; }
        public Decimal Total { get; set; }
    }
}
