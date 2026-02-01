using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EventsRepository : Repository<Events>, IEventsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public EventsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<Events>> GetDropdownById(int id)
        {
            return DbContext.Events
                .Where(x => x.Id == id && x.IsActive).ToListAsync();
        }



        //-----------------------------------New Karn Code 11 Dec 2023---------------
        public async Task<List<Events>> GetAllEvents()
        {

            // return await DbContext.Events.ToListAsync();

            return await DbContext.Events.Select(x =>
            new Events
            {
                Id = x.Id,
                EventName = x.EventName,
                EventNameHindi = x.EventNameHindi,


            }).ToListAsync();
        }



    }
}
