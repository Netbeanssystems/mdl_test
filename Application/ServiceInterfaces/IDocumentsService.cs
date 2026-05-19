using Application.Dtos;
using Application.ViewModels;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IDocumentsService
    {
        //Common Methods
        Task<List<DocumentsVM>> Get();
        Task<DocumentsDTO> Get(int id);
        Task<List<DocumentsVM>> Getbycreatedby(string createdby);
        Task<List<DocumentsVM>> GetByYearAndDescription(int? year, int? urlsTimingId);
        Task<List<DocumentsVM>> GetByURLsTimingAndDateRange(int? urlsTimingId, DateTime? fromDate, DateTime? toDate);
        Task<List<DocumentsVM>> GetGroupedByURLsTiming();
        Task<DocumentsDTO> Create(DocumentsDTO entity);
        Task<URLsTimingDTO> CreateURLsTiming(URLsTimingDTO entity);
        Task<List<URLsTimingVM>> GetURLsTiming();
        Task<List<URLsTimingVM>> GetActiveURLsTiming();
        Task<URLsTimingVM> GetCurrentActiveURLsTiming(string url);
        Task<List<int>> GetDocumentsYears();
        Task<DocumentsDTO> Update(DocumentsDTO entity);
        Task<int> Delete(int id);
        Task<List<DocumentsDTO>> CreateRange(List<DocumentsDTO> entities);
        Task<List<DocumentsDTO>> Upsert(List<DocumentsDTO> entities);
        Task<int> DeleteRange(List<DocumentsDTO> entities);
        //Custom Methods
        Task<List<DropdownVM>> GetDropdown();

        Task<List<ClosedWindowsVM>> GetClosedWindows();

        Task<bool> UpdateMultipleWindowsVisibility(List<ClosedWindowsDTO> dtos);

        Task<List<URLsTimingVM>> GetDocumentsIsShow();
        Task<List<DocumentsVM>> GetDocumentsList(int? urlsTimingId, DateTime? fromDate, DateTime? toDate);
        Task<bool> SaveDownloadLog(DocumentDownloadLog logEntity);
    }
}
