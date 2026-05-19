using Application.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<AcademicYears> AcademicYears { get; set; }
        public DbSet<AuthenticationTickets> AuthenticationTickets { get; set; }
        public DbSet<AuthenticationTicketsBank> AuthenticationTicketsBank { get; set; }
        public DbSet<AuthenticationTicketsBidder> AuthenticationTicketsBidder { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<DashboardAlerts> DashboardAlerts { get; set; }
        public DbSet<Districts> Districts { get; set; }
        public DbSet<DocumentDownloadLog> documentDownloadLogs { get; set; } 
        public DbSet<LoginLogs> LoginLogs { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<NotificationDetails> NotificationDetails { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RoleMenus> RoleMenus { get; set; }
        public DbSet<SiteAuthenticationTickets> SiteAuthenticationTickets { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<WebApiLogs> WebApiLogs { get; set; }
        public DbSet<WebAppLogs> WebAppLogs { get; set; }
        public DbSet<WebBankLogs> WebBankLogs { get; set; }
        public DbSet<WebSiteLogs> WebSiteLogs { get; set; }

        public DbSet<MenuHeadings> MenuHeadings { get; set; }
        public DbSet<TempMenuHeadings> TempMenuHeadings { get; set; }
        public DbSet<PhotoGallery> PhotoGallery { get; set; }
        public DbSet<TempPhotoGallery> TempPhotoGallery { get; set; }
        public DbSet<Video> Video { get; set; }
        public DbSet<TempVideo> TempVideo { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<TempNews> TempNews { get; set; }
        public DbSet<OtherLinkHeading> OtherLinkHeading { get; set; }
        public DbSet<TempOtherLinkHeading> TempOtherLinkHeading { get; set; }

        //  Banners .........
        public DbSet<Banners> Banners { get; set; }
        //public DbSet<BannersData> BannersDatas { get; set; }
        public DbSet<TempBanners> TempBanners { get; set; }

        // for Media ........
        public DbSet<Media> Media { get; set; }
        public DbSet<TempMedia> TempMedia { get; set; }

        // for Whats New ........
        public DbSet<WhatsNew> WhatsNew { get; set; }
        public DbSet<TempWhatsNew> TempWhatsNew { get; set; }

        //Events
        public DbSet<Events> Events { get; set; }
        public DbSet<VigilanceForm> VigilanceForm { get; set; }
        public DbSet<EnquiryForm> EnquiryForm { get; set; }
        public DbSet<FeedbackForm> FeedbackForm { get; set; }
        public DbSet<GrievanceForm> GrievanceForm { get; set; }

        //Contact Us
        public DbSet<ContactUsForm> ContactUsForm { get; set; }

        //WebSiteCounters
        public DbSet<WebSiteCounters> WebSiteCounters { get; set; }
        public DbSet<PasswordHistory> PasswordHistory { get; set; }
        public DbSet<ForgetPasswordDetails> ForgetPasswordDetails { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<URLsTiming> URLsTiming { get; set; }
        public DbSet<ApplicationUser> AspNetUsers { get; set; }
        public DbSet<BidderProjects> BidderProjects { get; set; }
        public DbSet<BidderYards> BidderYards { get; set; }
        public DbSet<BidderTenderUploads> BidderTenderUploads { get; set; }
        public DbSet<BidderTenderCorrigendum> BidderTenderCorrigendum { get; set; }
        public DbSet<GenaralUploadDocuments> GenaralUploadDocuments { get; set; }
        public DbSet<GeneraluploadURL> GeneraluploadURL { get; set; }
        public DbSet<BidderTenderDocuments> BidderTenderDocuments { get; set; }
        public DbSet<BidderGeneralDoc> BidderGeneralDoc { get; set; }
    }
}