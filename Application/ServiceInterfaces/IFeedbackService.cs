using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackDTO> Create(FeedbackDTO argModelDto);
        Task<List<FeedbackVM>> Get();
    }
}
