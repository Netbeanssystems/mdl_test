using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MediaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //-----------------Old -------
        public async Task<List<MediaVM>> Get()
        {
            var newProjects = await _unitOfWork.MediaRepo.Get().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<MediaVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms.OrderByDescending(x => x.UploadedDate).ToList();
            //return newProjectVms;
        }

        //------------------Karn 11Dec 2023---------- 
        public async Task<List<MediaVMNewModal>> GetAllMedia()
        {
            var newProjects = await _unitOfWork.MediaRepo.GetMedia().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<MediaVMNewModal>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms.OrderByDescending(x => x.UploadedDate).ToList();
            //return newProjectVms;
        }
        public async Task<List<MediaVM>> Getorder(string Key, int PageNo, int PageSize)
        {
            var newProjects = await _unitOfWork.MediaRepo.Getorder().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<MediaVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            if (PageSize != 0)
                return newProjectVms.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
            else
                return newProjectVms;

        }
        public async Task<List<MediaVM>> Getnews()
        {
            var newProjects = await _unitOfWork.MediaRepo.Getnews().ConfigureAwait(false);
            if (newProjects == null) return null;
            var newProjectVms = _mapper.Map<List<MediaVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<List<MediaVM>> Getorderdesc(int PageNo, int PageSize, int Minpage)
        {
            var newProjects = await _unitOfWork.MediaRepo.Getorderdesc().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<MediaVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            if (PageSize != 0 && Minpage == 1)
                return newProjectVms.Take((PageNo) * PageSize).OrderByDescending(x => x.Id).ToList();
            else
                return newProjectVms.Take((PageNo) * PageSize).OrderByDescending(x => x.Id).ToList();

        }
        public async Task<List<MediaVM>> Getorderdescfull()
        {
            var newProjects = await _unitOfWork.MediaRepo.Getorderdesc().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return null;
            var newProjectVms = _mapper.Map<List<MediaVM>>(newProjects);
            if (newProjectVms == null || newProjectVms.Count <= 0) return null;
            return newProjectVms;
        }
        public async Task<MediaDTO> Get(int id)
        {
            var newProject = await _unitOfWork.MediaRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return null;
            var newProjectDto = _mapper.Map<MediaDTO>(newProject);
            return newProjectDto;
        }

        public async Task<MediaDTO> Add(MediaDTO pressReleaseDTO)
        {
            if (pressReleaseDTO == null) return null;
            pressReleaseDTO.UploadedDate = System.DateTime.Now;
            var newpressrelease = _mapper.Map<Media>(pressReleaseDTO);
            _unitOfWork.MediaRepo.Create(newpressrelease);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            pressReleaseDTO.Id = newpressrelease.Id;
            return rowsChanged > 0 ? pressReleaseDTO : null;
        }

        public async Task<MediaDTO> Update(MediaDTO pressReleaseDTO)
        {
            if (pressReleaseDTO == null) return null;
            var newMarquee = _mapper.Map<Media>(pressReleaseDTO);
            _unitOfWork.MediaRepo.Update(newMarquee);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? pressReleaseDTO : null;
        }

        public async Task<int> Remove(int id)
        {
            var newProject = await _unitOfWork.MediaRepo.Get(id).ConfigureAwait(false);
            if (newProject == null) return -1;
            _unitOfWork.MediaRepo.Delete(newProject);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
        public async Task<MediaVM> GetMenus(int Id)
        {
            var category = await _unitOfWork.MediaRepo.Get(Id).ConfigureAwait(false);
            if (category == null) return null;
            var categoryDto = _mapper.Map<MediaVM>(category);
            return categoryDto;
        }
        public async Task<List<Media>> GetNewsData(int Heading)
        {
            var Result = await _unitOfWork.MediaRepo.GetNewsData(Heading).ConfigureAwait(false);
            if (Result == null || Result.Count <= 0) return null;
            var news = _mapper.Map<List<Media>>(Result);
            return news;
        }
        public async Task<MediaDTO> Getlastrecord()
        {
            var Result = await _unitOfWork.MediaRepo.Getlastrecord().ConfigureAwait(false);
            if (Result == null) return null;
            var news = _mapper.Map<MediaDTO>(Result);
            return news;
        }
        //public async Task<List<int>> GetNewsYear()
        //{
        //    var Result = await _unitOfWork.MediaRepo.GetNewsYear().ConfigureAwait(false);
        //    if (Result == null || Result.Count <= 0) return null;
        //    var news = _mapper.Map<List<int>>(Result);
        //    return news;
        //}
    }
}