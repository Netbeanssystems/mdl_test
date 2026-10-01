using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IAcademicYearsRepository : IRepository<AcademicYears>
    {
        Task<List<AcademicYears>> GetWithAll();
        Task<AcademicYears> CheckDuplicate(AcademicYears model);
    }
}
