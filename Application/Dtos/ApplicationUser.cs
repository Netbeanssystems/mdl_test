using Microsoft.AspNetCore.Identity;
using System;
namespace Application.Dtos
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string ProjectId { get; set; }
        public string YardId { get; set; }
        public string ExPNo { get; set; }
        public string ExtensionNo { get; set; }
        public string Designation { get; set; }
        public string OrganizationName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Department { get; set; }
        public DateTime? LoginValidFrom { get; set; }
        public DateTime? LoginValidTill { get; set; }
        public bool Approved { get; set; }
        public bool IsActive { get; set; }
        public bool ChangePassword { get; set; }
        public string EncSecret { get; set; }
        public string Pwd_SHA512 { get; set; }
    }
}
