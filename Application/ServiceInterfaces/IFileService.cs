using Application.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IFileService
    {
        bool CheckVideoFile(IFormFile file);
        bool CheckVideoSize(IFormFile file);
        bool CheckImageFile(IFormFile file);
        bool CheckFileSize(IFormFile file);
        //bool CheckImageFiles(List<IFormFile> files);
        Task<string> SaveFileAsync(FileUploadDTO FileUploadDto);
        //Task<string> SaveFileAsync(string path, IFormFile file, bool changename, string nameorpath);
        Task<string> SaveFileAsync(string path, Stream stream, string name, string extension);
        //Task<string> SaveFileAsync(string path, IFormFile file);
        //Task<List<string>> SaveFileAsync(string path, List<IFormFile> files);
        bool DeleteFile(string path);
        byte[] Download(string path, string filename);
        //string CopyFile(string sourcepath, string sourcename, string destinationpath, string destinationname, bool overwrite);
        string GetPhysicalPath(string relativepath);
        void ExtractZipFile(string relativepath, IFormFile file);
        //Task<string> SaveFileAsync(string path, string fileName, byte[] content);

        //Added later from HPCL to be refactored
        bool CheckValidFile(IFormFile file);
        (bool IsValid, string ErrorMessage) ValidateTenderArchive(IFormFile file, bool isPriceBid = false);
        (bool IsValid, string ErrorMessage) ValidateBidDocument(IFormFile file, string docType);
        (bool IsValid, string ErrorMessage) ValidateFileNameSecurity(string fileName);
        bool CheckFiles(IFormFile file, string AllowedExtentions);
        //Added later from HPCL to be refactored
        Task<string> SaveImageAsync(string path, IFormFile file);
        Task<string> SaveEncryptionAsync(string path, IFormFile file);
        //Added later from HPCL to be refactored
        Task<List<string>> SaveImageAsync(string path, List<IFormFile> files);
    }
}