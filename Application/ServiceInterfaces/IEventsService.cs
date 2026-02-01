using Application.Dtos;
using Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IEventsService
    {
        //Common Methods
        //Task<List<EventVM>> Get();
        Task<EventDTO> Get(int id);
        Task<EventDTO> Create(EventDTO entity);
        Task<EventDTO> Update(EventDTO entity);
        Task<int> Delete(int id);

        //Custom Methods
        Task<List<DropdownVM>> GetDropdown();
        Task<List<DropdownVM>> GetDropdownById(int id);




        Task<List<EventVM>> GetAllEvents();             //------------ Karn
    }
}
