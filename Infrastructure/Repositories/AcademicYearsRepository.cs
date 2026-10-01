using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class AcademicYearsRepository : Repository<AcademicYears>, IAcademicYearsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public AcademicYearsRepository(AppDbContext context) : base(context)
        {
        }
        public Task<List<AcademicYears>> GetWithAll()
        {
            return DbContext.AcademicYears
                .Where(x =>
                    x.IsActive
                   )
                .OrderBy(c => c.Id)
                .ToListAsync();
        }
        public Task<AcademicYears> CheckDuplicate(AcademicYears model)
        {
            return DbContext.AcademicYears
                .FirstOrDefaultAsync(x =>
                    x.Year == model.Year
                   );
        }
    }
}
