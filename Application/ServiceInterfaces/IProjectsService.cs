using Application.Dtos;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.ServiceInterfaces
{
    public interface IProjectsService
    {
        //Common Methods
        Task<List<BidderProjectsDTO>> Get();
        Task<BidderProjectsDTO> Get(int id);
        Task<List<BidderProjectsDTO>> GetByIds(List<int> ids);
        Task<List<BidderYardsDTO>> GetYard(string projectId);
        Task<List<BidderYardsDTO>> GetYardsByMultipleProjects(string projectIds);
        Task<List<ProjectResponse>> GetProjects();
        Task<BidderProjectsDTO> Create(BidderProjectsDTO projectDto);
        Task<int> UploadQuota(UpdateQuotaDTO projectDto);
    }
}
