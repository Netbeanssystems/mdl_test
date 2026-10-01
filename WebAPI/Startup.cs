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
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using WebAPI.IoC;
using WebAPI.Middlewares;


namespace WebAPI
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
            var allowedOrigins = _config.GetSection("AllowedOrigins:Names").Get<string[]>();
            services.AddServicesAPI(_config);

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
                x.ValueLengthLimit = AllowedFormSize;
                x.MultipartBodyLengthLimit = AllowedFormSize;
                x.MultipartHeadersLengthLimit = AllowedFormSize;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("SameOriginPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            //Add Authorization
            services.AddAuthorization();
            //Add CORS
            services.AddCors();
            //Add Response Caching
            services.AddResponseCaching();
            //Add Controllers
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    //options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                });
            //Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase("/api");

            //http to https redirection
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(StatusCodes.Status301MovedPermanently));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.UseHsts();
            }
            //Configure Allowed Origins
            var AllowedOrigins = _config.GetSection("AllowedOrigins:Names").Get<string[]>();
            app.UseCors(x => x
                .WithOrigins(AllowedOrigins)
                .WithMethods("GET", "POST", "PUT", "DELETE")
            );

              app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 365;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "private,max-age=" + durationInSeconds;
                }
            });
            //Serilog
            app.UseSerilogRequestLogging();
            //Use Response Caching
            app.UseResponseCaching();

            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "API v1");
                   
                    c.DocExpansion(DocExpansion.None);
                    c.DefaultModelsExpandDepth(-1);
                    c.DefaultModelExpandDepth(-1);
                    if (env.IsDevelopment() || env.IsStaging()) return;
                    c.SupportedSubmitMethods();
                });
            }
            else
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == "/" || context.Request.Path == "/swagger")
                    {
                        context.Response.Redirect("/api/api-online.html");
                        return;
                    }
                    await next();
                });
            }
            app.UseRouting();
            app.UseCors("SameOriginPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            //To redirect to swagger
            //app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
