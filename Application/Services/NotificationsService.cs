using Application.Dtos;
using Application.Helpers;
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
    public class NotificationsService : INotificationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public NotificationsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //Common Methods
        public async Task<List<NotificationsVM>> Get()
        {
            var models = await _unitOfWork.NotificationsRepo.Get().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<NotificationsVM>>(models);
            if (modelVms == null || modelVms.Count <= 0) return null;
            return modelVms;
        }
        public async Task<NotificationsDTO> Get(int id)
        {
            var model = await _unitOfWork.NotificationsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelDto = _mapper.Map<NotificationsDTO>(model);
            return modelDto;
        }
        public async Task<NotificationsVM> GetVM(int id)
        {
            var model = await _unitOfWork.NotificationsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return null;
            var modelVm = _mapper.Map<NotificationsVM>(model);
            return modelVm;
        }
        public async Task<NotificationsDTO> Create(NotificationsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<Notifications>(argModelDto);
            _unitOfWork.NotificationsRepo.Create(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<NotificationsDTO> Update(NotificationsDTO argModelDto)
        {
            if (argModelDto == null) return null;
            var model = _mapper.Map<Notifications>(argModelDto);
            _unitOfWork.NotificationsRepo.UpdateWithChildren(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }
        public async Task<List<NotificationsVM>> GetAll()
        {
            var models = await _unitOfWork.NotificationsRepo.GetAll().ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            var modelVms = _mapper.Map<List<NotificationsVM>>(models);
            return modelVms;
        }
        public async Task<List<NotificationsVM>> GetByUser(string unm)
        {
            var models = await _unitOfWork.NotificationsRepo.GetByUser(unm).ConfigureAwait(false);
            if (models == null || models.Count <= 0) return null;
            foreach (var model in models)
                model.NotificationDetails = model.NotificationDetails.Where(x => x.UserName == unm && x.IsActive).ToList();
            var modelVms = _mapper.Map<List<NotificationsVM>>(models);
            return modelVms;
        }
        public async Task<int> Delete(int id)
        {
            var model = await _unitOfWork.NotificationsRepo.Get(id).ConfigureAwait(false);
            if (model == null) return -1;
            if (model.NotificationDetails?.Count > 0 && model.NotificationDetails.Any(x => x.IsActive)) return -1;
            _unitOfWork.NotificationsRepo.Delete(model);
            return await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //var rowsChanged = -1;
            //try
            //{
            //    rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            //}
            //catch (Exception ex)
            //{
            //    if (ex.GetType() == typeof(DbUpdateException) || ex.GetType() == typeof(DbUpdateConcurrencyException))
            //        rowsChanged = -2;
            //}
            //return rowsChanged;
        }

        public async Task<NotificationsDTO> Notify(NotificationVM NotificationVm)
        {
            if (NotificationVm == null) return null;
            var argModelDto = Notifier.Notify(NotificationVm);
            var model = _mapper.Map<Notifications>(argModelDto);
            _unitOfWork.NotificationsRepo.CreateWithChildren(model);
            var rowsChanged = await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return rowsChanged > 0 ? argModelDto : null;
        }

        //Event Handlers
        public void OnTaskCompleted(object sender, NotifierEventArgs args)
        {
            var argModelDto = Notifier.Notify(args.NotificationVm);
            var model = _mapper.Map<Notifications>(argModelDto);
            _unitOfWork.NotificationsRepo.CreateWithChildren(model);
            _unitOfWork.SaveChanges();
        }
    }
}
