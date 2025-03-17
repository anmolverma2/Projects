namespace FM.Services.CoupenAPI.Models
{
    public class ResponseModel
    {
        public object? Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string? Message { get; set; } = string.Empty;
    }
}
