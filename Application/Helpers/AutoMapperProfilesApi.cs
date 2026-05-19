using Application.Dtos;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;

namespace Application.Helpers
{
    public class AutoMapperProfilesApi : Profile
    {
        public AutoMapperProfilesApi()
        {
            //ApplicationUser, UserProfileDTO
            CreateMap<ApplicationUser, UserProfileDTO>();
            CreateMap<UserProfileDTO, ApplicationUser>();
            CreateMap<ApplicationUser, UserVM>()
                .ForMember(dest => dest.PlainPass, opt => { opt.MapFrom(src => EnDeCryptor.DecryptStringAES(src.EncSecret)); });
            CreateMap<ApplicationUser, RegisterDTO>();
            CreateMap<ApplicationUser, DropdownStrVM>()
                .ForMember(dest => dest.Text, opt => { opt.MapFrom(src => src.UserName); });
            //AuthenticationTickets
            CreateMap<AuthenticationTicketsDTO, AuthenticationTickets>();
            CreateMap<AuthenticationTickets, AuthenticationTicketsDTO>();
            //AuthenticationTicketsBank
            CreateMap<AuthenticationTicketsBankDTO, AuthenticationTicketsBank>();
            CreateMap<AuthenticationTicketsBank, AuthenticationTicketsBankDTO>();
            //AuthenticationTicketsBidder
            CreateMap<AuthenticationTicketsBidderDTO, AuthenticationTicketsBidder>();
            CreateMap<AuthenticationTicketsBidder, AuthenticationTicketsBidderDTO>();
            //SiteAuthenticationTickets
            CreateMap<SiteAuthenticationTicketsDTO, SiteAuthenticationTickets>();
            CreateMap<SiteAuthenticationTickets, SiteAuthenticationTicketsDTO>();
            //Menus
            CreateMap<Menus, MenusVM>();
            CreateMap<MenuDTO, Menus>();
            CreateMap<Menus, MenuDTO>();
            //Notifications
            CreateMap<Notifications, NotificationsVM>();
            CreateMap<NotificationsDTO, Notifications>();
            CreateMap<Notifications, NotificationsDTO>();
            //Notifications
            CreateMap<NotificationDetails, NotificationDetailsVM>()
                .ForMember(x => x.Title, opt => { opt.MapFrom(src => src.Notification.Title); })
                .ForMember(x => x.Icon, opt => { opt.MapFrom(src => src.Notification.Icon); });
            CreateMap<NotificationDetailsDTO, NotificationDetails>();
            CreateMap<NotificationDetails, NotificationDetailsDTO>();
            //Countries
            CreateMap<Countries, CountriesVM>();
            CreateMap<CountriesDTO, Countries>();
            CreateMap<Countries, CountriesDTO>();
            CreateMap<Countries, DropdownVM>()
                .ForMember(dest => dest.Text, opt => { opt.MapFrom(src => src.Name); });
            //States
            CreateMap<States, StatesVM>()
                .ForMember(x => x.CountryName, opt => { opt.MapFrom(src => src.Country.Name); });
            CreateMap<StatesDTO, States>();
            CreateMap<States, StatesDTO>();
            CreateMap<States, DropdownVM>()
                .ForMember(dest => dest.Text, opt => { opt.MapFrom(src => src.StateName); });
            //Districts
            CreateMap<Districts, DistrictsVM>()
                .ForMember(dest => dest.StateName, opt => { opt.MapFrom(src => src.State.StateName); });
            CreateMap<DistrictsDTO, Districts>();
            CreateMap<Districts, DistrictsDTO>();
            CreateMap<Districts, DropdownVM>()
                .ForMember(dest => dest.Text, opt => { opt.MapFrom(src => src.DistrictName); });


            CreateMap<DocumentDownloadLog, DocumentDownloadLogDTO>();
            //RoleMenus
            CreateMap<RoleMenusDTO, RoleMenus>();
            CreateMap<RoleMenus, RoleMenusDTO>();
            //Roles
            CreateMap<ApplicationRole, RoleDTO>();
            CreateMap<RoleDTO, ApplicationRole>();
            //AcademicYears
            CreateMap<AcademicYears, AcademicYearsVM>();
            CreateMap<AcademicYearsDTO, AcademicYears>();
            CreateMap<AcademicYears, AcademicYearsDTO>();
            CreateMap<AcademicYears, DropdownVM>()
              .ForMember(dest => dest.Text, opt => { opt.MapFrom(src => src.Year); });

            //DashboardAlerts
            CreateMap<DashboardAlerts, DashboardAlertsVM>();
            CreateMap<DashboardAlertsDTO, DashboardAlerts>();
            CreateMap<DashboardAlerts, DashboardAlertsDTO>();

            //WebApiLogs
            CreateMap<WebApiLogs, WebApiLogsVM>();
            CreateMap<WebApiLogsDTO, WebApiLogs>();
            CreateMap<WebApiLogs, WebApiLogsDTO>();

            //WebAppLogs
            CreateMap<WebAppLogs, WebAppLogsVM>();
            CreateMap<WebAppLogsDTO, WebAppLogs>();
            CreateMap<WebAppLogs, WebAppLogsDTO>();

            //WebBankLogs
            CreateMap<WebBankLogs, WebBankLogsVM>();
            CreateMap<WebBankLogsDTO, WebBankLogs>();
            CreateMap<WebBankLogs, WebBankLogsDTO>();

            //WebSiteLogs
            CreateMap<WebSiteLogs, WebSiteLogsVM>();
            CreateMap<WebSiteLogsDTO, WebSiteLogs>();
            CreateMap<WebSiteLogs, WebSiteLogsDTO>();

            //LoginLogs
            CreateMap<LoginLogs, LoginLogsVM>();
            CreateMap<LoginLogsDTO, LoginLogs>();
            CreateMap<LoginLogs, LoginLogsDTO>();

            //Menu Heading
            CreateMap<MenuHeadings, MenuHeadingsDTO>();
            CreateMap<MenuHeadingsDTO, MenuHeadings>();
            CreateMap<MenuHeadings, MenuHeadingsVM>();
            CreateMap<MenuHeadingsListPriorityDto, MenuHeadingsListPriority>();
            CreateMap<MenuHeadings, MenuHeadingsCustomVM>();
            //CreateMap<MenuHeadingsTest, MenuHeadingsCustomVM>();
            CreateMap<MenuHeadings, MenuHeadingsCustomVMModal>();

            //Menu Temp Heading
            CreateMap<TempMenuHeadings, TempMenuHeadingsDTO>();
            CreateMap<TempMenuHeadingsDTO, TempMenuHeadings>();
            CreateMap<TempMenuHeadings, TempMenuHeadingsVM>();

            //Photogallery
            CreateMap<PhotoGallery, PhotoGalleryVM>();
            CreateMap<PhotoGalleryModel, PhotoGalleryVM>();
            CreateMap<PhotoGalleryDTO, PhotoGallery>();
            CreateMap<PhotoGallery, PhotoGalleryDTO>();

            //TempPhotogallery
            CreateMap<TempPhotoGallery, TempPhotoGalleryVM>();
            CreateMap<TempPhotoGalleryDTO, TempPhotoGallery>();
            CreateMap<TempPhotoGallery, TempPhotoGalleryDTO>();

            //Video
            CreateMap<Video, VideoVM>();
            CreateMap<VideoDTO, Video>();
            CreateMap<Video, VideoDTO>();

            //TempVideo
            CreateMap<TempVideo, TempVideoVM>();
            CreateMap<TempVideoDTO, TempVideo>();
            CreateMap<TempVideo, TempVideoDTO>();

            //Menu News
            CreateMap<News, NewsDTO>();
            CreateMap<NewsDTO, News>();
            CreateMap<News, NewsVM>();
            CreateMap<NewsListPriorityDto, NewsListPriorityModel>();
            CreateMap<News, NewsVMModal>();
            CreateMap<NewsData, NewsVMModal>();

            //Menu Temp News
            CreateMap<TempNews, TempNewsDTO>();
            CreateMap<TempNewsDTO, TempNews>();
            CreateMap<TempNews, TempNewsVM>();

            // OtherLinkHeading
            CreateMap<OtherLinkHeading, OtherLinkHeadingDTO>();
            CreateMap<OtherLinkHeadingDTO, OtherLinkHeading>();
            CreateMap<OtherLinkHeading, OtherLinkHeadingVM>();
            CreateMap<OtherLinkListPriorityDTO, OtherLinkListPriority>();

            // TempOtherLinkHeading
            CreateMap<TempOtherLinkHeading, TempOtherLinkHeadingDTO>();
            CreateMap<TempOtherLinkHeadingDTO, TempOtherLinkHeading>();
            CreateMap<TempOtherLinkHeading, TempOtherLinkHeadingVM>();

            //Banner
            CreateMap<Banners, BannerVM>();
            CreateMap<BannerDTO, Banners>();
            CreateMap<Banners, BannerDTO>();
            CreateMap<Banners, BannerVMModal>();
            CreateMap<BannersData, BannerVMModal>();

            //TempBanner
            CreateMap<TempBanners, TempBannerVM>();
            CreateMap<TempBannerDTO, TempBanners>();
            CreateMap<TempBanners, TempBannerDTO>();

            //Media
            CreateMap<Media, MediaVM>();
            CreateMap<MediaDTO, Media>();
            CreateMap<Media, MediaDTO>();
            CreateMap<Media, MediaVMNewModal>();
            CreateMap<MediaNewData, MediaVMNewModal>();

            //TempMedia
            CreateMap<TempMedia, TempMediaVM>();
            CreateMap<TempMediaDTO, TempMedia>();
            CreateMap<TempMedia, TempMediaDTO>();

            //Whats New
            CreateMap<WhatsNew, WhatsNewDTO>();
            CreateMap<WhatsNewDTO, WhatsNew>();
            CreateMap<WhatsNew, WhatsNewVM>();
            CreateMap<NewsListPriorityDto, NewsListPriorityModel>();
            CreateMap<WhatsNew, WhatsNewModal>();
            CreateMap<WhatsNewData, WhatsNewModal>();

            //Temp Whats New
            CreateMap<TempWhatsNew, TempWhatsNewDTO>();
            CreateMap<TempWhatsNewDTO, TempWhatsNew>();
            CreateMap<TempWhatsNew, TempWhatsNewVM>();

            //Events
            CreateMap<Events, EventVM>();
            CreateMap<EventDTO, Events>();
            CreateMap<Events, EventDTO>();
            CreateMap<Events, DropdownVM>()
              .ForMember(dest => dest.Text, opt => { opt.MapFrom(src => src.EventName); });

            //Vigilance Form
            CreateMap<VigilanceFormDTO, VigilanceForm>();
            CreateMap<VigilanceForm, VigilanceFormDTO>();

            //Enquiry Form
            CreateMap<EnquiryDTO, EnquiryForm>();
            CreateMap<EnquiryForm, EnquiryDTO>();

            //Feedback Form
            CreateMap<FeedbackDTO, FeedbackForm>();
            CreateMap<FeedbackForm, FeedbackDTO>();

            //Grievance Form
            CreateMap<GrievanceDTO, GrievanceForm>();
            CreateMap<GrievanceForm, GrievanceDTO>();

            //Contact Form
            CreateMap<ContactUsDTO, ContactUsForm>();
            CreateMap<ContactUsForm, ContactUsDTO>();

            //WebsiteCounter
            CreateMap<WebsiteCounterDTO, WebSiteCounters>();
            CreateMap<WebSiteCounters, WebsiteCounterDTO>();

            //WebsiteCounter
            CreateMap<PasswordHistory, PasswordHistoryVM>();
            CreateMap<PasswordHistoryVM, PasswordHistory>();

            //ForgetPasswordDetails  Documents
            CreateMap<ForgetPasswordDetails, ForgetPasswordDetailsDTO>();
            CreateMap<ForgetPasswordDetailsDTO, ForgetPasswordDetails>();
            CreateMap<ForgetPasswordDetails, ForgetPasswordDetailsVM>();


            //Documents  
            CreateMap<Documents, DocumentsDTO>();
            CreateMap<DocumentsDTO, Documents>();
            CreateMap<Documents, DocumentsVM>();

            //URLsTiming  
            CreateMap<URLsTiming, URLsTimingDTO>();
            CreateMap<URLsTimingDTO, URLsTiming>();
            CreateMap<URLsTiming, URLsTimingVM>();
            CreateMap<URLsTiming, ClosedWindowsVM>();
            CreateMap<ClosedWindowsDTO, URLsTiming>()
             .ForMember(dest => dest.IsShow, opt => opt.MapFrom(src => src.IsShow))
             .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Bidder Project 
            CreateMap<BidderProjects, BidderProjectsDTO>();
            CreateMap<BidderProjectsDTO, BidderProjects>();
            CreateMap<BidderProjects, UpdateQuotaDTO>();
            CreateMap<UpdateQuotaDTO, BidderProjects>();

            //Bidder Yard
            CreateMap<BidderYards, BidderYardsDTO>();
            CreateMap<BidderYardsDTO, BidderYards>();

            //GenaralUploadDocuments  
            CreateMap<GenaralUploadDocuments, GenaralUploadDocumentsDTO>();
            CreateMap<GenaralUploadDocumentsDTO, GenaralUploadDocuments>();
            CreateMap<GenaralUploadDocuments, GenaralUploadDocumentsVM>();

            //GeneraluploadURL  
            CreateMap<GeneraluploadURL, GeneraluploadURLDTO>();
            CreateMap<GeneraluploadURLDTO, GeneraluploadURL>();
            CreateMap<GeneraluploadURL, GeneraluploadURLVM>();

            // Upload Tender
            CreateMap<BidderTenderUploads, BidderTenderUploadsDTO>();
            CreateMap<BidderTenderUploadsDTO, BidderTenderUploads>();
            CreateMap<BidderTenderUploads, BidderTenderUploadsVM>();

            CreateMap<BidderTenderCorrigendumUploads, BidderTenderUploadsDTO>();
            CreateMap<BidderTenderUploadsDTO, BidderTenderCorrigendumUploads>();
            CreateMap<BidderTenderCorrigendumUploads, BidderTenderUploadsVM>();

            // Upload Corrigendum Tender
            CreateMap<BidderTenderCorrigendum, BidderTenderCorrigendumDto>();
            CreateMap<BidderTenderCorrigendumDto, BidderTenderCorrigendum>();
            CreateMap<BidderTenderCorrigendum, BidderTenderCorrigendumVM>();


            // For Tender Documents
            CreateMap<BidderTenderDocuments, BidderTenderDocumentsDTO>();
            CreateMap<BidderTenderDocumentsDTO, BidderTenderDocuments>();
            CreateMap<BidderTenderDocuments, BidderTenderDocumentsVM>();

            // For BidderGeneralDoc
            CreateMap<BidderGeneralDoc, BidderGeneralDocDTO>();
            CreateMap<BidderGeneralDocDTO, BidderGeneralDoc>();
            CreateMap<BidderGeneralDoc, BidderGeneralDocVM>();
        }
    }
}