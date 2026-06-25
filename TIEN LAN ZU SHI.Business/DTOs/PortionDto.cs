using System.Text.Json.Serialization;

namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class PortionDto
    {
        public required int Id { get; set; }
        public required int CategoryId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
        public string CategoryName { get; set; }
        public string PortionName { get; set; }
        public decimal PortionPrice { get; set; }
        public string Description { get; set; }

        public string PortionType {  get; set; }

        //omitir las propiedades de state y reconsumable
        public bool State { get; set; }
        public bool Reconsumable { get; set; }
    }
}
