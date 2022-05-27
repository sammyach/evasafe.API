using EmailManager;
using evasafe.API.Data;
using evasafe.API.dtos;
using evasafe.API.Helpers;
using evasafe.API.utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace evasafe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EVASAFEDBContext _context;
        private readonly IEmailSender _emailSender;
        private readonly AppSettings _appSettings;

        public AuthController(EVASAFEDBContext context, IEmailSender emailSender, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _emailSender = emailSender;
            _appSettings = appSettings.Value;
        }

        //[HttpPost]
        //[Route("login")]
        ////[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(string email)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var foundUser = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        //    if (foundUser == null) return BadRequest("Email not associated with any account.");
        //    return Ok(foundUser.Email);
        //}

        [HttpPatch]
        [Route("hash")]
        public async Task<IActionResult> HashString(string email)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var foundUser = await _context.EvAppUsers.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (foundUser == null) return BadRequest("Email not associated with any account.");
            if (string.IsNullOrWhiteSpace(foundUser.HashString))
            {
                foundUser.HashString = Utilities.GenerateHashString();
                _context.Update(foundUser);
                await _context.SaveChangesAsync();
            }            
            return Ok();             
        }

        [HttpPost]
        [Route("login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var foundUser = await _context.EvAppUsers.Where(x => x.Email == login.Email).FirstOrDefaultAsync();
            if (foundUser == null) return Unauthorized("Email not associated with any account.");
            var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(login.Password, foundUser?.Nacl, AppConstants.NUMBER_OF_ITERATIONS, AppConstants.KEY_ALGORITHM, AppConstants.KEY_LENGTH);
            
            if (!hashedPassword.SequenceEqual(foundUser.PasswordHash)) return Unauthorized("Wrong Password");
            else
            {
                foundUser.LastLoginDate = DateTime.UtcNow;
                _context.Update(foundUser);
                await _context.SaveChangesAsync();
                //login user
                //generate jwt
                //var token = await generateJwtToken(foundUser);
                return Ok(new AppUserDto(foundUser));
            }
            
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var newUser = new EvAppUser { Email = data.Email, Phone = data.Phone, FirstName = data.FirstName, LastName = data.LastName, Enabled = false };
            var nacl = Utilities.GenerateRandomNaCl();
            newUser.Nacl = nacl;
            newUser.HashString = Utilities.GenerateHashString();
            //hash password with generated nacl
            newUser.PasswordHash = Rfc2898DeriveBytes.Pbkdf2(data.Password, nacl, AppConstants.NUMBER_OF_ITERATIONS, AppConstants.KEY_ALGORITHM, AppConstants.KEY_LENGTH);

            newUser.DateCreated = DateTime.UtcNow;

            await _context.EvAppUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return Ok(new AppUserDto(newUser));
        }

        [HttpPatch]
        [Route("user/enable")]
        public async Task<IActionResult> EnableUser(EnableDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (data.Xcode != "qazplm") return BadRequest("Wrong xcode");
            try
            {
                var foundUser = await _context.EvAppUsers.Where(x => x.Email == data.Email).FirstOrDefaultAsync();
                if (foundUser == null) return BadRequest("Email not associated with any account.");
                foundUser.LastModifiedDate = DateTime.UtcNow;
                foundUser.Enabled = data.Enabled;
                _context.Update(foundUser);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);   
            }            
        }

        [HttpGet]
        [Route("validateemail")]
        public async Task<IActionResult> ValidateEmail(string email) 
        {
            //var ret = Utilities.IsValidEmail(email);
            byte[] bArr = RandomNumberGenerator.GetBytes(32);
            
            //var message = new Message(new string[] { email }, "Test email", "This is the content from our email. xoxo");
            //await _emailSender.SendEmailAsync(message);
            return Ok(bArr.Length + "-" + Convert.ToBase64String(bArr));
        }

        private async Task<string> generateJwtToken(EvAppUser user)
        {
            // generate token that is valid for 1 day
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var authClaims = new List<Claim> {                    
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("first_name", user?.FirstName),
                    new Claim("last_name", user?.LastName),
                    new Claim("last_login_date", user?.LastLoginDate.ToString())
                };
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
