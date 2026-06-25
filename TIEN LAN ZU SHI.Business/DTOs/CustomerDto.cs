namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class CustomerDto
    {
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        ////omitir la propiedad state
        public bool State { get; set; }
    }
}
