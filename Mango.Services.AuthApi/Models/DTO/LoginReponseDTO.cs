namespace Mango.Services.AuthApi.Models.DTO
{
	public class LoginReponseDTO
	{
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
