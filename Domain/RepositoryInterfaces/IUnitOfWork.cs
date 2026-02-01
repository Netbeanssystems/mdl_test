using System;
using System.Threading.Tasks;
namespace Domain.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAcademicYearsRepository AcademicYearsRepo { get; }
        IAuthenticationTicketsRepository AuthenticationTicketsRepo { get; }
        IAuthenticationTicketsBankRepository AuthenticationTicketsBankRepo { get; }
        IAuthenticationTicketsBidderRepository AuthenticationTicketsBidderRepo { get; }
        ICountriesRepository CountriesRepo { get; }
        IDashboardAlertsRepository DashboardAlertsRepo { get; }
        IDistrictsRepository DistrictsRepo { get; }
        ILoginLogsRepository LoginLogsRepo { get; }
        IMenusRepository MenusRepo { get; }
        INotificationDetailsRepository NotificationDetailsRepo { get; }
        INotificationsRepository NotificationsRepo { get; }
        IRefreshTokensRepository RefreshTokensRepo { get; }
        IRoleMenusRepository RoleMenusRepo { get; }
        ISiteAuthenticationTicketsRepository SiteAuthenticationTicketsRepo { get; }
        IStatesRepository StatesRepo { get; }
        IWebApiLogsRepository WebApiLogsRepo { get; }
        IWebAppLogsRepository WebAppLogsRepo { get; }
        IWebBankLogsRepository WebBankLogsRepo { get; }
        IWebSiteLogsRepository WebSiteLogsRepo { get; }

        IMenuHeadingsRepository MenuHeadingsRepo { get; }
        ITempMenuHeadingsRepository TempMenuHeadingsRepo { get; }
        IPhotoGalleryRepository PhotoGalleryRepo { get; set; }
        ITempPhotoGalleryRepository TempPhotoGalleryRepo { get; set; }
        IVideoRepository VideoRepo { get; set; }
        ITempVideoRepository TempVideoRepo { get; set; }
        INewsRepository NewsRepo { get; set; }
        ITempNewsRepository TempNewsRepo { get; set; }
        IOtherLinkHeadingRepository OtherLinkHeadingRepo { get; set; }
        ITempOtherLinkHeadingRepository TempOtherLinkHeadingRepo { get; set; }
        ITempBannerRepository TempBannerRepo { get; set; }
        IBannerRepository BannerRepo { get; set; }
        ITempMediaRepository TempMediaRepo { get; set; }
        IMediaRepository MediaRepo { get; set; }
        ITempWhatsNewRepository TempwhatsRepo { get; set; }
        IWhatsNewRepository whatsRepo { get; set; }
        IEventsRepository eventsRepo { get; set; }
        IVigilanceRepository vigiRepo { get; set; }
        IEnquiryRepository enquiryRepo { get; set; }
        IFeedbackRepository feedbackRepo { get; set; }
        IGrievanceRepository grievanceRepo { get; set; }
        IContactUsRepository contactusRepo { get; set; }
        IWebsiteCounterRepository counterRepo { get; set; }
        IPasswordHistoryRepository passhistoryRepo { get; set; }
        IForgetPasswordDetailsRepository ForgetPasswordDetailsRepo { get; set; }
        IDocumentsRepository DocumentsRepo { get; set; }
        IURLsTimingRepository URLsTimingRepo { get; set; }
        IAuthRepository AuthRepo { get; set; }
        IProjectsRepository ProjectRepo { get; set; }
        IYardsRepository YardRepo { get; set; }
        IBidderTenderUploadsRepository BidderTenderUploadRepo { get; set; }
        IGeneraluploadURLRepository GeneraluploadURLRepo { get; set; }
        IGenaralUploadDocumentsRepository GenaralUploadDocumentsRepo { get; set; }
        IBidderCorrigendumUploadsRepository BidderCorrigendumRepo { get; set; }
        IBidderTenderDocumentsRepository BidderTenderDocumentsRepo { get; set; }
        IBidderGeneralDocRepository BidderGeneralDocRepo { get; set; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}