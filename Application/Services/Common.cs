using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Application.Services
{
    public class Common : ICommon
    {
        public bool CheckValidFile(IFormFile file)
        {
            string fileclass = "";
            bool Status = false;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var FF = reader.BaseStream;
                fileclass = reader.Read().ToString();
                fileclass += reader.Read().ToString();
            }
            //To Get File Extension  
            string FileExtension = Path.GetExtension(file.FileName);
            if (FileExtension == ".xls" || FileExtension == ".xlsx" || FileExtension == ".pdf" || FileExtension == ".jpg" || FileExtension == ".JPG" || FileExtension == ".png" || FileExtension == ".PNG" || FileExtension == ".jpeg" || FileExtension == ".doc" || FileExtension == ".docx")
                Status = true;
            //To Get File Type From Extenion  
            var contentType = MimeTypes.GetContentType(file.FileName);
            if (contentType == "application/pdf" || contentType == "application/excel" || contentType == "image/jpeg") // || contentType == "application/msword")
                Status = true;
            //......//To Get Actual File Using Array.......
            //if (fileclass == "7790" || fileclass == "8297" || fileclass == "760")
            if (fileclass != "3780" && fileclass != "6553365533")
                Status = false;
            return Status;
        }

        public bool CheckValidJpgFile(IFormFile file)
        {
            string fileclass = "";
            bool Status = false;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var FF = reader.BaseStream;
                fileclass = reader.Read().ToString();
                fileclass += reader.Read().ToString();
            }
            //To Get File Extension  
            string FileExtension = Path.GetExtension(file.FileName);
            if (FileExtension == ".jpg" || FileExtension == ".png" || FileExtension == ".PNG" || FileExtension == ".jpeg")
                Status = true;
            //To Get File Type From Extenion  
            //var contentType = MimeTypes.GetContentType(file.FileName);
            //if (contentType == "application/pdf" || contentType == "application/excel") // || contentType == "application/msword")
            //    Status = true;
            //......//To Get Actual File Using Array.......


            //if (fileclass == "7790" || fileclass == "8297" || fileclass == "760" || fileclass == "3780" || fileclass == "116101")
            //    Status = false;

            if (fileclass != "6553365533" && fileclass != "6553380")
            {
                Status = false;
            }

            return Status;
        }
    }
}