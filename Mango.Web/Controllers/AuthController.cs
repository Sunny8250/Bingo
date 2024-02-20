using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
	public class AuthController : Controller
	{
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var loginRequestDTO = new LoginRequestDTO();
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _authService.LoginAsync(loginRequestDTO);
                if (loginResult != null && loginResult.IsSuccess)
                {
                    var loginReponseDTO = JsonConvert.DeserializeObject<LoginReponseDTO>(Convert.ToString(loginResult.Result));
                    await SiginInUser(loginReponseDTO);
                    _tokenProvider.SetToken(loginReponseDTO.Token);
                    TempData["success"] = $"Welcome back {loginReponseDTO.User.Name}";
                    return RedirectToAction("Index", "Home");
                }
				else
				{
					TempData["error"] = loginResult.Message;
                    return View(loginRequestDTO);
				}
			}
            return View(loginRequestDTO);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var RoleList = new List<SelectListItem>()
            {
                new SelectListItem(){Text=SD.AdminRole, Value = SD.AdminRole},
                new SelectListItem(){Text=SD.CustomerRole, Value = SD.CustomerRole}
            };
            ViewBag.RoleList = RoleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            if(ModelState.IsValid)
            {
                var registerResult = await _authService.RegisterAsync(registrationRequestDTO);
                if (registerResult != null && registerResult.IsSuccess)
                {
                    if(!string.IsNullOrEmpty(registrationRequestDTO.Role))
                    {
                        var assignRoleResult = await _authService.AssignRoleAsync(registrationRequestDTO);
                        if(assignRoleResult != null && assignRoleResult.IsSuccess)
                        {
                            TempData["success"] = "Registration is successful! Login Now.";
                        }
                    }
                }
                else
                {
                    TempData["error"] = registerResult.Message;
                }
            }
            var RoleList = new List<SelectListItem>()
            {
                new SelectListItem(){Text=SD.AdminRole, Value = SD.AdminRole},
                new SelectListItem(){Text=SD.CustomerRole, Value = SD.CustomerRole}
            };
            ViewBag.RoleList = RoleList;
            if (TempData["error"] != null)
                return View(registrationRequestDTO);
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.clearToken();
            return RedirectToAction(nameof(Login));
        }

        private async Task SiginInUser(LoginReponseDTO loginReponseDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginReponseDTO.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
