using Application.AppSettings;
using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;

using Application.ViewModels;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtOptions;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDataService _dataService;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtOptions,
            RoleManager<ApplicationRole> roleManager, IDataService dataService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _roleManager = roleManager;
            _jwtOptions = jwtOptions.Value;
            _dataService = dataService;
        }
        public async Task<bool> CheckUsername(string uname)
        {
            var userFrmDb = await _userManager.FindByNameAsync(uname).ConfigureAwait(false);
            return userFrmDb != null;
        }
        public async Task<bool> CheckEmail(string email)
        {
            var userFrmDb = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            return userFrmDb != null;
        }

        //New Function for Hashing
        public int RandomNumber(int MaxNumber, int MinNumber = 0)
        {

            // initialize random number generator
            Random r = new Random(System.DateTime.Now.Millisecond);

            // if passed incorrect arguments, swap them
            // can also throw exception or return 0

            if (MinNumber > MaxNumber)
            {
                int t = MinNumber;
                MinNumber = MaxNumber;
                MaxNumber = t;
            }
            return r.Next(MinNumber, MaxNumber);
        }

        public async Task<TokenVM> Login(LoginDTO model)
        {
            //Decrypt the UserName and Password
            model.Username = EnDeCryptor.DecryptStringAES(model.EncUsername);
            //model.Password = EnDeCryptor.DecryptStringAES(model.EncPassword.Substring(8));


            //New Code By Ankit

            //model.Username = model.EncUsername;
            model.Password = model.EncPassword;

            // find user with this username
            var User = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
                if (User?.Approved != true || User.IsActive == false) return null;

            if (User.LoginValidTill != null && User.LoginValidTill.Value.Date < DateTime.Now.Date && (User.UserName.Contains("ForeignBidder") || User.UserName.Contains("GeneralUpload") || User.UserName.Contains("CommercialExecutive")))
            {
                return null;
            }

            var AccountLockedOut = await _userManager.IsLockedOutAsync(User).ConfigureAwait(false);
            if (AccountLockedOut) return null;

            //verify the password
            //var pwdVerified = await _userManager.CheckPasswordAsync(User, model.Password).ConfigureAwait(false);
            //if (!pwdVerified)
            //{
            //    await _userManager.AccessFailedAsync(User).ConfigureAwait(false);
            //    return null;
            //}

            if (User.Pwd_SHA512 != model.Password)
            {
                await _userManager.AccessFailedAsync(User).ConfigureAwait(false);
                return null;
            }

            //get role for this user
            var rolename = ((List<string>)await _userManager.GetRolesAsync(User).ConfigureAwait(false)).First();
            var Role = await _roleManager.FindByNameAsync(rolename).ConfigureAwait(false);
            var TokenVm = await GetTokenAsync(User, Role).ConfigureAwait(false);
            return TokenVm;
        }
        public async Task<TokenVM> PrivilegeLogin(string Id)
        {
            // find user with this user id
            var User = await _userManager.FindByIdAsync(Id).ConfigureAwait(false);
            if (User?.Approved != true || User.IsActive == false) return null;
            var AccountLockedOut = await _userManager.IsLockedOutAsync(User).ConfigureAwait(false);
            if (AccountLockedOut) return null;
            //get role for this user
            var rolename = ((List<string>)await _userManager.GetRolesAsync(User).ConfigureAwait(false)).First();
            var Role = await _roleManager.FindByNameAsync(rolename).ConfigureAwait(false);
            var TokenVm = await GetTokenAsync(User, Role).ConfigureAwait(false);
            return TokenVm;
        }
        //public async Task<TokenVM> Register(RegisterDTO model)
        //{
        //    //check if user already exists
        //    var userFrmDb = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
        //    if (userFrmDb != null)
        //        return null;
        //    //create new user and add to AspNetUsers
        //    var user = new ApplicationUser
        //    {
        //        UserName = model.Username,
        //        Name = model.Name,
        //        Email = model.Email,
        //        EmailConfirmed = true,
        //        PhoneNumber = model.PhoneNumber,
        //        PhoneNumberConfirmed = true,
        //        ProfileImage = model.ProfileImage,
        //        Approved = model.Approved,
        //        IsActive = model.IsActive,
        //        ChangePassword = model.ChangePassword,
        //        EncSecret = model.EncSecret
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
        //    if (!result.Succeeded) return null;
        //    //add user to Anonymous role
        //    model.Role = string.IsNullOrEmpty(model.Role) ? _config["DefaultUserRole"] : model.Role;
        //    await _userManager.AddToRoleAsync(user, model.Role).ConfigureAwait(false);
        //    var Role = await _roleManager.FindByNameAsync(model.Role).ConfigureAwait(false);
        //    var TokenVm = await GetTokenAsync(user, Role, null).ConfigureAwait(false);
        //    return TokenVm;
        //}
        public async Task<TokenVM> RefreshToken(string refreshtoken)
        {
            var refreshTokenDb = await _unitOfWork.RefreshTokensRepo.GetRefreshToken(refreshtoken).ConfigureAwait(false);
            if (refreshTokenDb == null || refreshTokenDb.ExpiresUtc < DateTime.UtcNow)
                return null;
            var User = await _userManager.FindByIdAsync(refreshTokenDb.UserId).ConfigureAwait(false);
            if (User?.Approved != true || User.IsActive == false || !await _signInManager.CanSignInAsync(User).ConfigureAwait(false))
                return null;
            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(User).ConfigureAwait(false))
                return null;
            //get role for this user
            var rolename = ((List<string>)await _userManager.GetRolesAsync(User).ConfigureAwait(false)).First();
            var Role = await _roleManager.FindByNameAsync(rolename).ConfigureAwait(false);
            //generate encoded access token
            var encodedToken = await _jwtService.GenerateEncodedToken(User, Role).ConfigureAwait(false);
            //generate TokenVM
            var TokenVm = new TokenVM
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(encodedToken),
                RefreshToken = refreshTokenDb.Token
            };
            return TokenVm;
        }
        public async Task<ForgotPasswordVM> GeneratePasswordResetToken(string username)
        {
            // find user with this username
            var user = await _userManager.FindByNameAsync(username).ConfigureAwait(false);
            if (user?.Approved != true || user.IsActive == false) return null;
            //Get the code
            var tokenGenerated = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var tokenGeneratedBytes = Encoding.UTF8.GetBytes(tokenGenerated);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
            var ForgotPasswordVm = new ForgotPasswordVM
            {
                Id = user.EmailConfirmed ? user.Id : null,
                Email = user.EmailConfirmed ? user.Email : null,
                Code = user.EmailConfirmed ? codeEncoded : null
            };
            return ForgotPasswordVm;
        }
        public async Task<string> GetPasswordResetToken(string userid)
        {
            // find user with this username
            var user = await _userManager.FindByIdAsync(userid).ConfigureAwait(false);
            var tokenGenerated = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var tokenGeneratedBytes = Encoding.UTF8.GetBytes(tokenGenerated);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
            return codeEncoded;
        }
        public async Task<EmailDTO> GetPasswordResetTokenByEmail(string email)
        {
            // find user with this email
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user?.Approved != true || user.IsActive == false) return null;
            //get the code
            var tokenGenerated = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var tokenGeneratedBytes = Encoding.UTF8.GetBytes(tokenGenerated);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
            var emailDto = new EmailDTO
            {
                Id = user.Id,
                Email = email,
                Code = codeEncoded
            };
            return emailDto;
        }
        public async Task<string> GetUsernameByEmail(string email)
        {
            // find user with this email
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            return user?.Approved != true || user.IsActive == false ? string.Empty : user.UserName;
        }

        public async Task<List<ApplicationUser>> GetUsersByUserNames(List<string> UserNames)
        {
            var Users = new List<ApplicationUser>(UserNames.Count);
            foreach (var UserName in UserNames)
            {
                var user = await _userManager.FindByNameAsync(UserName);
                if (user == null) continue;
                Users.Add(user);
            }
            return Users;
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO model)
        {
            // find user with this userid
            var user = await _userManager.FindByIdAsync(model.UserId).ConfigureAwait(false);
            if (user?.Approved != true || user.IsActive == false) return false;
            if (string.IsNullOrEmpty(model.Code)) return false;
            user.EncSecret = model.EncSecret;
            model.Password = EnDeCryptor.DecryptStringAES(model.EncPassword.Substring(8));
            //reset the password
            var codeDecodedBytes = WebEncoders.Base64UrlDecode(model.Code);
            var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
            var result = await _userManager.ResetPasswordAsync(user, codeDecoded, model.Password).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<bool> ResetPasswordAsync(string username, string newPassword)
        {
            // Mark the entity as modified and save changes
            var result = await _unitOfWork.AuthRepo.ResetPassword(username, newPassword);

            return true;
        }

        //public async Task<bool> ChangePassword(ChangePasswordDTO model)
        //{
        //    model.Password = EnDeCryptor.DecryptStringAES(model.EncPassword.Substring(8));
        //    model.NewPassword = EnDeCryptor.DecryptStringAES(model.EncNewPassword.Substring(8));
        //    model.ConfirmPassword = EnDeCryptor.DecryptStringAES(model.EncConfirmPassword.Substring(8));

        //    var tmodel = await _dataService.passhistory.GetByUsernamePwd(model.Username, model.NewPassword);

        //    if (tmodel != null)
        //    {
        //        //New By Ankit 18.05.2023 - For Maintaining Last 3 Passwords
        //        var tempmodel = new PasswordHistoryVM();
        //        tempmodel.Pwd1 = model.NewPassword;
        //        tempmodel.Username = model.Username;
        //        tempmodel.Id = 0;
        //        tempmodel.LastUpdated = DateTime.Now;
        //        await _dataService.passhistory.CreateOrUpdate(tempmodel).ConfigureAwait(false);


        //        // find user with this username
        //        var user = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
        //        if (user?.Approved != true || user.IsActive == false) return false;
        //        //change the password

        //        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword).ConfigureAwait(false);
        //        //set password status in user
        //        user.ChangePassword = false;
        //        user.EncSecret = model.EncSecret;
        //        //user.Password256Hash = EnDeCryptor.sha256_hash(model.NewPassword);
        //        var userPasswordStatusResult = await _userManager.UpdateAsync(user);
        //        //return true only if both are success
        //        return changePasswordResult.Succeeded && userPasswordStatusResult.Succeeded;
        //    }
        //    else
        //    {
        //        return changePasswordResult.Succeeded && userPasswordStatusResult.Succeeded;
        //    }
        //}

        public async Task<bool> ChangePassword(ChangePasswordDTO model)
        {
            model.Password = EnDeCryptor.DecryptStringAES(model.EncPassword.Substring(8));
            model.NewPassword = EnDeCryptor.DecryptStringAES(model.EncNewPassword.Substring(8));
            model.ConfirmPassword = EnDeCryptor.DecryptStringAES(model.EncConfirmPassword.Substring(8));

            //New By Ankit 18.05.2023 - For Maintaining Last 3 Passwords
            var tempmodel = new PasswordHistoryVM();
            tempmodel.Pwd1 = model.NewPassword;
            tempmodel.Username = model.Username;
            tempmodel.Id = 0;
            tempmodel.LastUpdated = DateTime.Now;
            await _dataService.passhistory.CreateOrUpdate(tempmodel).ConfigureAwait(false);


            // find user with this username
            var user = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
            if (user?.Approved != true || user.IsActive == false) return false;
            //change the password

            //var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword).ConfigureAwait(false);
            //set password status in user
            user.ChangePassword = false;
            user.EncSecret = model.EncSecret;
            user.Pwd_SHA512 = EnDeCryptor.sha512hash(model.NewPassword);
            //user.Password256Hash = EnDeCryptor.sha256_hash(model.NewPassword);
            var userPasswordStatusResult = await _userManager.UpdateAsync(user);
            //return true only if both are success
            return userPasswordStatusResult.Succeeded;
        }

        private async Task<TokenVM> GetTokenAsync(ApplicationUser User, ApplicationRole Role)
        {
            //try get refresh token of this user from the database
            var refreshTokenDb = await _unitOfWork.RefreshTokensRepo.GetRefreshTokenByUserId(User.Id).ConfigureAwait(false);
            if (refreshTokenDb != null)
            {
                _unitOfWork.RefreshTokensRepo.Delete(refreshTokenDb);
                //await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = User.Id,
                Token = _jwtService.GenerateRefreshToken(),
                IssuedUtc = _jwtOptions.IssuedAt,
                ExpiresUtc = _jwtOptions.RefreshTokenExpiresUtc
            };
            _unitOfWork.RefreshTokensRepo.Create(newRefreshToken);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //generate encoded access token
            var encodedToken = await _jwtService.GenerateEncodedToken(User, Role).ConfigureAwait(false);
            //generate TokenVM
            return new TokenVM
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(encodedToken),
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
