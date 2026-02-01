using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IDataService _dataService;
        public AuthController(IAuthService authService, IDataService dataService)
        {
            _authService = authService;
            _dataService = dataService;
        }
        [HttpGet("CheckUsername/{uname}")]
        public async Task<IActionResult> CheckUsername([FromRoute] string uname)
        {
            var userExists = await _authService.CheckUsername(uname).ConfigureAwait(false);
            return Ok(userExists);
        }
        [HttpGet("CheckEmail/{email}")]
        public async Task<IActionResult> CheckEmail([FromRoute] string email)
        {
            var userExists = await _authService.CheckEmail(email).ConfigureAwait(false);
            return Ok(userExists);
        }

        // POST: Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            //check if username is null
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("Username is required");
            //check if password is null
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Password is required");
            var TokenVm = await _authService.Login(model).ConfigureAwait(false);
            if (TokenVm == null)
                return BadRequest("Invalid login attempt.<br/>Possible reasons:<br/>1. User not found, not approved, or not active.<br/>2. Password incorrect.<br/>3. Account lockedout.");
            return Ok(TokenVm);
        }
        // GET: Auth/PrivilegeLogin
        [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin")]
        [HttpGet("PrivilegeLogin/{Id}")]
        public async Task<IActionResult> PrivilegeLogin([FromRoute] string Id)
        {
            var TokenVm = await _authService.PrivilegeLogin(Id).ConfigureAwait(false);
            if (TokenVm == null)
                return BadRequest("Invalid login attempt.<br/>Possible reasons:<br/>1. User not found, not approved, or not active.<br/>2. Password incorrect.<br/>3. Account lockedout.");
            return Ok(TokenVm);
        }
        //[Authorize(Roles = "SuperAdmin")]
        //[HttpPost("Register")]
        //public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        //{
        //    //check if username is null
        //    if (string.IsNullOrEmpty(model.Username))
        //        return BadRequest("Username is required");
        //    //check if password is null
        //    if (string.IsNullOrEmpty(model.Password))
        //        return BadRequest("Password is required");
        //    if (string.IsNullOrEmpty(model.Role))
        //        model.Role = _config["DefaultUserRole"];
        //    var TokenVm = await _authService.Register(model).ConfigureAwait(false);
        //    if (TokenVm == null)
        //        return BadRequest("Registration failed");
        //    return Ok(TokenVm);
        //}
        [HttpGet("RefreshToken/{refreshtoken}")]
        public async Task<IActionResult> RefreshToken([FromRoute] string refreshtoken)
        {
            if (string.IsNullOrEmpty(refreshtoken))
                return BadRequest("Invalid Input");
            var TokenVm = await _authService.RefreshToken(refreshtoken).ConfigureAwait(false);
            if (TokenVm == null)
                return BadRequest("Token refresh failed");
            return Ok(TokenVm);
        }
        // POST: Auth/ForgotPassword
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string username)
        {
            //check if username is null
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is required");
            //generate password reset token
            var ForgotPasswordVm = await _authService.GeneratePasswordResetToken(username).ConfigureAwait(false);
            return Ok(ForgotPasswordVm);
        }
        // POST: Auth/ForgotUsername
        [HttpPost("ForgotUsername")]
        public async Task<IActionResult> ForgotUsername([FromBody] string email)
        {
            //check if email is null
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required");
            //get Username by Email
            var username = await _authService.GetUsernameByEmail(email).ConfigureAwait(false);
            return Ok(username);
        }
        // POST: Auth/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            //check if username is null
            if (string.IsNullOrEmpty(model.UserId))
                return BadRequest("User Id is required");
            var plain = EnDeCryptor.DecryptStringAES(model.EncPassword.Substring(8));
            model.EncPassword = EnDeCryptor.sha512hash(plain);
            var result = await _authService.ResetPasswordAsync(model.UserId, model.EncPassword).ConfigureAwait(false);
            if (result)
                return Ok("Your password has been reset");
            return BadRequest("Password couldn't be reset");
        }
        // POST: Auth/ChangePassword
        //[Authorize]
        //[HttpPost("ChangePassword")]
        //public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        //{
        //    //check if Username is null
        //    if (string.IsNullOrEmpty(model.Username))
        //        return BadRequest("Username is required");

        //    var tmodel = await _dataService.passhistory.GetByUsernamePwd(model.Username, model.NewPassword);

        //    //change the password
        //    var result = await _authService.ChangePassword(model).ConfigureAwait(false);
        //    if (result)
        //        return Ok("Your password has been changed");
        //    return BadRequest("Password couldn't be changed");
        //}


        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            //check if Username is null
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("Username is required");

            var plain = EnDeCryptor.DecryptStringAES(model.EncNewPassword.Substring(8));
            var tmodel = await _dataService.passhistory.GetByUsername(model.Username);

            if (tmodel != null)
            {
                if (tmodel.Pwd1 == plain || tmodel.Pwd2 == plain || tmodel.Pwd3 == plain)
                {
                    return Ok("Password can't be same as previous one");
                }
                else
                {
                    //change the password
                    var result = await _authService.ChangePassword(model).ConfigureAwait(false);
                    if (result)
                        return Ok("Your password has been changed");
                    return BadRequest("Password couldn't be changed");
                }
            }
            else
            {
                //change the password
                var result = await _authService.ChangePassword(model).ConfigureAwait(false);
                if (result)
                    return Ok("Your password has been changed");
                return BadRequest("Password couldn't be changed");
            }
        }

        // POST: Auth/GetPasswordResetToken
        [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin")]
        [HttpPost("GetPasswordResetToken")]
        public async Task<IActionResult> GetPasswordResetToken([FromBody] string userid)
        {
            //check if userid is null
            if (string.IsNullOrEmpty(userid))
                return BadRequest("User Id is required");
            //generate password reset token
            var resetToken = await _authService.GetPasswordResetToken(userid).ConfigureAwait(false);
            return Ok(resetToken);
        }
        // POST: Auth/GetPasswordResetTokenByEmail
        [Authorize(Roles = "SuperAdmin,TrainingAdmin,Admin,HRDivision")]
        [HttpPost("GetPasswordResetTokenByEmail")]
        public async Task<IActionResult> GetPasswordResetTokenByEmail([FromBody] string email)
        {
            //check if email is null
            if (string.IsNullOrEmpty(email))
                return BadRequest("email is required");
            //generate password reset token by email
            var emailDto = await _authService.GetPasswordResetTokenByEmail(email).ConfigureAwait(false);
            return Ok(emailDto);
        }
    }
}
