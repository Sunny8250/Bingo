﻿using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Services.IService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthApi.Services
{
	public class JwtTokenGenerator : IJwtTokenGenerator
	{
		private readonly JwtOptions _jwtOptions;
		public JwtTokenGenerator(JwtOptions jwtOptions)
        {
			_jwtOptions = jwtOptions;
        }
        public string GenerateToken(ApplicationUser applicationUser)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.ClientSecretKey));
			var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
				new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
				new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName)
			};

			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Audience = _jwtOptions.Audience,
				Issuer = _jwtOptions.Issuer,
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(10),
				SigningCredentials = credential
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
