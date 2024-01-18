using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
	public class AuthService : IAuthService
	{
		private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
			_baseService = baseService;
        }
        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO assignRoleDTO)
		{
			return await _baseService.SendAsync(new RequestDTO()
			{
				ApiType = SD.ApiType.POST,
				Url = SD.AuthApiBaseUrl + "/api/auth/assignRole",
				Data = assignRoleDTO
			}, withBearer: false);
		}

		public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
		{
			return await _baseService.SendAsync(new RequestDTO()
			{
				ApiType = SD.ApiType.POST,
				Url = SD.AuthApiBaseUrl + "/api/auth/login",
				Data = loginRequestDTO
			}, withBearer: false);
		}

		public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
		{
			return await _baseService.SendAsync(new RequestDTO()
			{
				ApiType = SD.ApiType.POST,
				Url = SD.AuthApiBaseUrl + "/api/auth/register",
				Data = registrationRequestDTO
			}, withBearer: false);
		}
	}
}
