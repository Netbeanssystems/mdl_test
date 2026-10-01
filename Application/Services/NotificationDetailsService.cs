using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Application.Services
{
    public class NotificationDetailsService : INotificationDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public NotificationDetailsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Common Methods
        public async Task<List<NotificationDetailsVM>> Get()
        {
            var models = await _unitOfWork.NotificationDetailsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<NotificationDetailsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<NotificationDetailsDTO> Get(int id)
        {
            var model = await _unitOfWork.NotificationDetailsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<NotificationDetailsDTO>(model);
            return modelDto;
        }
        public async Task<NotificationDetailsVM> GetVM(int id)
        {
            var model = await _unitOfWork.NotificationDetailsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelVm = _mapper.Map<NotificationDetailsVM>(model);
            return modelVm;
        }
        public async Task<NotificationDetailsDTO> Create(NotificationDetailsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<NotificationDetails>(argModelDto);
            _unitOfWork.NotificationDetailsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<NotificationDetailsDTO> Update(NotificationDetailsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<NotificationDetails>(argModelDto);
            _unitOfWork.NotificationDetailsRepo.Update(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.NotificationDetailsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            if (!model.IsActive)
                _unitOfWork.NotificationDetailsRepo.Delete(model);
            else
            {
                model.IsActive = false;
                _unitOfWork.NotificationDetailsRepo.Update(model);
            }
            ////if this was the last child set the parent IsActive false
            //var siblings = await _unitOfWork.NotificationDetailsRepo.GetByNotificationId(model.NotificationId, null).ConfigureAwait(false);
            //if (siblings.All(x => !x.IsActive))
            //{
            //    var notification = await _unitOfWork.NotificationsRepo.Get(model.NotificationId).ConfigureAwait(false);
            //    notification.IsActive = false;
            //    _unitOfWork.NotificationsRepo.Update(notification);
            //}
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? rowsChanged : -1;
        }
        //Custom Methods
        public async Task<List<NotificationDetailsVM>> GetAll()
        {
            var models = await _unitOfWork.NotificationDetailsRepo.GetAll().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<NotificationDetailsVM>>(models);
            return modelVms;
        }
        public async Task<List<NotificationDetailsVM>> GetByUser(string unm)
        {
            var models = await _unitOfWork.NotificationDetailsRepo.GetByUser(unm).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<NotificationDetailsVM>>(models);
            return modelVms;
        }
        public async Task<List<NotificationDetailsVM>> GetByNotificationId(int nid, string unm)
        {
            var models = await _unitOfWork.NotificationDetailsRepo.GetByNotificationId(nid, unm).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<NotificationDetailsVM>>(models);
            return modelVms;
        }
        //public async Task<int> Delete(int id, string unm, bool hard)
        //{
        //    var model = await _unitOfWork.NotificationDetailsRepo.Get(id).ConfigureAwait(false);
        //    if (model == null) return -1;
        //    if (string.IsNullOrEmpty(model.NotificationDetailDetails.Users) || hard)
        //        _unitOfWork.NotificationDetailsRepo.Delete(model);
        //    else
        //    {
        //        var Users = model.Users.Split(",").ToList();
        //        Users.Remove(unm);
        //        if (Users.Count <= 0 || unm == "all")
        //        {
        //            model.IsActive = false;
        //            
        //        }
        //        else
        //        {
        //            model.Users = string.Join(",", Users);
        //        }
        //        _unitOfWork.NotificationDetailsRepo.Update(model);
        //    }
        //    var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        //    return rowsChanged > 0 ? rowsChanged : -1;
        //}
        public async Task<NotificationDetailsVM> GetByTargetUrlUserName(string url, string uname)
        {
            var model = await _unitOfWork.NotificationDetailsRepo.GetByUrlUser(url, uname).ConfigureAwait(false);
            if (model == null) return null;
            var modelVm = _mapper.Map<NotificationDetailsVM>(model);
            return modelVm;
        }
    }
}
