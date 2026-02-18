using Application.ServiceInterfaces;
using Application.Services;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Rotativa.AspNetCore;
using System;
using WebSite.Helpers;
using WebSite.IoC;
using WebSite.Middlewares;

namespace WebSite
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
            services.AddServicesSite(_config);
            services.AddResponseCaching();
            // services.AddControllers(); // this is necessary for the captcha's image provider
            services.AddNotyf(notyfConfig => { notyfConfig.DurationInSeconds = 7; notyfConfig.IsDismissable = true; notyfConfig.Position = NotyfPosition.TopRight; });
            services.AddHttpClient<IHttpClientServiceSite, HttpClientServiceSite>();
            var mvcBuilder = services.AddRazorPages()
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();
#if DEBUG
            //mvcBuilder.AddRazorRuntimeCompilation();
#endif

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
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = "/Errors/AccessDenied/";
                    options.ClaimsIssuer = "Site";
                    options.LoginPath = "/Account/Seller-Login/";
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
                    options.SessionStore = new CustomTicketStore(services);
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment hostingEnvironment, IEmailService emailService)
        {
            app.UseNotyf();
            app.UseRewriter(new RewriteOptions()
                .AddRedirectToHttps(StatusCodes.Status301MovedPermanently));
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (hostingEnvironment.IsDevelopment())
            {
                //app.UseMiddleware<ExceptionHandlingMiddleware>();
                //app.UseStatusCodePagesWithReExecute("/Errors/Exception/{0}");

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();

                app.UseStatusCodePagesWithReExecute("/Errors/Exception/{0}");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 365;
                    //const int durationInSeconds = 60;
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

            //app.UseSerilogRequestLogging();
            app.UseMiddleware<RoutingMiddleware>();
            app.UseRouting();
            app.UseResponseCaching();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            //app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                // this is necessary for the captcha's image provider
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(name: "default1", pattern: "{lang?}/{controller=Home}/{action=Index}");

                //endpoints.MapFallback(context => {
                //    context.Response.Redirect("/Errors/PageNotFound");
                //    return Task.CompletedTask;
                //});
            });
         //   RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)hostingEnvironment);
        }
    }
}