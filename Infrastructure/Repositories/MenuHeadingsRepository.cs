using Domain.Models;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MenuHeadingsRepository : Repository<MenuHeadings>, IMenuHeadingsRepository
    {
        private AppDbContext DbContext => _dbContext as AppDbContext;
        public MenuHeadingsRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<MenuHeadings>> GetCategoriesWithAll()
        {
            //return DbContext.MenuHeadings.Where(c => c.Show && c.ParentId == null)
            //    .OrderBy(c => c.Priority).ToListAsync();

            return await DbContext.MenuHeadings
                .Select(a =>
                new MenuHeadings
                {
                    Id = a.Id,
                    ParentId = a.ParentId,
                    Priority = a.Priority,
                    EnglishHeadingName = a.EnglishHeadingName,
                    HindiHeadingName = a.HindiHeadingName,
                    EnglishPageLink = a.EnglishPageLink,
                    HindiPageLink = a.HindiPageLink,
                    UpdateDate = a.UpdateDate,
                    Show = a.Show,
                    Children = a.Children
                                .Select(c =>
                                    new MenuHeadings
                                    {
                                        Id = c.Id,
                                        ParentId = c.ParentId,
                                        Priority = c.Priority,
                                        EnglishHeadingName = c.EnglishHeadingName,
                                        HindiHeadingName = c.HindiHeadingName,
                                        EnglishPageLink = c.EnglishPageLink,
                                        HindiPageLink = c.HindiPageLink,
                                        UpdateDate = c.UpdateDate,
                                        Show = c.Show,
                                        Children = c.Children

                                    })
                                .OrderBy(c => c.Priority)
                                .ToList()
                })
                .Where(a => a.Show && a.ParentId == null)
                .OrderBy(a => a.Priority)
                .ToListAsync();
        }

        public Task<DateTime?> GetLastDateTime()
        {
            return DbContext.MenuHeadings.Where(c => c.Show && c.ParentId == null)
                .OrderBy(c => c.Priority).OrderByDescending(x => x.UpdateDate).Take(1).Select(x => x.UpdateDate).FirstOrDefaultAsync();
        }

        //..............................Test..................
        public Task<List<MenuHeadings>> GetCategoriesWithAllTest(int id)
        {
            return DbContext.MenuHeadings.Where(c => c.Show && c.ParentId == id)
                .OrderBy(c => c.Priority).ToListAsync();
        }
        public void UpdateMenuPriority(List<MenuHeadingsListPriority> lstMenuPriourty)
        {
            foreach (MenuHeadingsListPriority item in lstMenuPriourty)
            {
                var ExisitingEntity = DbContext.MenuHeadings.First(x => x.Id == item.Id);
                ExisitingEntity.Priority = item.Priority;
                DbContext.MenuHeadings.Update(ExisitingEntity);
            }
        }

        public Task<MenuHeadings> GetByMenuHeading(string Menu)
        {
            return DbContext.MenuHeadings.FirstOrDefaultAsync(a => a.EnglishHeadingName.ToLower() == Menu);
        }

        public async Task<List<MenuHeadings>> GetForMenu(string Menu)
        {
            return await DbContext.MenuHeadings.Select(a => new MenuHeadings { EnglishHeadingName = a.EnglishHeadingName, HindiHeadingName = a.HindiHeadingName, EnglishAttachment = a.EnglishAttachment, EnglishPageLink = a.EnglishPageLink, ParentId = a.ParentId, Id = a.Id, Priority = a.Priority, Show = a.Show, Clickable = (a.EnglishContentDesc != null && a.EnglishContentDesc.Length > 0) }).ToListAsync();
            //return await DbContext.MenuHeadings.Select(a => new MenuHeadings { EnglishHeadingName = a.EnglishHeadingName, HindiHeadingName = a.HindiHeadingName, }).ToListAsync();
        }

        public Task<List<MenuHeadings>> GetSearch(string q)
        {
            //var a = DbContext.MenuHeadings.Where(a => a.EnglishContentDesc.Contains(q)).OrderByDescending(x => x.Id).Select(x => x.EnglishContentDesc).FirstOrDefault();
            //var b = Html(a.Replace("<", "");
            //b = a.Replace(">", "");
            //return DbContext.MenuHeadings.Where(a => a.EnglishHeadingName.Contains(q) || a.EnglishContentDesc.Contains(q)).OrderByDescending(x => x.Id).ToListAsync();
            //return DbContext.MenuHeadings
            //    .Where(a => a.EnglishHeadingName.Contains(q) && a.ParentId != null && DbContext.MenuHeadings.Any(parent => parent.Id == a.ParentId))
            //    .OrderByDescending(x => x.Id)
            //    .ToListAsync();
            return DbContext.MenuHeadings.Where(a =>(a.EnglishHeadingName.Contains(q) || a.EnglishContentDesc.Contains(q))&& a.ParentId != null
        && DbContext.MenuHeadings.Any(parent => parent.Id == a.ParentId)).OrderByDescending(x => x.Id).ToListAsync();
        }


    }
}