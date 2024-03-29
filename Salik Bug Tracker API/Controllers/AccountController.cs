﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Salik_Bug_Tracker_API.Data;
using Salik_Bug_Tracker_API.DTO;
using Salik_Bug_Tracker_API.Models;
using Salik_Bug_Tracker_API.Models.Helpers;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Salik_Bug_Tracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signinManager, RoleManager<IdentityRole> roleManager,
           TokenValidationParameters tokenValidationParameters1,ApplicationDbContext context, IConfiguration configuration,ILogger<AccountController> logger)
        {
            _userManager = usermanager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _tokenValidationParameters = tokenValidationParameters1;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register-user")]
        [AllowAnonymous]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                _logger.LogInformation("Received registration request for user: " + model.Email);
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in registration request for user: " + model.Email);
                    return BadRequest("Please, provide all the required fields");
                }

       
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    _logger.LogWarning("User already exists: " + model.Email);
                    return BadRequest($"User {model.Email} already exists");
                }

                ApplicationUser newUser = new ApplicationUser()
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    //Add user role
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Developer);
                    _logger.LogInformation("Successfully registered user: " + model.Email);
                    return Ok("User created");
                }
                _logger.LogWarning("Failed to register user: " + model.Email);
                return BadRequest("User could not be created");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in registration request for user: " + model.Email + ". Error: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }

        }


        [HttpPost("login-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginVM)
        {
            try
            {
                _logger.LogInformation("Received login request for user: " + loginVM.Email);
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in login request for user: " + loginVM.Email);
                    return BadRequest("Please, provide all required fields");
                }

                var userExists = await _userManager.FindByEmailAsync(loginVM.Email);
                if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginVM.Password))
                {
                    var tokenValue = await GenerateJWTTokenAsync(userExists, null);
                    _logger.LogInformation("Successfully logged in user: " + loginVM.Email);
                    return Ok(tokenValue);
                }
                _logger.LogWarning("Unauthorized login attempt for user: " + loginVM.Email);
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in login request for user: " + loginVM.Email + ". Error: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }
        }
        /// <summary>
        /// A method to referesh generated tokens after they are expired 
        /// </summary>
        /// <param name="tokenRequestVM"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequestVM)
        {
            try
            {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, provide all required fields");
            }

            var result = await VerifyAndGenerateTokenAsync(tokenRequestVM);
            return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }

        }

        private async Task<AuthResultDTO> VerifyAndGenerateTokenAsync([FromBody] TokenRequestDTO tokenRequestVM)
        {
        
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestVM.RefreshToken);
            var dbUser = await _userManager.FindByIdAsync(storedToken?.UserId);

            try
            {
                var tokenCheckResult = jwtTokenHandler.ValidateToken(tokenRequestVM.Token, _tokenValidationParameters, out var validatedToken);

                return await GenerateJWTTokenAsync(dbUser, storedToken);
            }
            catch (SecurityTokenExpiredException)
            {
                if (storedToken?.DateExpire >= DateTime.UtcNow)
                {
                    return await GenerateJWTTokenAsync(dbUser, storedToken);
                }
                else
                {
                    return await GenerateJWTTokenAsync(dbUser, null);
                }
            }
        }



        private async Task<AuthResultDTO> GenerateJWTTokenAsync(ApplicationUser user, RefreshToken? rToken)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                
            };

            //Add User Role Claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }


            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (rToken != null)
            {
                var rTokenResponse = new AuthResultDTO()
                {
                    Token = jwtToken,
                    RefreshToken = rToken.Token,
                    ExpiresAt = token.ValidTo
                };
                return rTokenResponse;
            }

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();


            var response = new AuthResultDTO()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = token.ValidTo
            };

            return response;

        }
    }
}
