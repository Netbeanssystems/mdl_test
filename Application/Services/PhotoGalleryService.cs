using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PhotoGalleryService : IPhotoGalleryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PhotoGalleryService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public List<PhotoGalleryVM> Get()
        {
            var newProjects = _unitOfWork.PhotoGalleryRepo.GetEvents();
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<PhotoGalleryVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }

        public async Task<List<PhotoGalleryVM>> GetbyPriorty()
        {
            var newProjects = await _unitOfWork.PhotoGalleryRepo.GetbyPriorty().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<PhotoGalleryVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<PhotoGalleryVM>> GetbyPriortyForAwardAccolades()
        {
            var newProjects = await _unitOfWork.PhotoGalleryRepo.GetbyPriortyForAwardAccolades().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<PhotoGalleryVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<PhotoGalleryDTO> Get(int id)
        {
            var newProject = await _unitOfWork.PhotoGalleryRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return null;
            var newProjectDto = _mapper.Map<PhotoGalleryDTO>(newProject);
            return newProjectDto;
        }

        public async Task<PhotoGalleryDTO> Add(PhotoGalleryDTO videoDto)
        {
            if (videoDto == null) return null;
            var newMarquee = _mapper.Map<PhotoGallery>(videoDto);
            _unitOfWork.PhotoGalleryRepo.Create(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            videoDto.Id = newMarquee.Id;
            return rowsChanged > 0 ? videoDto : null;
        }

        public async Task<PhotoGalleryDTO> Update(PhotoGalleryDTO videoDto)
        {
            if (videoDto == null) return null;
            var newMarquee = _mapper.Map<PhotoGallery>(videoDto);
            _unitOfWork.PhotoGalleryRepo.Update(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? videoDto : null;
        }

        public async Task<int> Remove(int id)
        {
            var newProject = await _unitOfWork.PhotoGalleryRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return -1;
            _unitOfWork.PhotoGalleryRepo.Delete(newProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }

        public async Task<List<PhotoGalleryVM>> Getorder(string Key, int PageNo, int PageSize)
        {
            var newProjects = await _unitOfWork.PhotoGalleryRepo.Getorder(Key).ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<PhotoGalleryVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            if (PageSize != 0)
                return newProjectVms.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
            else
                return newProjectVms;

        }
        public async Task<List<PhotoGalleryVM>> GetorderHindi(string Key, int PageNo, int PageSize)
        {
            var newProjects = await _unitOfWork.PhotoGalleryRepo.GetorderHindi(Key).ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<PhotoGalleryVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            if (PageSize != 0)
                return newProjectVms.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
            else
                return newProjectVms;

        }
        public async Task<List<PhotoGalleryVM>> GetorderPriortyForAwardAccolades(string Key, int PageNo, int PageSize)
        {
            var newProjects = await _unitOfWork.PhotoGalleryRepo.GetorderPriortyForAwardAccolades(Key).ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<PhotoGalleryVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            if (PageSize != 0)
                return newProjectVms.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
            else
                return newProjectVms;

        }
        public async Task<List<PhotoGalleryVM>> GetorderPriortyForAwardAccoladeshindi(string Key, int PageNo, int PageSize)
        {
            var newProjects = await _unitOfWork.PhotoGalleryRepo.GetorderPriortyForAwardAccoladeshindi(Key).ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<PhotoGalleryVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            if (PageSize != 0)
                return newProjectVms.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
            else
                return newProjectVms;

        }

        public async Task<List<PhotoGalleryVM>> GetCategory(string Key)
        {
            var category = await _unitOfWork.PhotoGalleryRepo.GetCategory(Key).ConfigureAwait(false);
            if (category == null || category.Count <= 0) return null;
            var categoryDto = _mapper.Map<List<PhotoGalleryVM>>(category);
            if (categoryDto == null || categoryDto.Count <= 0) return null;
            return categoryDto;
        }

        public List<TopEventsVM> GetTopEvents()
        {
            List<TopEventsVM> list = new List<TopEventsVM>();
            string CS = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("Sp_Events", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da1.SelectCommand.Parameters.AddWithValue("@calltype", "TopEvents");

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[0].Rows)
                    {
                        TopEventsVM vm = new TopEventsVM();
                        vm.Id = Convert.ToInt32(dr1["Id"].ToString());
                        vm.EventName = dr1["EventName"].ToString();
                        vm.EventNameHindi = dr1["EventNameHindi"].ToString();
                        vm.EnglishAttachment = dr1["EnglishAttachment"].ToString();
                        vm.HindiAttachment = dr1["HindiAttachment"].ToString();
                        list.Add(vm);
                    }
                }
            }
            return list;
        }

        public List<DropdownVM> GetYears()
        {
            List<DropdownVM> list = new List<DropdownVM>();
            string CS = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("Sp_Events", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da1.SelectCommand.Parameters.AddWithValue("@calltype", "GetYears");

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[0].Rows)
                    {
                        DropdownVM vm = new DropdownVM();
                        vm.Id = Convert.ToInt32(dr1["YearName"].ToString());
                        vm.Text = dr1["YearName"].ToString();
                        list.Add(vm);
                    }
                }
            }
            return list;
        }

        public List<TopEventsVM> GetEventsByYear(int year)
        {
            List<TopEventsVM> list = new List<TopEventsVM>();
            string CS = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("Sp_Events", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                if (year == 0)
                {
                    da1.SelectCommand.Parameters.AddWithValue("@calltype", "GetAllEvents");
                }
                else
                {
                    da1.SelectCommand.Parameters.AddWithValue("@calltype", "GetEventsByYear");
                }

                da1.SelectCommand.Parameters.AddWithValue("@year", year);

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[0].Rows)
                    {
                        TopEventsVM vm = new TopEventsVM();
                        vm.Id = Convert.ToInt32(dr1["Id"].ToString());
                        vm.EventName = dr1["EventName"].ToString();
                        vm.EventNameHindi = dr1["EventNameHindi"].ToString();
                        vm.EnglishAttachment = dr1["EnglishAttachment"].ToString();
                        vm.HindiAttachment = dr1["HindiAttachment"].ToString();
                        vm.Added_on = Convert.ToDateTime(dr1["CreatedDate"]);
                        vm.EventCategory = dr1["EventCategory"].ToString();
                        list.Add(vm);
                    }
                }
            }
            return list;
        }

        public List<EventPhotosVM> GetPhotos(int id)
        {
            List<EventPhotosVM> list = new List<EventPhotosVM>();
            string CS = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("Sp_Events", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da1.SelectCommand.Parameters.AddWithValue("@calltype", "GetPhotosByEvent");
                da1.SelectCommand.Parameters.AddWithValue("@evtid", id);

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[0].Rows)
                    {
                        EventPhotosVM vm = new EventPhotosVM();
                        vm.Id = Convert.ToInt32(dr1["Id"].ToString());
                        vm.EventName = dr1["EventName"].ToString();
                        vm.EventNameHindi = dr1["EventNameHindi"].ToString();
                        vm.EnglishAttachment = dr1["EnglishAttachment"].ToString();
                        vm.HindiAttachment = dr1["HindiAttachment"].ToString();
                        vm.EnglishHeading = dr1["EnglishHeading"].ToString();
                        vm.HindiHeading = dr1["HindiHeading"].ToString();
                        vm.Added_on = Convert.ToDateTime(dr1["CreatedDate"]);
                        list.Add(vm);
                    }
                }
            }
            return list;
        }
    }
}
