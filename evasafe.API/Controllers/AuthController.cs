using EmailManager;
using evasafe.API.data;
using evasafe.API.dtos;
using evasafe.API.Helpers;
using evasafe.API.utils;
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
            try
            {
                var app_req = new EvUserAppRequest { User = login.User, SourceIp = HttpContext.Connection?.RemoteIpAddress?.ToString(), TimeStamp = DateTime.UtcNow, Path = HttpContext.Request.Path, Successful = false };
                HttpContext.Request.Headers.TryGetValue("LocalTime", out var localTime);
                app_req.ClientLocalTime = localTime;
                var remarks = "";
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var foundUser = await _context.EvAppUsers.Where(x => x.Email == login.User || x.Username == login.User && !x.Deleted).FirstOrDefaultAsync();
                if (foundUser == null)
                {
                    remarks = "Email/Username not associated with any account.";
                    app_req.Remarks = remarks;
                    _context.EvUserAppRequests.Add(app_req);
                    await _context.SaveChangesAsync();
                    return Unauthorized(remarks);
                }
                var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(login.Password, foundUser?.Nacl, AppConstants.NUMBER_OF_ITERATIONS, AppConstants.KEY_ALGORITHM, AppConstants.KEY_LENGTH);

                if (!hashedPassword.SequenceEqual(foundUser.PasswordHash))
                {
                    remarks = "Wrong Password";
                    app_req.Remarks = remarks;
                    _context.EvUserAppRequests.Add(app_req);
                    await _context.SaveChangesAsync();
                    return Unauthorized(remarks);
                }
                else if (!foundUser.Enabled)
                {
                    remarks = "Account not activated!";
                    app_req.Remarks = remarks;
                    _context.EvUserAppRequests.Add(app_req);
                    await _context.SaveChangesAsync();
                    return Unauthorized(remarks);
                }
                else
                {
                    var usr_sub = await _context.EvUserSubscriptions.Where(x => x.User == foundUser.Username).FirstOrDefaultAsync();
                    app_req.Successful = true;
                    app_req.Remarks = remarks;
                    _context.EvUserAppRequests.Add(app_req);
                    await _context.SaveChangesAsync();
                    return Ok(new AppUserDto(foundUser, usr_sub));
                    //foundUser.LastLoginDate = DateTime.UtcNow;
                    //_context.Update(foundUser);
                    //await _context.SaveChangesAsync();
                    //login user
                    //generate jwt
                    //var token = await generateJwtToken(foundUser);
                }
            }
            catch (Exception e)
            {
                return BadRequest($"EVERR - {e.Message} -> {e.InnerException}");
            }

            
        }

        [HttpPost]
        [Route("register/details")]
        public async Task<IActionResult> RegisterDetails(RegisterDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                //var foundUser = await _context.EvAppUsers.FindAsync(data.Username);
                var res = await _context.EvAppUsers.Where(x=>x.Username == data.Username).CountAsync();
                if (res > 0) {
                    //username already exists
                    //modify it
                    data.Username = data.Username + DateTime.Now.Minute + DateTime.Now.Second;
                }
                var newUser = new EvAppUser { Username = data.Username, Email = data.Email, Enabled = false, Phone = data.Phone, FirstName = data.FirstName, LastName = data.LastName, Organization = data.Organization, JobTitle = data.JobTitle, Deleted = false };
                var nacl = Utilities.GenerateRandomNaCl();
                newUser.Nacl = nacl;
                newUser.HashString = Utilities.GenerateHashString();
                //hash password with generated nacl
                newUser.PasswordHash = Rfc2898DeriveBytes.Pbkdf2(data.Password, nacl, AppConstants.NUMBER_OF_ITERATIONS, AppConstants.KEY_ALGORITHM, AppConstants.KEY_LENGTH);

                newUser.DateCreated = DateTime.UtcNow;

                await _context.EvAppUsers.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return Ok(new AppUserDto(newUser, null));
            }
            catch (Exception e)
            {
                return BadRequest($"EVERR - {e.Message} -> {e.InnerException}");
            }

            
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(LoginDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var newUser = new EvAppUser { Email = data.User, Enabled = false };
            var nacl = Utilities.GenerateRandomNaCl();
            newUser.Nacl = nacl;
            newUser.HashString = Utilities.GenerateHashString();
            //hash password with generated nacl
            newUser.PasswordHash = Rfc2898DeriveBytes.Pbkdf2(data.Password, nacl, AppConstants.NUMBER_OF_ITERATIONS, AppConstants.KEY_ALGORITHM, AppConstants.KEY_LENGTH);

            newUser.DateCreated = DateTime.UtcNow;

            await _context.EvAppUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return Ok(new AppUserDto(newUser, null));
        }

        [HttpPatch]
        [Route("user/enable")]
        public async Task<IActionResult> EnableUser(EnableDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (data.Xcode != "qazplm") return BadRequest("Wrong xcode");
            try
            {
                var foundUser = await _context.EvAppUsers.Where(x => x.Email == data.User || x.Username == data.User).FirstOrDefaultAsync();
                if (foundUser == null) return BadRequest("Email/Username not associated with any account.");
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

        [HttpPatch]
        [Route("user/delete")]
        public async Task<IActionResult> DeleteUser(LoginDto data)
        {
            var app_req = new EvUserAppRequest { User = data.User, SourceIp = HttpContext.Connection?.RemoteIpAddress?.ToString(), TimeStamp = DateTime.UtcNow, Path = HttpContext.Request.Path, Successful = false };
            HttpContext.Request.Headers.TryGetValue("LocalTime", out var localTime);
            app_req.ClientLocalTime = localTime;
            var remarks = "";
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var foundUser = await _context.EvAppUsers.Where(x => x.Email == data.User || x.Username == data.User).FirstOrDefaultAsync();
                if (foundUser == null)
                {
                    remarks = "Email / Username not associated with any account.";
                    app_req.Remarks = remarks;
                    _context.EvUserAppRequests.Add(app_req);
                    await _context.SaveChangesAsync();
                    return BadRequest(remarks);
                }

                var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(data.Password, foundUser?.Nacl, AppConstants.NUMBER_OF_ITERATIONS, AppConstants.KEY_ALGORITHM, AppConstants.KEY_LENGTH);

                if (!hashedPassword.SequenceEqual(foundUser.PasswordHash))
                {
                    remarks = "Wrong Password";
                    app_req.Remarks = remarks;
                    _context.EvUserAppRequests.Add(app_req);
                    await _context.SaveChangesAsync();
                    return Unauthorized(remarks);
                }

                foundUser.LastModifiedDate = DateTime.UtcNow;
                foundUser.Deleted = true;
                foundUser.DateDeleted = DateTime.UtcNow;
                _context.Update(foundUser);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPatch]
        [Route("user/edit")]
        public async Task<IActionResult> EditUser(UserDetailsDto data)
        {
            var app_req = new EvUserAppRequest { User = data.Username, SourceIp = HttpContext.Connection?.RemoteIpAddress?.ToString(), TimeStamp = DateTime.UtcNow, Path = HttpContext.Request.Path, Successful = false };
            HttpContext.Request.Headers.TryGetValue("LocalTime", out var localTime);
            app_req.ClientLocalTime = localTime;
            var remarks = "";
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //if (data.Xcode != "qazplm") return BadRequest("Wrong xcode");
            try
            {
                var foundUser = await _context.EvAppUsers.Where(x => x.Username == data.Username).FirstOrDefaultAsync();
                if (foundUser == null)
                {
                    remarks = "Username not associated with any account.";
                    app_req.Remarks = remarks;
                    _context.EvUserAppRequests.Add(app_req);
                    await _context.SaveChangesAsync();
                    return BadRequest();
                }
                foundUser.LastModifiedDate = DateTime.UtcNow;
                foundUser.Email = data.Email;
                foundUser.Phone = data.Phone;
                foundUser.FirstName = data.FirstName;
                foundUser.LastName = data.LastName;
                foundUser.Organization = data.Organization;
                foundUser.JobTitle = data.JobTitle;
                _context.Update(foundUser);

                app_req.Successful = true;
                _context.EvUserAppRequests.Add(app_req);
                await _context.SaveChangesAsync();
                var usr_sub = await _context.EvUserSubscriptions.Where(x => x.User == foundUser.Username).FirstOrDefaultAsync();
                return Ok(new AppUserDto(foundUser, usr_sub));
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

        [HttpPost]
        [Route("user/subscribe")]
        public async Task<IActionResult> Subscribe(SubscribeDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var foundUser = await _context.EvAppUsers.Where(x => x.Username == data.Username).FirstOrDefaultAsync();
                if (foundUser == null) return BadRequest();

                var sub = new EvUserSubscription { User = data.Username, SubscriptionType = data.SubscriptionType, DateCreated = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(data.NumberOfDays) };
                _context.EvUserSubscriptions.Add(sub);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
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
