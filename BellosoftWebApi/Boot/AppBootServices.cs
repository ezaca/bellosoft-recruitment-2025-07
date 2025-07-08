using BellosoftWebApi.Facades;
using BellosoftWebApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BellosoftWebApi.Boot
{
    internal class AppBootServices
    {
        public const string DefaultConnection = "DefaultConnection";
        public const string LoginPath = "/auth/login";
        public const string LogoutPath = "/auth/logout";
        public const double HoursToExpireSession = 1.0;

        private WebApplicationBuilder builder;

        public AppBootServices(WebApplicationBuilder builder)
        {
            this.builder = builder;
        }

        public void AddDatabase()
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString(DefaultConnection)));
        }

        public void AddAuth()
        {
            builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", SetupAuthCookie);
            builder.Services.AddAuthorization();
        }

        private void SetupAuthCookie(CookieAuthenticationOptions options)
        {
            options.LoginPath = LoginPath;
            options.LogoutPath = LogoutPath;
            options.ExpireTimeSpan = TimeSpan.FromHours(HoursToExpireSession);
            options.SlidingExpiration = true;

            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        }

        public void AddControllers()
        {
            builder.Services.AddControllers();
        }

        public void AddSwagger()
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        public void AddInfrastructureServices()
        {
            builder.Services.AddHttpContextAccessor();
        }

        public void AddApplicationServices()
        {
            builder.Services.AddScoped<IAuthenticatedUser, AuthenticatedUserService>();
        }
    }
}
