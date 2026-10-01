using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IRandomService
    {
        Task<int> RandomNumber(int min, int max);
        Task<string> RandomString(int size, bool lowerCase = true);
        Task<string> RandomSpecialChars(int size = 2);
        Task<string> RandomPassword();
        Task<string> GetUserName(string str, bool rnd = false);
    }
}
