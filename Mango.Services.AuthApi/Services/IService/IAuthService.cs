using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.CouponApi.Models.DTO;

namespace Mango.Services.AuthApi.Services.IService
{
	public interface IAuthService
	{
		Task<string> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
		Task<LoginReponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
		Task<bool> AssignRoleAsync(string email, string roleName);
	}
}
