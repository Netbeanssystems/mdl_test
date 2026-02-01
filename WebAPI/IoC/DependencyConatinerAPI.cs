using Application.AppSettings;
using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.Services;
using AutoMapper;
using Domain.RepositoryInterfaces;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace WebAPI.IoC
{
    public static class DependencyContainerAPI
    {
        private static readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("4leroyts-kt9d-6gt3-7e57-5346789jh45j-gf7x-4123234re45s-6f87-6jhtdsfrs"));
        public static IServiceCollection AddServicesAPI(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Database Context and set database to use
            services.AddDbContext<AppDbContext>(optionBuilder => optionBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"), builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(60), null);
                })
            );

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<ISmsService, SmsService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRandomService, RandomService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEventService, EventService>();
            services.AddSingleton<ICommon, Application.Services.Common>();
            services.AddScoped<DbHelper>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
                {
                    option.Password.RequiredLength = 8;
                    option.Password.RequireDigit = true;
                    option.Password.RequireNonAlphanumeric = true;
                    option.Password.RequireUppercase = true;
                    option.Password.RequireLowercase = true;
                    option.User.RequireUniqueEmail = true;
                    option.Lockout.MaxFailedAccessAttempts = 5;
                    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddDefaultTokenProviders()
                 .AddEntityFrameworkStores<AppDbContext>();
            //Add JWT Service for generating JWT and Refresh Tokens
            services.AddSingleton<IJwtService, JwtService>();
            //Remove default claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //Initialize Jwt Issuer Options
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtSettings));
            services.Configure<JwtSettings>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtSettings.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtSettings.Audience)];
                options.AccessTokenExpiry = Convert.ToInt32(jwtAppSettingOptions[nameof(JwtSettings.AccessTokenExpiry)]);
                options.RefreshTokenExpiry = Convert.ToInt32(jwtAppSettingOptions[nameof(JwtSettings.RefreshTokenExpiry)]);
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512);
            });
            //Add JWT Authentication and bearer configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.RequireHttpsMetadata = true;
                configureOptions.SaveToken = true;
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtSettings.Issuer)];
                configureOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = _signingKey,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions[nameof(JwtSettings.Audience)],
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions[nameof(JwtSettings.Issuer)]
                };
            });
            //Add Automapper
            var mappingConfig = new MapperConfiguration(options => options.AddProfile(new AutoMapperProfilesApi()));
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}
