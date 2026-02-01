using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
namespace Application.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DbHelper _dbHelper;
        public ProjectsService(IUnitOfWork unitOfWork, IMapper mapper, DbHelper dbHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dbHelper = dbHelper;
        }
        //Common Methods
        public async Task<List<BidderProjectsDTO>> Get()
        {
            var models = await _unitOfWork.ProjectRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<BidderProjectsDTO>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<BidderProjectsDTO> Get(int id)
        {
            var model = await _unitOfWork.ProjectRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<BidderProjectsDTO>(model);
            var yardModel = await _unitOfWork.YardRepo.GetYard(id.ToString()).ConfigureAwait(false);
            modelDto.YardFrom = yardModel.Min(y => y.YardNumber);
            modelDto.YardTo = yardModel.Max(y => y.YardNumber);
            if (model == null) return null;
            return modelDto;
        }
        public async Task<List<BidderProjectsDTO>> GetByIds(List<int> ids)
        {
            var projectList = new List<BidderProjectsDTO>();

            if (ids == null || ids.Count == 0)
                return projectList;

            foreach (var id in ids)
            {
                // Get single project
                var model = await _unitOfWork.ProjectRepo.Get(id).ConfigureAwait(false);
                if (model == null)
                    continue;

                // Map to DTO
                var dto = _mapper.Map<BidderProjectsDTO>(model);

                // Get yard details
                var yardModel = await _unitOfWork.YardRepo.GetYard(id.ToString()).ConfigureAwait(false);
                if (yardModel != null && yardModel.Any())
                {
                    dto.YardFrom = yardModel.Min(y => y.YardNumber);
                    dto.YardTo = yardModel.Max(y => y.YardNumber);
                }

                projectList.Add(dto);
            }

            return projectList;
        }


        public async Task<List<BidderYardsDTO>> GetYard(string projectId)
        {
            var model = await _unitOfWork.YardRepo.GetYard(projectId).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<List<BidderYardsDTO>>(model);
            return modelDto;
        }
        public async Task<List<BidderYardsDTO>> GetYardsByMultipleProjects(string projectIds)
        {
            if (string.IsNullOrWhiteSpace(projectIds))
                return null;

            // ✅ Call repository directly with comma-separated IDs
            var yards = await _unitOfWork.YardRepo.GetYardbyyardis(projectIds).ConfigureAwait(false);

            if (yards == null || !yards.Any())
                return null;

            var yardDtos = _mapper.Map<List<BidderYardsDTO>>(yards);
            return yardDtos;
        }

        public async Task<List<ProjectResponse>> GetProjects()
        {
            var model = await _unitOfWork.YardRepo.GetProjects().ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<List<ProjectResponse>>(model);
            return modelDto;
        }

        public async Task<BidderProjectsDTO> Create(BidderProjectsDTO projectDto)
        {
            var procedureName = "Sp_BidderProjects";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ProjectId", SqlDbType.NVarChar) { Value = projectDto.Id },
                new SqlParameter("@ProjectName", SqlDbType.NVarChar) { Value = projectDto.ProjectName },
                new SqlParameter("@Remarks", SqlDbType.NVarChar) { Value = projectDto.Remarks },
                new SqlParameter("@TotalQuota", SqlDbType.NVarChar) { Value = projectDto.TotalQuota },
                new SqlParameter("@OccupiedQuota", SqlDbType.NVarChar) { Value = projectDto.OccupiedQuota },
                new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = projectDto.CreatedDate },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = projectDto.CreatedBy },
                new SqlParameter("@ModifiedDate", SqlDbType.DateTime) { Value = projectDto.ModifiedDate },
                new SqlParameter("@ModifiedBy", SqlDbType.NVarChar) { Value = projectDto.ModifiedBy },
                new SqlParameter("@IP", SqlDbType.NVarChar) { Value = projectDto.IP },
                new SqlParameter("@YardFrom", SqlDbType.NVarChar) { Value = projectDto.YardFrom },
                new SqlParameter("@YardTo", SqlDbType.NVarChar) { Value = projectDto.YardTo },
                new SqlParameter("@Type", SqlDbType.NVarChar) { Value = "1" }
            };
            var rowEffected = await _dbHelper.ExecuteStoredProcedureAndCheckRowsAffectedAsync(procedureName, parameters);
            if (rowEffected == 0 || rowEffected == -1) return null;
            return projectDto;
        }
        public async Task<int> UploadQuota(UpdateQuotaDTO projectDto)
        {
            if (projectDto == null) return 0;
            var model = _mapper.Map<BidderProjects>(projectDto);
            var rowAffected = await _unitOfWork.ProjectRepo.UploadQuota(model).ConfigureAwait(false);
            return rowAffected;
        }
    }
}
