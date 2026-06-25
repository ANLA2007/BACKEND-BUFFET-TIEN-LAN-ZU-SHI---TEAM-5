namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class CategoryDto
    {
        public required int Id { get; set; }
        public required string CategoryName { get; set; }
        public string? Description { get; set; }

        ////omitir la propiedad state
        public bool State { get; set; }
    }
}
