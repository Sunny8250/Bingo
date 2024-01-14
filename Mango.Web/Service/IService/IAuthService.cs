using Mango.Web.Models.DTO;

namespace Mango.Web.Service.IService
{
	public interface IAuthService
	{
		Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
		Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
		Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO assignRoleDTO);
	}
}
