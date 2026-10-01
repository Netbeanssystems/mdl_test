using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryInterfaces
{
    public interface IMenuHeadingsRepository : IRepository<MenuHeadings>
    {
        Task<List<MenuHeadings>> GetCategoriesWithAll();
        Task<DateTime?> GetLastDateTime();
        Task<List<MenuHeadings>> GetCategoriesWithAllTest(int id);
        void UpdateMenuPriority(List<MenuHeadingsListPriority> lstMenuPriourty);
        Task<MenuHeadings> GetByMenuHeading(string Menu);
        Task<List<MenuHeadings>> GetForMenu(string Menu);
        Task<List<MenuHeadings>> GetSearch(string q);
    }
}