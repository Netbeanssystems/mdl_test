using Application.ServiceInterfaces;
using Application.Services;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Rotativa.AspNetCore;
using Serilog;
using System;
using WebApp.Helpers;
using WebApp.IoC;

namespace WebApp
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServicesAPP(_config);
            var AllowedFormSize = Convert.ToInt32(_config["AllowedFormSize"]);
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = AllowedFormSize;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = AllowedFormSize; // if don't set default value is: 30 MB
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = AllowedFormSize;
                x.ValueLengthLimit = AllowedFormSize;
                x.MultipartBodyLengthLimit = AllowedFormSize;
                x.MultipartHeadersLengthLimit = AllowedFormSize;
            });

            services.AddHttpClient<IHttpClientService, HttpClientService>();

            //services.AddDataProtection()
            //  .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"C:\KeyRing")) //shared network folder for key location
            //  .SetApplicationName("WebApp")
            //  .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

            services.AddCors(options =>
            {
                options.AddPolicy("DigiLockerPolicy",
                    builder =>
                    {
                        builder.WithOrigins("https://partners.digitallocker.gov.in")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            services.AddResponseCaching();
            services.AddControllers(); // this is necessary for the captcha's image provider
            services.AddNotyf(notyfConfig => { notyfConfig.DurationInSeconds = 7; notyfConfig.IsDismissable = true; notyfConfig.Position = NotyfPosition.TopRight; });
            var mvcBuilder = services.AddRazorPages()
                            .AddXmlSerializerFormatters()
                            .AddXmlDataContractSerializerFormatters();
            //#if DEBUG
            //            mvcBuilder.AddRazorRuntimeCompilation();
            //#endif
            services.AddAntiforgery(o =>
            {
                o.HeaderName = "XSRF-TOKEN";
                o.Cookie.Name = _config["Antiforgery"];
                o.Cookie.Domain = _config["Domain"];
                o.Cookie.Path = _config["CookiePath"];
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
                o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            //services.AddProgressiveWebApp();
            services.AddAuthentication(sharedOptions =>
                    {
                        sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        sharedOptions.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = "/Errors/AccessDenied/";
                    options.ClaimsIssuer = "App";
                    options.LoginPath = "/Account/Login/";
                    options.LogoutPath = "/";
                    options.Cookie.Name = _config["AuthCookie"];
                    options.Cookie.Domain = _config["Domain"];
                    options.Cookie.Path = _config["CookiePath"];
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(_config["CookieExpiry"]));
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SessionStore = new CustomTicketStore(services, _config);
                });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = _config["Session"];
                options.Cookie.Domain = _config["Domain"];
                options.Cookie.Path = _config["CookiePath"];
                options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToInt32(_config["CookieExpiry"]));
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddBrowserDetection();
        }
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment hostingEnvironment)
        {
            app.UseNotyf();
            app.UseRewriter(new RewriteOptions()
                .AddRedirectToHttps(StatusCodes.Status301MovedPermanently));
            if (hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.UseStatusCodePagesWithReExecute("/Errors/Exception/{0}");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 365;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "private,max-age=" + durationInSeconds;
                }
            });
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("OPTIONS", "false");
                context.Response.Headers.Add("TRACE", "false");
                context.Response.Headers.Add("DEBUG", "false");
                await next();
            });
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                // this is necessary for the captcha's image provider
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
       //     RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)hostingEnvironment);
        }
    }
}
