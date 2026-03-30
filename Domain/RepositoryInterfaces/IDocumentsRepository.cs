using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IDocumentsRepository : IRepository<Documents>
    {
        Task<List<Documents>> Getbycreatedby(string createdby);
        Task<List<Documents>> GetByYearAndDescription(int? year, int? urlsTimingId);
        Task<List<Documents>> GetByURLsTimingAndDateRange(int? urlsTimingId, DateTime? fromDate, DateTime? toDate);
        Task<List<Documents>> GetGroupedByURLsTiming();
        new Task<List<Documents>> GetActive();
    }
}
