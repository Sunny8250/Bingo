using Mango.MessageBus;
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
		private readonly IMessageBus _messageBus;
		private readonly IConfiguration _configuration;
        public AuthApiController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
			_authService = authService;
			response = new ResponseDTO();
			_messageBus = messageBus;
			_configuration = configuration;
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
                var queueName = _configuration.GetValue<string>("TopicAndQueueName:EmailAuthQueue");
                await _messageBus.PublishMessage(registrationRequestDTO.Email, queueName);
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
				if(loginResponse.User == null)
				{
					response.IsSuccess = false;
					response.Message = "Username or Password is incorrect";
					return BadRequest(response);
				}
				response.Result = loginResponse;
			}
			return Ok(response);
		}

		[HttpPost]
		[Route("assignRole")]
		public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO assignRoleDTO)
		{
			if (ModelState.IsValid)
			{
				var roleAsssignSuccessfully = await _authService.AssignRoleAsync(assignRoleDTO.Email, assignRoleDTO.Role);
				if (roleAsssignSuccessfully == null)
				{
					response.IsSuccess = false;
					response.Message = "Error Encounterd";
					return BadRequest(response);
				}
				response.Result = roleAsssignSuccessfully;
			}
			return Ok(response);
		}
	}
}
