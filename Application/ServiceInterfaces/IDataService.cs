namespace Application.ServiceInterfaces
{
    public interface IDataService
    {
        IAcademicYearsService AcademicYears { get; }
        IAuthenticationTicketsService AuthenticationTickets { get; }
        IAuthenticationTicketsBankService AuthenticationTicketsBank { get; }
        IAuthenticationTicketsBidderService AuthenticationTicketsBidder { get; }
        ICountriesService Countries { get; }
        IDashboardAlertsService DashboardAlerts { get; }
        IDistrictsService Districts { get; }
        ILoginLogsService LoginLogs { get; }
        IMenusService Menus { get; }
        INotificationDetailsService NotificationDetails { get; }
        INotificationsService Notifications { get; }
        IRefreshTokenService RefreshTokens { get; }
        IRoleMenusService RoleMenus { get; }
        ISiteAuthenticationTicketsService SiteAuthenticationTickets { get; }
        IStatesService States { get; }
        IWebApiLogsService WebApiLogs { get; }
        IWebAppLogsService WebAppLogs { get; }
        IWebBankLogsService WebBankLogs { get; }
        IWebSiteLogsService WebSiteLogs { get; }

        IMenuHeadingsService MenuHeadings { get; }
        ITempMenuHeadingsService TempMenuHeadings { get; }
        IPhotoGalleryService PhotoGallery { get; set; }
        ITempPhotoGalleryService TempPhotoGallery { get; set; }
        IVideoService Video { get; set; }
        ITempVideoService TempVideo { get; set; }
        INewsService News { get; }
        ITempNewsService TempNews { get; }
        IOtherLinkHeadingService OtherLinkHeading { get; }
        ITempOtherLinkHeadingService TempOtherLinkHeading { get; }
        ITempBannerService _tempbanner { get; }
        IBannerService _banner { get; }
        ITempMediaService _tempmedia { get; }
        IMediaService _media { get; }
        IWhatsNewService _whatsNew { get; }
        ITempWhatsNewService _TempwhatsNew { get; }
        IEventsService events { get; }
        IVigilanceFormService vigilance { get; }
        IEnquiryService enquiry { get; }
        IFeedbackService feedback { get; }
        IGrievanceService grievance { get; }
        IContactUsService contactus { get; }
        IWebsiteCounterService counter { get; }
        IPasswordHistoryService passhistory { get; }
        IForgetPasswordDetailsService ForgetPasswordDetailsServices { get; }
        IDocumentsService Documents { get; }
        IProjectsService Projects { get; }
        IBidderTenderUploadsService BidderTenderUpload { get; }
        IGenaralUploadDocumentsService GenaralUploadDocuments { get; }
        IBidderTenderDocumentsService BidderTenderDocuments { get; }
        IBidderGeneralDocService BidderGeneralDoc { get; }
    }
}