using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using netcore_rest_api.Configuration;
using netcore_rest_api.Data;

namespace netcore_rest_api {

  public class Startup {
    public IConfiguration config { get; }

    public Startup(IConfiguration configuration) {
      config = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.Configure<IdentityOptions>(options => {
        options.Password.RequiredLength = 4;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
      });
      services.AddDbContext<ApiDbContext>(options =>
        options.UseSqlite(config.GetConnectionString("Default")));
      services.Configure<JwtConfig>(config.GetSection("JwtConfig"));
      services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(options => {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JwtConfig:Secret"])),
          ValidateLifetime = true,
          RequireExpirationTime = false, // TODO: change to true -- don't expire during testing 
          ValidateIssuer = false, // TODO: change to true
          ValidateAudience = false, // TODO: change to true
        };
      });
      services.AddDefaultIdentity<IdentityUser>(options => {
        options.SignIn.RequireConfirmedAccount = true;
      }).AddEntityFrameworkStores<ApiDbContext>();
      services.AddControllers();
      // services.AddLogging();
      services.AddSwaggerGen(options => {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "netcore_rest_api", Version = "v1" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseSwagger(); /* swagger/index.html */
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "netcore_rest_api v1"));
      }

      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}
