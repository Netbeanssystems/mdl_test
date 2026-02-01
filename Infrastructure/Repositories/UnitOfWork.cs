using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IAcademicYearsRepository AcademicYearsRepo { get; }
        public IAuthenticationTicketsRepository AuthenticationTicketsRepo { get; }
        public IAuthenticationTicketsBankRepository AuthenticationTicketsBankRepo { get; }
        public IAuthenticationTicketsBidderRepository AuthenticationTicketsBidderRepo { get; }
        public ICountriesRepository CountriesRepo { get; }
        public IDashboardAlertsRepository DashboardAlertsRepo { get; }
        public IDistrictsRepository DistrictsRepo { get; }
        public ILoginLogsRepository LoginLogsRepo { get; }
        public IMenusRepository MenusRepo { get; }
        public INotificationDetailsRepository NotificationDetailsRepo { get; }
        public INotificationsRepository NotificationsRepo { get; }
        public IRefreshTokensRepository RefreshTokensRepo { get; }
        public IRoleMenusRepository RoleMenusRepo { get; }
        public ISiteAuthenticationTicketsRepository SiteAuthenticationTicketsRepo { get; }
        public IStatesRepository StatesRepo { get; }
        public IWebApiLogsRepository WebApiLogsRepo { get; }
        public IWebAppLogsRepository WebAppLogsRepo { get; }
        public IWebBankLogsRepository WebBankLogsRepo { get; }
        public IWebSiteLogsRepository WebSiteLogsRepo { get; }

        public IMenuHeadingsRepository MenuHeadingsRepo { get; }
        public ITempMenuHeadingsRepository TempMenuHeadingsRepo { get; }
        public IPhotoGalleryRepository PhotoGalleryRepo { get; set; }
        public ITempPhotoGalleryRepository TempPhotoGalleryRepo { get; set; }
        public IVideoRepository VideoRepo { get; set; }
        public ITempVideoRepository TempVideoRepo { get; set; }
        public INewsRepository NewsRepo { get; set; }
        public ITempNewsRepository TempNewsRepo { get; set; }
        public IOtherLinkHeadingRepository OtherLinkHeadingRepo { get; set; }
        public ITempOtherLinkHeadingRepository TempOtherLinkHeadingRepo { get; set; }
        public ITempBannerRepository TempBannerRepo { get; set; }
        public IBannerRepository BannerRepo { get; set; }
        public ITempMediaRepository TempMediaRepo { get; set; }
        public IMediaRepository MediaRepo { get; set; }
        public ITempWhatsNewRepository TempwhatsRepo { get; set; }
        public IWhatsNewRepository whatsRepo { get; set; }
        public IEventsRepository eventsRepo { get; set; }
        public IVigilanceRepository vigiRepo { get; set; }
        public IEnquiryRepository enquiryRepo { get; set; }
        public IFeedbackRepository feedbackRepo { get; set; }
        public IGrievanceRepository grievanceRepo { get; set; }
        public IContactUsRepository contactusRepo { get; set; }
        public IWebsiteCounterRepository counterRepo { get; set; }
        public IPasswordHistoryRepository passhistoryRepo { get; set; }
        public IForgetPasswordDetailsRepository ForgetPasswordDetailsRepo { get; set; }
        public IDocumentsRepository DocumentsRepo { get; set; }
        public IURLsTimingRepository URLsTimingRepo { get; set; }
        public IAuthRepository AuthRepo { get; set; }
        public IProjectsRepository ProjectRepo { get; set; }
        public IYardsRepository YardRepo { get; set; }
        public IBidderTenderUploadsRepository BidderTenderUploadRepo { get; set; }
        public IGeneraluploadURLRepository GeneraluploadURLRepo { get; set; }
        public IGenaralUploadDocumentsRepository GenaralUploadDocumentsRepo { get; set; }
        public IBidderCorrigendumUploadsRepository BidderCorrigendumRepo { get; set; }
        public IBidderTenderDocumentsRepository BidderTenderDocumentsRepo { get; set; }
        public IBidderGeneralDocRepository BidderGeneralDocRepo { get; set; }

        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            ForgetPasswordDetailsRepo = new ForgetPasswordDetailsRepository(_dbContext);
            AcademicYearsRepo = new AcademicYearsRepository(_dbContext);
            AuthenticationTicketsRepo = new AuthenticationTicketsRepository(_dbContext);
            AuthenticationTicketsBankRepo = new AuthenticationTicketsBankRepository(_dbContext);
            AuthenticationTicketsBidderRepo = new AuthenticationTicketsBidderRepository(_dbContext);
            CountriesRepo = new CountriesRepository(_dbContext);
            DashboardAlertsRepo = new DashboardAlertsRepository(_dbContext);
            DistrictsRepo = new DistrictsRepository(_dbContext);
            LoginLogsRepo = new LoginLogsRepository(_dbContext);
            MenusRepo = new MenusRepository(_dbContext);
            NotificationDetailsRepo = new NotificationDetailsRepository(_dbContext);
            NotificationsRepo = new NotificationsRepository(_dbContext);
            RefreshTokensRepo = new RefreshTokensRepository(_dbContext);
            RoleMenusRepo = new RoleMenusRepository(_dbContext);
            SiteAuthenticationTicketsRepo = new SiteAuthenticationTicketsRepository(_dbContext);
            StatesRepo = new StatesRepository(_dbContext);
            WebApiLogsRepo = new WebApiLogsRepository(_dbContext);
            WebAppLogsRepo = new WebAppLogsRepository(_dbContext);
            WebBankLogsRepo = new WebBankLogsRepository(_dbContext);
            WebSiteLogsRepo = new WebSiteLogsRepository(_dbContext);

            MenuHeadingsRepo = new MenuHeadingsRepository(_dbContext);
            TempMenuHeadingsRepo = new TempMenuHeadingsRepository(_dbContext);
            PhotoGalleryRepo = new PhotoGalleryRepository(_dbContext);
            TempPhotoGalleryRepo = new TempPhotoGalleryRepository(_dbContext);
            VideoRepo = new VideoRepository(_dbContext);
            TempVideoRepo = new TempVideoRepository(_dbContext);
            NewsRepo = new NewsRepository(_dbContext);
            TempNewsRepo = new TempNewsRepository(_dbContext);
            OtherLinkHeadingRepo = new OtherLinkHeadingRepository(_dbContext);
            TempOtherLinkHeadingRepo = new TempOtherLinkHeadingRepository(_dbContext);
            TempBannerRepo = new TempBannerRepository(_dbContext);
            BannerRepo = new BannerRepository(_dbContext);
            TempMediaRepo = new TempMediaRepository(_dbContext);
            MediaRepo = new MediaRepository(_dbContext);
            TempwhatsRepo = new TempWhatsNewRepository(_dbContext);
            whatsRepo = new WhatsNewRepository(_dbContext);
            eventsRepo = new EventsRepository(_dbContext);
            vigiRepo = new VigilanceRepository(_dbContext);
            enquiryRepo = new EnquiryRepository(_dbContext);
            feedbackRepo = new FeedbackRepository(_dbContext);
            grievanceRepo = new GrievanceRepository(_dbContext);
            contactusRepo = new ContactUsRepository(_dbContext);
            counterRepo = new WebsiteCounterRepository(_dbContext);
            passhistoryRepo = new PasswordHistoryRepository(_dbContext);
            DocumentsRepo = new DocumentsRepository(_dbContext);
            URLsTimingRepo = new URLsTimingRepository(_dbContext);
            AuthRepo = new AuthRepository(_dbContext);
            ProjectRepo = new ProjectsRepository(_dbContext);
            YardRepo = new YardsRepository(_dbContext);
            BidderTenderUploadRepo = new BidderTenderUploadsRepository(_dbContext);
            GeneraluploadURLRepo = new GeneraluploadURLRepository(_dbContext);
            GenaralUploadDocumentsRepo = new GenaralUploadDocumentsRepository(_dbContext);
            BidderCorrigendumRepo = new BidderCorrigendumUploadsRepository(_dbContext);
            BidderTenderDocumentsRepo = new BidderTenderDocumentsRepository(_dbContext);
            BidderGeneralDocRepo = new BidderGeneralDocRepository(_dbContext);
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
