using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SistemaEncomiendas.Core.Data;
using SistemaEncomiendas.Core.Data.Repositories;
using SistemaEncomiendas.Core.Providers;
using SistemaEncomiendas.Core.Services;
using SistemaEncomiendas.Data;
using SistemaEncomiendas.Data.Repositories;
using SistemaEncomiendas.Helpers;
using SistemaEncomiendas.Services.Providers;
using SistemaEncomiendas.Services.Services;

namespace SistemaEncomiendas
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration config, ILogger<Startup> logger)
        {
            Configuration = config;
            _logger = logger;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-ES");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("es-ES") };
                options.RequestCultureProviders.Clear();
            });

            services.AddCors();
            services.AddAutoMapper();
            services.AddMvc();
            string connString = Configuration.GetConnectionString("DefaultConnection");

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/Cuenta/Login";
                options.AccessDeniedPath = "/Cuenta/PermisoDenegado";
            });

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDbContext<EncomiendasContext>(options => options.UseSqlServer(connString).EnableSensitiveDataLogging());

            services.AddScoped<DbContext, EncomiendasContext>();

            services.AddScoped<ICifradoProvider, CifradoProvider>();

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            services.AddScoped<IPaqueteRepository, PaqueteRepository>();

            services.AddScoped<IUsuariosService, UsuariosServices>();

            services.AddScoped<IPaqueteService, PaquetesService>();

            services.AddScoped<IMultimediaUsuarioRepository, MultimediaUsuarioRepository>();

            services.AddScoped<IViewRenderService, ViewRenderService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                _logger.LogInformation("Estamos en modo desarrollo");
            }
            else
            {
                _logger.LogInformation("Estamos en modo produccion");
            }




            //app.UseRequestLocalization();

            //CultureInfo cultureInfo = new CultureInfo("es-ES") { NumberFormat = { CurrencySymbol = "$" } };
            //CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            //CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsProduction())
            {
                var defaultDateCulture = "es-ES";
                var ci = new CultureInfo(defaultDateCulture);

                ci.NumberFormat.NumberDecimalSeparator = ".";
                ci.NumberFormat.CurrencyDecimalSeparator = ".";
                ci.NumberFormat.CurrencySymbol = "$";
                CultureInfo.DefaultThreadCurrentCulture = ci;
                CultureInfo.DefaultThreadCurrentUICulture = ci;
                // Configure the Localization middleware
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(ci),
                    SupportedCultures = new List<CultureInfo>
                    {
                        ci,
                    },
                    SupportedUICultures = new List<CultureInfo>
                    {
                        ci,
                    }
                });
            }
            

            app.UseAuthentication();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
