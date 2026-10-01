using System;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IHttpClientServiceSite : IDisposable
    {
        Task<string> GetAsync(string path, bool addAuthHeader);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, int bid);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, int bid, int cid);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, int bid, int? cid);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, int bid, string cid);
        Task<string> GetAsync(string path, bool addAuthHeader, int? aid, int? bid);
        Task<string> GetAsync(string path, bool addAuthHeader, int? aid);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, int bid, int cid, int? did);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, int bid, int cid, int did, string str1);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, string bid, string cid, int? did);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, int bid, int cid, string str1);
        Task<string> GetAsync(string path, bool addAuthHeader, string str1);
        Task<string> GetAsync(string path, bool addAuthHeader, string str1, int aid);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, string str1);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, string str1, string str2);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, string str1, int? bid);
        Task<string> GetAsync(string path, bool addAuthHeader, int aid, string str1, string str2, string str3);
        Task<string> GetAsync(string path, bool addAuthHeader, string str1, string str2);
        Task<string> GetAsync(string path, bool addAuthHeader, string str1, string str2, string str3);
        Task<string> GetAsync(string path, bool addAuthHeader, string str1, string str2, string str3, string str4);

        Task<string> PutAsync(string path, bool addAuthHeader, int id, object model);
        Task<string> PutAsync(string path, bool addAuthHeader, string id, object model);

        Task<string> PostAsync(string path, bool addAuthHeader, object model);
        Task<string> PostMultipartAsync(string path, bool addAuthHeader, object model);

        Task<string> DeleteAsync(string path, bool addAuthHeader, int id);
        Task<string> DeleteAsync(string path, bool addAuthHeader, string id);
    }
}
