using Application.Dtos;
using Application.ViewModels;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IMenuHeadingsService
    {
        //Common Methods
        Task<List<MenuHeadingsCustomVM>> Get();
        Task<DateTime?> GetLastDateTime();
        Task<MenuHeadingsDTO> Add(MenuHeadingsDTO modelDto);
        Task<MenuHeadingsDTO> Get(int id);
        Task<MenuHeadingsVM> Get(string Heading);
        Task<List<MenuHeadingsVM>> GetForMenu(string Heading);
        Task<MenuHeadingsDTO> Update(MenuHeadingsDTO modelDto);
        Task<List<MenuHeadingsVM>> GetCategoriesWithAll();
        Task<int> Remove(int id);
        Task<List<MenuHeadingsVM>> GetCategoriesWithAllTest(int id);
        Task<MenuHeadingsDTO> EditMenuContent(MenuHeadingsDTO modelDTO);
        Task<int> UpdateMenus(List<MenuHeadingsListPriorityDto> lstMenuPriourty);
        Task<List<MenuHeadings>> GetSearch(string q);
    }
}
