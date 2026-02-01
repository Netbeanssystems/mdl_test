using Application.ViewModels;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailVM EmailVm);
        Task SendEmailAsync2(EmailVM EmailVm);
        Task OnTaskCompleted(object sender, NotifierEventArgs args);
    }
}