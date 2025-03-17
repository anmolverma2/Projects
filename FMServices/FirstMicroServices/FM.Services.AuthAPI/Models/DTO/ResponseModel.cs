namespace FM.Services.AuthAPI.Models.DTO
{
    public class ResponseModel
    {
        public object? Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? Message { get; set; } = string.Empty;
    }
}
