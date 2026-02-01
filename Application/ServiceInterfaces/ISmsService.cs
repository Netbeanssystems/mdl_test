using Application.ViewModels;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ISmsService
    {
        Task<string> SendSmsAsync(SmsVM SmsVm);
        void OnTaskCompleted(object sender, NotifierEventArgs args);
    }
}