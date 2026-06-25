namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }

        public object? Meta { get; set; }
    }
}
