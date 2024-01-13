using Mango.Services.AuthApi.Models;

namespace Mango.Services.AuthApi.Services.IService
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(ApplicationUser applicationUser);
	}
}
