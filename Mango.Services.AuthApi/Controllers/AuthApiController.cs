using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Services.IService;
using Mango.Services.CouponApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthApi.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthApiController : ControllerBase
	{
		private readonly IAuthService _authService;
		private ResponseDTO response;
        public AuthApiController(IAuthService authService)
        {
			_authService = authService;
			response = new ResponseDTO();
        }
        [HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
		{
			if(ModelState.IsValid)
			{
				var errorMessage = await _authService.RegisterAsync(registrationRequestDTO);
				if(!string.IsNullOrEmpty(errorMessage))
				{
					response.IsSuccess = false;
					response.Message = errorMessage;

					return BadRequest();
				}
			}
			return Ok(response);
			
			
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
		{
			if(ModelState.IsValid)
			{
				var loginResponse = await _authService.LoginAsync(loginRequestDTO);
				if(loginResponse == null)
				{
					response.IsSuccess = false;
					response.Message = "Login unsuccessful";
					return BadRequest(response);
				}
				response.Result = loginResponse;
			}
			return Ok(response);
		}
	}
}
