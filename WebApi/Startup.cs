using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Users.Common.Interfaces;
using Users.Repositories;
using Users.Services;
using Utils.Extentions;
using Utils.Encrypt;
using Token.Common.Interfaces;
using Token.Repositories;
using Token.Services;
using Users.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WebApi
{
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
            services.AddCors();
            var authSecretSection = Configuration.GetSection("AuthSecret");
            services.Configure<AuthSecret>(authSecretSection);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddControllers();
            var authSecret = authSecretSection.Get<AuthSecret>();
            var secretKey = authSecret.JwtSecret.ToBytes();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = Jwt
                  .GetAppTokenValidationParameters(secretKey);
            });

            services.AddDbContext<UserContext>(
                options => options.UseMySql(authSecret.ConnectionString,
                mySqlOptions =>
                {
                    mySqlOptions.ServerVersion(new Version(8,0,17), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql);
                })
            );

            services.AddStackExchangeRedisCache(options => {
                options.Configuration = "authentikate-cache";
                options.InstanceName = "some-name";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
