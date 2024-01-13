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
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
		
		public AuthService(AppDbContext dbContext, RoleManager<IdentityRole> roleManager,
			UserManager<ApplicationUser> userManager, IMapper mapper,
			IJwtTokenGenerator jwtTokenGenerator)
		{
			_dbContext = dbContext;
			_roleManager = roleManager;
			_userManager = userManager;
			_mapper = mapper;
			_jwtTokenGenerator = jwtTokenGenerator;
		}

		public async Task<bool> AssignRoleAsync(string email, string roleName)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if(user != null)
			{
				if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
				{
					await _roleManager.CreateAsync(new IdentityRole(roleName));
				}
				await _userManager.AddToRoleAsync(user, roleName);
				return true;
			}
			return false;
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
					var jwtToken = _jwtTokenGenerator.GenerateToken(user);

					LoginReponseDTO loginReponse = new LoginReponseDTO()
					{
						User = userDTO,
						Token = jwtToken
					};
				return loginReponse;
				}
            }
			return new LoginReponseDTO() { User = null, Token = ""};
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
