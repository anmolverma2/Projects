namespace FM.Services.AuthAPI.Models.DTO
{
    public class LoginResposeDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
