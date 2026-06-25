namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class SaleDetailDto
    {
        public required int Id { get; set; }
        public required int SaleId { get; set; }
        public int PortionId { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal LineTotal { get; set; }
        public string PortionType {  get; set; }

    }
}
