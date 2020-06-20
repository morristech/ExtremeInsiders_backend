using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using ExtremeInsiders.Areas;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ExtremeInsiders
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      var builder = new ConfigurationBuilder().AddJsonFile("config.json").AddConfiguration(configuration);

      Configuration = builder.Build();
    }

    private IConfiguration Configuration { get; }


    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors();
      services.AddHttpContextAccessor();
      services.AddControllersWithViews();

      services.AddDbContext<Data.ApplicationContext>(options =>
      {
        var connect = Configuration.GetConnectionString("Default");
        options.UseLazyLoadingProxies();
        options.UseNpgsql(connect);
      });

      var key = services.ConfigureJwt(Configuration);

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(x =>
        {
          x.RequireHttpsMetadata = false;
          x.SaveToken = true;
          x.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
          };
        })
        .AddCookie(x =>
        {
          x.LoginPath = new PathString("/admin/auth/login");
          // x.LogoutPath = new PathString("/admin/auth/logout");
        });

      services.AddSingleton<IPasswordHasher<User>, PasswordHasherService>();
      services.AddTransient<ImageService>();

      services.AddScoped<UserService>();

      services.AddScoped<SocialAuthService, FacebookSocialAuthService>();
      services.AddScoped<SocialAuthService, GoogleSocialAuthService>();
      services.AddScoped<SocialAuthService, VkSocialAuthService>();
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
        app.UseHsts();
      }

      //app.UseHttpsRedirection();
      app.UseStaticFiles();

      
      app.UseAuthentication();
      app.UseRouting();
      app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

      app.UseAuthorization();
      
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();

        endpoints.MapControllerRoute(
          name: "Admin",
          pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        
        // endpoints.MapAreaControllerRoute(
        //   name: "AdminArea",
        //   areaName: "Admin",
        //   pattern: "{area:exists}/{controller=Index}/{action=Index}/{id?}");
      });
    }
  }
}