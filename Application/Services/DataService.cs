using Application.Helpers;
using Application.ServiceInterfaces;
using AutoMapper;
using Domain.Models;
using Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class DataService : IDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly DbHelper _dbHelper;
        public IAcademicYearsService AcademicYears { get; }
        public IAuthenticationTicketsService AuthenticationTickets { get; }
        public IAuthenticationTicketsBankService AuthenticationTicketsBank { get; }
        public IAuthenticationTicketsBidderService AuthenticationTicketsBidder { get; }
        public ICountriesService Countries { get; }
        public IDashboardAlertsService DashboardAlerts { get; }
        public IDistrictsService Districts { get; }
        public ILoginLogsService LoginLogs { get; }
        public IMenusService Menus { get; }
        public INotificationDetailsService NotificationDetails { get; }
        public INotificationsService Notifications { get; }
        public IRefreshTokenService RefreshTokens { get; }
        public IRoleMenusService RoleMenus { get; }
        public ISiteAuthenticationTicketsService SiteAuthenticationTickets { get; }
        public IStatesService States { get; }
        public IWebApiLogsService WebApiLogs { get; }
        public IWebAppLogsService WebAppLogs { get; }
        public IWebBankLogsService WebBankLogs { get; }
        public IWebSiteLogsService WebSiteLogs { get; }

        public IMenuHeadingsService MenuHeadings { get; }
        public ITempMenuHeadingsService TempMenuHeadings { get; }
        public IPhotoGalleryService PhotoGallery { get; set; }
        public ITempPhotoGalleryService TempPhotoGallery { get; set; }
        public IVideoService Video { get; set; }
        public ITempVideoService TempVideo { get; set; }
        public INewsService News { get; }
        public ITempNewsService TempNews { get; }
        public IOtherLinkHeadingService OtherLinkHeading { get; }
        public ITempOtherLinkHeadingService TempOtherLinkHeading { get; }
        public ITempBannerService _tempbanner { get; }
        public IBannerService _banner { get; }
        public ITempMediaService _tempmedia { get; }
        public IMediaService _media { get; }
        public IWhatsNewService _whatsNew { get; }
        public ITempWhatsNewService _TempwhatsNew { get; }
        public IEventsService events { get; }
        public IVigilanceFormService vigilance { get; }
        public IEnquiryService enquiry { get; }
        public IFeedbackService feedback { get; }
        public IGrievanceService grievance { get; }
        public IContactUsService contactus { get; }
        public IWebsiteCounterService counter { get; }
        public IPasswordHistoryService passhistory { get; }
        public IForgetPasswordDetailsService ForgetPasswordDetailsServices { get; }
        public IDocumentsService Documents { get; }
        public IProjectsService Projects { get; }
        public IBidderTenderUploadsService BidderTenderUpload { get; }
        public IGenaralUploadDocumentsService GenaralUploadDocuments { get; }
        public IBidderTenderDocumentsService BidderTenderDocuments { get; }
        public IBidderGeneralDocService BidderGeneralDoc { get; }

        public DataService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, DbHelper dbHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _dbHelper = dbHelper;
            ForgetPasswordDetailsServices = new ForgetPasswordDetailsService(_unitOfWork, _mapper);
            AcademicYears = new AcademicYearsService(_unitOfWork, _mapper);
            AuthenticationTickets = new AuthenticationTicketsService(_unitOfWork, _mapper);
            AuthenticationTicketsBank = new AuthenticationTicketsBankService(_unitOfWork, _mapper);
            AuthenticationTicketsBidder = new AuthenticationTicketsBidderService(_unitOfWork, _mapper);
            Countries = new CountriesService(_unitOfWork, _mapper);
            DashboardAlerts = new DashboardAlertsService(_unitOfWork, _mapper);
            Districts = new DistrictsService(_unitOfWork, _mapper);
            LoginLogs = new LoginLogsService(_unitOfWork, _mapper);
            Menus = new MenusService(_unitOfWork, _mapper);
            NotificationDetails = new NotificationDetailsService(_unitOfWork, _mapper);
            Notifications = new NotificationsService(_unitOfWork, _mapper);
            RefreshTokens = new RefreshTokenService(_unitOfWork, _mapper);
            RoleMenus = new RoleMenusService(_unitOfWork, _mapper);
            SiteAuthenticationTickets = new SiteAuthenticationTicketsService(_unitOfWork, _mapper);
            States = new StatesService(_unitOfWork, _mapper);
            WebApiLogs = new WebApiLogsService(_unitOfWork, _mapper);
            WebAppLogs = new WebAppLogsService(_unitOfWork, _mapper);
            WebBankLogs = new WebBankLogsService(_unitOfWork, _mapper);
            WebSiteLogs = new WebSiteLogsService(_unitOfWork, _mapper);

            MenuHeadings = new MenuHeadingsService(_unitOfWork, _mapper);
            TempMenuHeadings = new TempMenuHeadingsService(_unitOfWork, _mapper);
            PhotoGallery = new PhotoGalleryService(_unitOfWork, _mapper, _configuration);
            TempPhotoGallery = new TempPhotoGalleryService(_unitOfWork, _mapper);
            Video = new VideoService(_unitOfWork, _mapper);
            TempVideo = new TempVideoService(_unitOfWork, _mapper);
            News = new NewsService(_unitOfWork, _mapper);
            TempNews = new TempNewsService(_unitOfWork, _mapper);
            OtherLinkHeading = new OtherLinkHeadingService(_unitOfWork, _mapper);
            TempOtherLinkHeading = new TempOtherLinkHeadingService(_unitOfWork, _mapper);
            _tempbanner = new TempBannerService(_unitOfWork, _mapper);
            _banner = new BannerService(_unitOfWork, _mapper);
            _tempmedia = new TempMediaService(_unitOfWork, _mapper);
            _media = new MediaService(_unitOfWork, _mapper);
            _whatsNew = new WhatsNewService(_unitOfWork, _mapper);
            _TempwhatsNew = new TempWhatsNewService(_unitOfWork, _mapper);
            events = new EventsService(_unitOfWork, _mapper);
            vigilance = new VigilanceService(_unitOfWork, _mapper);
            enquiry = new EnquiryService(_unitOfWork, _mapper);
            feedback = new FeedbackService(_unitOfWork, _mapper);
            grievance = new GrievanceService(_unitOfWork, _mapper);
            contactus = new ContactUsService(_unitOfWork, _mapper);
            counter = new WebsiteCounterService(_unitOfWork, _mapper);
            passhistory = new PasswordHistoryService(_unitOfWork, _mapper);
            Documents = new DocumentsService(_unitOfWork, _mapper);
            Projects = new ProjectsService(_unitOfWork, _mapper, _dbHelper);
            BidderTenderUpload = new BidderTenderUploadsService(_unitOfWork, _mapper);
            GenaralUploadDocuments = new GenaralUploadDocumentsService(_unitOfWork, _mapper);
            BidderTenderDocuments = new BidderTenderDocumentsService(_unitOfWork, _mapper);
            BidderGeneralDoc = new BidderGeneralDocService(_unitOfWork, _mapper);
        }
    }
}