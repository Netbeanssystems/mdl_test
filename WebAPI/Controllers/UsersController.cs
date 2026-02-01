using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using WebBank.Extensions;
namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        Validate objbal = new Validate();
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IRandomService _randomService;
        private readonly IFileService _fileService;
        public readonly IConfiguration _config;
        public UsersController(
            IIdentityService identityService,
            IMapper mapper,
            IEmailService emailService,
            IRandomService randomService,
            IFileService fileService,
            IConfiguration config)
        {
            _identityService = identityService;
            _mapper = mapper;
            _emailService = emailService;
            _randomService = randomService;
            _fileService = fileService;
            _config = config;
        }
        [Authorize(Roles = "SuperAdmin,Admin,BidderSuperAdmin")]
        // GET Users
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var users = await _identityService.GetUsers().ConfigureAwait(false);
            if (users == null || users.Count <= 0) return NotFound("Users not found");
            var userVms = _mapper.Map<List<UserVM>>(users);
            return Ok(userVms);
        }

        [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin,BidderSuperAdmin")]
        // GET GetById
        [HttpGet("GetById/{uid}")]
        public async Task<IActionResult> GetById(string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return BadRequest("UserId is required");
            var user = await _identityService.GetUser(uid).ConfigureAwait(false);
            if (user == null)
                return BadRequest("User does not exist");
            var userTR = _mapper.Map<RegisterDTO>(user);
            var userRolesVm = await _identityService.GetRolesForUser(user).ConfigureAwait(false);
            userTR.Role = userRolesVm.userRoles.First();
            return Ok(userTR);
        }

        [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin,BidderSuperAdmin")]
        // POST "Users/Create"
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] RegisterDTO model)
        {
            //Password
            model.Password = await _randomService.RandomPassword().ConfigureAwait(false);

            model.Pwd_SHA512 = EnDeCryptor.sha512hash(model.Password);
            model.Name = objbal.OnlyValid(model.Name);
            model.Email = model.Email;
            model.PhoneNumber = objbal.OnlyValid(model.PhoneNumber);
            if (string.IsNullOrWhiteSpace(model.BranchName))
            {
                model.BranchName = "NA";
            }
            else
            {
                model.BranchName = objbal.OnlyValid(model.BranchName);
            }
            if (string.IsNullOrWhiteSpace(model.BranchAddress))
            {
                model.BranchAddress = "NA";
            }
            else
            {
                model.BranchAddress = objbal.OnlyValid(model.BranchAddress);
            }
            ;
            model.ProjectId = model.ProjectId;
            model.YardId = model.YardId;
            model.ExPNo = model.ExPNo;
            model.ExtensionNo = model.ExtensionNo;
            model.OrganizationName = model.OrganizationName;
            model.Address = model.Address;
            model.Country = model.Country;
            model.Designation = model.Designation;
            model.Department = model.Department;
            model.LoginValidFrom = model.LoginValidFrom;
            model.LoginValidTill = model.LoginValidTill;
            model.Approved = model.Approved;
            model.IsActive = model.IsActive;
            var user = await _identityService.CreateUser(model).ConfigureAwait(false);
            if (user == null) return BadRequest("User couldn't be created or added to specified role");
            var userTR = _mapper.Map<RegisterDTO>(user);
            //send the username and password to new User
            var body = string.Format("<html>" + Environment.NewLine +
                                     "<body>" + Environment.NewLine +
                                     "<h3>Respected User</h3>" + Environment.NewLine +
                                     "<p>Congratulations, your registration as a <b>{0}</b> with MDL has been successful.</p>" + Environment.NewLine +
                                     "<p>Your one time credentials for login into the system  are:</p>" + Environment.NewLine +
                                     "<p>Username: <b>{1}</b></p>" + Environment.NewLine +
                                     "<p>Password: <b>{2}</b></p>" + Environment.NewLine + Environment.NewLine +
                                     "<p><a href='https://mazagondock.in/bank' target='_blank'>Click here to login</a></p>" + Environment.NewLine + Environment.NewLine +
                                     "<p><b>Note</b>: This password is a one time temporary password, you are highly recommended to change your password on your first login.</p>" + Environment.NewLine + Environment.NewLine +
                                     "<p>Regards,</p>" + Environment.NewLine +
                                     "<p>Finance Department</p>" + Environment.NewLine +
                                     "<p>Mazagon Dock Shipbuilders Limited</p>" + Environment.NewLine +
                                     "<p>Mumbai - 400010</p>" + Environment.NewLine +
                                     "</body>" + Environment.NewLine +
                                     "</html>", model.Role, user.UserName, model.Password);


            if (_config["Environment"].ToString() == "Live")
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_config["SMTPFrom"]);
                mail.To.Add(new MailAddress(model.Email));
                mail.IsBodyHtml = true;
                mail.Subject = "Account credentials";
                mail.Body = body;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _config["SMTPHost"];
                smtp.Send(mail);
            }
            else if (_config["Environment"].ToString() == "Dev")
            {
                //Send email
                var EmailVm = new EmailVM
                {
                    ToAddresses = new List<string> { model.Email },
                    Subject = "Account credentials",
                    Body = body
                };
                await _emailService.SendEmailAsync(EmailVm).ConfigureAwait(false);
            }

            //using (MailMessage mail = new MailMessage())
            //{
            //    mail.From = new MailAddress(_config["SMTPFrom"]);
            //    mail.To.Add(new MailAddress(model.Email));
            //    mail.IsBodyHtml = true;
            //    mail.Subject = "Account credentials";
            //    mail.Body = body;

            //    using (SmtpClient smtp = new SmtpClient())
            //    {
            //        smtp.Host = _config["SMTPHost"];
            //        smtp.Send(mail);
            //    }
            //}

            return Ok(userTR);
        }
        [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin,BidderSuperAdmin")]
        // PUT: "Users/Edit/id
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] RegisterDTO model)
                {
            if (model == null) return BadRequest("Input not valid or null");
            if (id != model.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var user = await _identityService.GetUser(id).ConfigureAwait(false);
            if (user == null) return BadRequest("Input not valid or null");
            user.Name = objbal.OnlyValid(model.Name);
            user.Email = model.Email;
            user.OrganizationName = model.OrganizationName;
            user.Country = model.Country;
            user.Address = model.Address;
            user.LoginValidFrom = model.LoginValidFrom;
            user.LoginValidTill = model.LoginValidTill;
            user.ProjectId = model.ProjectId;
            user.YardId = model.YardId;
            user.ExPNo = model.ExPNo;
            user.ExtensionNo = model.ExtensionNo;
            user.Department = model.Department;
            user.PhoneNumber = objbal.OnlyValid(model.PhoneNumber);
            if (string.IsNullOrWhiteSpace(user.BranchName))
            {
                user.BranchName = "NA";
            }
            else
            {
                user.BranchName = objbal.OnlyValid(user.BranchName);
            }
            if (string.IsNullOrWhiteSpace(user.BranchAddress))
            {
                user.BranchAddress = "NA";
            }
            else
            {
                user.BranchAddress = objbal.OnlyValid(user.BranchAddress);
            }
            user.Approved = model.Approved;
            user.IsActive = model.IsActive;
            var result = await _identityService.Update(user).ConfigureAwait(false);
            if (!result.Succeeded) return BadRequest("User couldn't be updated");
            var userTR = _mapper.Map<RegisterDTO>(user);
            return Ok(userTR);
        }
        [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin,BidderSuperAdmin")]
        // GET: Users/UsersForList
        [HttpGet("UsersForList")]
        public async Task<IActionResult> UsersForList()
        {
            var users = await _identityService.GetUsers().ConfigureAwait(false);
            var userTR = _mapper.Map<List<UserVM>>(users);
            if (User.IsInRole("BankAdmin"))
            {
                userTR = userTR.Where(x => x.UserName.Contains("BankAdmin") || x.UserName.Contains("BankUser")).ToList();
            }
            return Ok(userTR);
        }
        [Authorize]
        // GET: Users/UserById
        [HttpGet("UserById/{uid}")]
        public async Task<IActionResult> UserById([FromRoute] string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return BadRequest("UserId is required");
            var user = await _identityService.GetUser(uid).ConfigureAwait(false);
            if (user == null)
                return BadRequest("User does not exist");
            var userTR = _mapper.Map<UserProfileDTO>(user);
            return Ok(userTR);
        }
        //Method accepting MultipartFormData with IFormFile
        // GET: Users/UpdateUser
        [Authorize]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser()
        {
            var Id = Request.Form["Id"].ToString();
            var Name = objbal.OnlyValid(Request.Form["Name"].ToString());
            var Email = Request.Form["Email"].ToString();
            var PhoneNumber = objbal.OnlyValid(Request.Form["PhoneNumber"].ToString());
            var ProfileImage = Request.Form["ProfileImage"].ToString();
            var Designation = Request.Form["Designation"].ToString();
            var Department = Request.Form["Department"].ToString();
            var ProjectId = Request.Form["ProjectId"].ToString();
            var YardId = Request.Form["YardId"].ToString();
            var ExtensionNo = Request.Form["ExtensionNo"].ToString();
            var user = await _identityService.GetUser(Id).ConfigureAwait(false);
            if (user == null)
                return BadRequest("User does not exist");
            if (Request.Form.Files.Count > 0)
            {
                var ImageFile = Request.Form.Files[0];
                if (ImageFile.Length > 0 && _fileService.CheckImageFile(ImageFile) && _fileService.CheckFileSize(ImageFile))
                {
                    var FileUploadDto = new FileUploadDTO
                    {
                        UploadedFile = ImageFile,
                        FilePath = "\\img\\users",
                        ChangeName = true,
                        ReturnValue = "name",
                        FileOldName = ProfileImage,
                        ChangeDimensions = true,
                        Width = 128,
                        Height = 128
                    };
                    if (!string.IsNullOrEmpty(ProfileImage) && ProfileImage != "default_user100.png")
                        //Delete previous file
                        _fileService.DeleteFile(!string.IsNullOrEmpty(FileUploadDto.FileOldName)
                            ? $"{FileUploadDto.FilePath}\\{FileUploadDto.FileOldName}"
                            : $"{FileUploadDto.FilePath}\\{FileUploadDto.UploadedFile.FileName}");

                    ProfileImage = await _fileService.SaveFileAsync(FileUploadDto).ConfigureAwait(false);
                }
            }
            user.Name = Name;
            user.Email = Email;
            user.PhoneNumber = PhoneNumber;
            user.ProfileImage = ProfileImage;
            user.Designation = Designation;
            user.Designation = Designation;
            user.ProjectId = ProjectId;
            user.YardId = YardId;
            user.ExtensionNo = ExtensionNo;
            var result = await _identityService.Update(user).ConfigureAwait(false);
            if (!result.Succeeded) return BadRequest("User couldn't be updated");
            var userTR = _mapper.Map<UserProfileDTO>(user);
            return Ok(userTR);
        }
        // GET: Users/UserRoles
        [Authorize(Roles = "SuperAdmin,Admin,BidderSuperAdmin")]
        [HttpGet("UserRoles/{uid}")]
        public async Task<IActionResult> UserRoles([FromRoute] string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return BadRequest("UserId is required");
            var user = await _identityService.GetUser(uid).ConfigureAwait(false);
            if (user == null) return BadRequest("User does not exist");
            var UserRoleVm = await _identityService.GetRolesForUser(user).ConfigureAwait(false);
            if (UserRoleVm == null)
                return BadRequest("User does not exist");
            return Ok(UserRoleVm);
        }
        // POST: Users/AddToRole
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("AddToRole")]
        public async Task<IActionResult> AddToRole([FromBody] UserRolesVM model)
        {
            if (string.IsNullOrEmpty(model.userId))
                return BadRequest("UserId is required");
            var user = await _identityService.GetUser(model.userId).ConfigureAwait(false);
            if (user == null)
                return BadRequest("User does not exist");
            var result = await _identityService.AddToRole(user, model.userRole).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var UserRoleVm = await _identityService.GetRolesForUser(user).ConfigureAwait(false);
                return Ok(UserRoleVm);
            }
            return BadRequest("User couldn't be added to role");
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        // POST: Users/RemoveFromRole
        [HttpPost("RemoveFromRole")]
        public async Task<IActionResult> RemoveFromRole([FromBody] UserRolesVM model)
        {
            if (string.IsNullOrEmpty(model.userId))
                return BadRequest("UserId is required");
            var user = await _identityService.GetUser(model.userId).ConfigureAwait(false);
            if (user == null)
                return BadRequest("User does not exist");
            var result = await _identityService.RemoveFromRole(user, model.userRole).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var UserRoleVm = await _identityService.GetRolesForUser(user).ConfigureAwait(false);
                return Ok(UserRoleVm);
            }
            return BadRequest("User couldn't be removed from role");
        }
        // GET Users Dropdown
        [Authorize(Roles = "SuperAdmin,Admin,BidderSuperAdmin")]
        [HttpGet("GetDropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var users = await _identityService.GetUsers().ConfigureAwait(false);
            if (users == null || users.Count <= 0) return NotFound("Users not found");
            var dropDownStrVms = _mapper.Map<List<DropdownStrVM>>(users.Where(x => x.Approved).ToList());
            return Ok(dropDownStrVms);
        }
        // GET: Users/GetRole
        [Authorize(Roles = "SuperAdmin,BankAdmin,AuditAdmin,BidderSuperAdmin")]
        [HttpGet("GetRole/{uid}")]
        public async Task<IActionResult> GetRole([FromRoute] string uid)
        {
            if (string.IsNullOrEmpty(uid)) return BadRequest("UserId is required");
            var user = await _identityService.GetUser(uid).ConfigureAwait(false);
            if (user == null) return BadRequest("User does not exist");
            var UserRoleVm = await _identityService.GetRoleForUser(user).ConfigureAwait(false);
            if (UserRoleVm == null) return BadRequest("User does not exist");
            return Ok(UserRoleVm);
        }
        // POST: Users/UpdateUserRole
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleVM model)
        {
            //The Validations
            if (string.IsNullOrEmpty(model.UserId)) return BadRequest("UserId is required");
            if (string.IsNullOrEmpty(model.Role)) return BadRequest("Role is required");
            //The User
            var user = await _identityService.GetUser(model.UserId).ConfigureAwait(false);
            if (user == null) return BadRequest("User does not exist");
            //The Roles
            var userRoles = (await _identityService.GetUserRoles(user).ConfigureAwait(false)).Distinct().ToList();
            if (userRoles.Count > 0)
            {
                foreach (var role in userRoles)
                {
                    var result1 = await _identityService.RemoveFromRole(user, role).ConfigureAwait(false);
                    if (!result1.Succeeded) return BadRequest("User couldn't be removed from existing role");
                }
            }
            var result2 = await _identityService.AddToRole(user, model.Role).ConfigureAwait(false);
            if (!result2.Succeeded) return BadRequest("User couldn't be added to specied role");
            return Ok(true);
        }
        [Authorize]
        // GET: Users/UpdatePasswordStatus
        [HttpGet("UpdatePasswordStatus/{uid}/{status}")]
        public async Task<IActionResult> UpdatePasswordStatus([FromRoute] string uid, [FromRoute] int status)
        {
            var user = await _identityService.GetUser(uid).ConfigureAwait(false);
            if (user == null)
                return BadRequest("User does not exist");
            user.ChangePassword = Convert.ToBoolean(status);
            var result = await _identityService.Update(user).ConfigureAwait(false);
            if (!result.Succeeded) return BadRequest("Password status couldn't be updated");
            return Ok(true);
        }
        [Authorize]
        // GET: Users/GetByRole
        [HttpGet("GetByRole/{role}")]
        public async Task<IActionResult> GetByRole([FromRoute] string role)
        
        
        {
            var users = await _identityService.GetUsersByRole(role).ConfigureAwait(false);
            if (users == null || users.Count <= 0)
                return BadRequest("No users in this role");
            var userVms = _mapper.Map<List<UserVM>>(users.Distinct());
            if (User.IsInRole("BankAdmin"))
            {
                userVms = userVms.Where(x => x.UserName.Contains("BankAdmin") || x.UserName.Contains("BankUser")).ToList();
            }
            return Ok(userVms);
        }
        [Authorize]
        // GET: Users/GetByRoles
        [HttpPost("GetByRoles")]
        public async Task<IActionResult> GetByRoles([FromBody] List<string> roles)
        {
            var UserRoleVms = new List<UserRoleVM>();
            foreach (var role in roles)
            {
                var usersByRole = await _identityService.GetUsersByRole(role).ConfigureAwait(false);
                UserRoleVms.AddRange(usersByRole.Distinct().Select(user => new UserRoleVM { Role = role, UserId = user.Id, UserName = user.UserName }));
            }
            if (UserRoleVms.Count <= 0)
                return BadRequest("No users in this role");
            return Ok(UserRoleVms);
        }
    }
}
