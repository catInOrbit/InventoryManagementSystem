using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BlazorShared;
using Infrastructure.Data;
using Infrastructure.Identity.DbContexts;
using Infrastructure.Logging;
using Infrastructure.Services;
using InventoryManagementSystem.ApplicationCore.Constants;
using InventoryManagementSystem.ApplicationCore.Entities;
using InventoryManagementSystem.ApplicationCore.Entities.Products;
using InventoryManagementSystem.ApplicationCore.Interfaces;
using InventoryManagementSystem.PublicApi.AuthorizationEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using WebApi.Helpers;

namespace InventoryManagementSystem.PublicApi
{
    public class Startup
    {
        private const string CORS_POLICY = "CorsPolicy";
        public IConfiguration Configuration { get; set; }

        // private const string LOCAL_IDENTITY = "IdentityConnection";
        // private const string HEROKUSQL = "MSSQLHeroku";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // public void ConfigureDevelopmentServices(IServiceCollection services)
        // {
        //     services.AddDbContext<IdentityAndProductDbContext>(options =>
        //         options.UseNpgsql(Configuration.GetConnectionString("Heroku"),  b => b.MigrationsAssembly("IMSPublicApi")));
        //     ConfigureServices(services);
        // }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddDbContext<IdentityAndProductDbContext>(options =>
            //     options.UseNpgsql(Configuration.GetConnectionString("Heroku"),  b => b.MigrationsAssembly("IMSPublicApi")));
            
            services.AddHostedService<RedisWorkerService>();
            
            services.AddDbContext<IdentityAndProductDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(ConnectionPropertiesConstant.MAIN_CONNECTION_STRING),  b => b.MigrationsAssembly("IMSPublicApi")));

            services.AddCors(c =>
            {
                c.AddPolicy(CORS_POLICY, options => options.AllowAnyOrigin());
            });

            var lockoutOptions = new LockoutOptions()
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20),
                MaxFailedAccessAttempts = 3
            };

            services.AddIdentity<ApplicationUser, IdentityRole>(
                    options =>
                    {
                        options.User.RequireUniqueEmail = true;
                        options.Lockout = lockoutOptions;
                    }
                )
                    .AddEntityFrameworkStores<IdentityAndProductDbContext>()
                    .AddRoles<IdentityRole>()
                    .AddDefaultTokenProviders();
            
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(1));
            
            //The AddScoped method registers the service with a scoped lifetime, the lifetime of a single request.
            // services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddElasticsearch(Configuration);
            services.AddScoped(typeof(IAsyncRepository<>), typeof(AppGlobalRepository<>));
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            services.AddScoped<ITokenClaimsService, IdentityTokenClaimService>();
            
            var baseUrlConfig = new BaseUrlConfiguration();
            Configuration.Bind(BaseUrlConfiguration.CONFIG_NAME, baseUrlConfig);
            services.AddScoped<IFileSystem, WebFileSystem>(x => new WebFileSystem($"{baseUrlConfig.WebBase}File"));

            services.AddMemoryCache();

            var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);

            services.AddMediatR(typeof(Product).Assembly);

            services.AddAutoMapper(typeof(Startup).Assembly);
            
            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options =>
            //     {
            //     });
            // services.ConfigureApplicationCookie(options =>
            // {
            //     options.Cookie.Name = "IMSCookie";
            //     options.Cookie.HttpOnly = true;
            //     options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            //     // options.LoginPath = "/Identity/Account/Login";
            //     // ReturnUrlParameter requires 
            //     //using Microsoft.AspNetCore.Authentication.Cookies;
            //     options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            //     options.SlidingExpiration = true;
            // });
            
            //Email Service
            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSendingService>();

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            //
            // services.ConfigureApplicationCookie(options =>
            // {
            //     // Cookie settings
            //     options.Cookie.HttpOnly = true;
            //     options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //
            //     // options.LoginPath = "/Identity/Account/Login";
            //     // options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            //     options.SlidingExpiration = true;
            // });
            //
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMS Public API", Version = "v1" });
                c.EnableAnnotations();
                c.SchemaFilter<CustomSchemaFilters>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
            
            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });
            
           
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );
            
            services.AddScoped<IAuthorizationHandler,
                AccountAuthorizationHandler>();
            
            services.AddSingleton<IUserSession, UserSessionService>();
            services.AddSignalR();
            
            services.AddTransient<IRedisRepository, RedisRepository>();

            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                ConfigurationOptions configuration = new ConfigurationOptions()
                {
                    AllowAdmin = true,
                    EndPoints = { { "redis-11726.c258.us-east-1-4.ec2.cloud.redislabs.com", 11726 }},
                    Password = "6FSKO6Yu0vT8A34f761k84gisHlI5GAt",
                    ConnectTimeout = 5000,
                    ConnectRetry = 5,
                    SyncTimeout = 5000,
                    AbortOnConnectFail = false,
                };

                return ConnectionMultiplexer.Connect(configuration);
            });
            
            services.AddScoped<INotificationService, NotificationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseMiddleware<JwtMiddleware>();

            // var cookiePolicyOptions = new CookiePolicyOptions
            // {
            //     MinimumSameSitePolicy = SameSiteMode.Strict,
            // };
            //
            // app.UseCookiePolicy(cookiePolicyOptions);
            app.UseCors(options => options.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c=> c.SerializeAsV2 = true);

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
                endpoints.MapHub<NotificationHub>("/notiHub");
            });

        }
    }
}
