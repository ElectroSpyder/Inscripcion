namespace ADJInsc.Core
{    
    using ADJInsc.Core.Service;
    using ADJInsc.Core.Service.Interface;
    using ADJInsc.Models.Models.DBInsc;
    using ADJInsc.Models.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System.Globalization;
    using Wkhtmltopdf.NetCore;
    using xa.App.Services;

    //using xa.App.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var cultureInfo = new CultureInfo("es-ES"); // Espa�ol Espa�a
            cultureInfo.NumberFormat.NumberGroupSeparator = "."; // Asegura que use el punto

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            services.AddControllersWithViews();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 15728640; // 15 MB (cambiar seg�n necesidad)
            });

            services.AddWkhtmltopdf();

            services.AddCors();
            // Add MVC services to the services container.
            services.AddMvc();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<UrlBase>(Configuration.GetSection("UrlBase"));

            services.AddDbContext<dbActuContext>(cfg =>
            {
                //cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));   Produccion
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));     //  Base Test 
            });
                        
            services.AddHttpClient();
            //services.AddTransient<IMailService, MailService>();
            services.AddTransient<IApiService, ApiServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.UseHttpsRedirection();

            Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "..\\Rotativa\\Windows\\");

            app.UseStaticFiles();

            app.UseRouting();

           // app.UseAuthorization();
 // app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseSession();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Inscripcion}/{action=Inscripcion}/{id?}");
            });
        }
    }
}
