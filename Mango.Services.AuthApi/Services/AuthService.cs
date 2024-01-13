using AutoMapper;
using Mango.Services.AuthApi.Data;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthApi.Services
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext _dbContext;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;
		
		public AuthService(AppDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper)
		{
			_dbContext = dbContext;
			_roleManager = roleManager;
			_userManager = userManager;
			_mapper = mapper;
		}
		public async Task<LoginReponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
		{
			ApplicationUser? user = await _userManager.FindByEmailAsync(loginRequestDTO.UserName);
			if (user != null)
			{
				bool checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
				if (checkPasswordResult)
				{
					UserDTO userDTO = new()
					{
						ID = user.Id,
						Email = user.Email,
						PhoneNumber = user.PhoneNumber,
						Name = user.Name
					};
					//create token
					LoginReponseDTO loginReponse = new LoginReponseDTO()
					{
						User = userDTO,
						Token = "ABC"
					};
				return loginReponse;
				}
            }
			return null;
        }

		public async Task<string> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
		{
			ApplicationUser user = new()
			{
				UserName = registrationRequestDTO.Email,
				Email = registrationRequestDTO.Email,
				NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
				Name = registrationRequestDTO.Name,
				PhoneNumber = registrationRequestDTO.PhoneNumber,
			};
			try
			{
				var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
				if(result.Succeeded)
				{
					var response = _dbContext.ApplicationUsers.First(x => x.UserName == registrationRequestDTO.Email);
					return "";
				}
				else
				{
					return result.Errors.FirstOrDefault().Description;
				}
			}
			catch(Exception ex)
			{
				return "Error Encountered";
			}
		}
	}
}
