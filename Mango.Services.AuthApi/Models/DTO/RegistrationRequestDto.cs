using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthApi.Models.DTO
{
    public class RegistrationRequestDTO
    {
		public string Email { get; set; }
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public string Password { get; set; }
	}
}
