using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Claim.Data;
using Claim.Data.Enties;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Model;

namespace Train
{
    public class Startup
    {
        private readonly string _loginOrigin="_localorigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Train", Version = "v1"});//,Description="This test description"
                // c.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
                // {
                //     In = ParameterLocation.Header,
                //     Description = "Please insert token",
                //     Name = "Authorization",
                //     Type = SecuritySchemeType.Http,
                //     BearerFormat = "JWT",
                //     Scheme = "bearer"
                // });
                // c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                //     {
                //         new OpenApiSecurityScheme{
                //         Reference = new OpenApiReference{
                //             Type=ReferenceType.SecurityScheme,
                //             Id="Bearer"
                //         }
                //     },
                //     new string[]{}
                // }
                // });
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory,xmlFile);
                // c.IncludeXmlComments(xmlPath);
            });
            //...
            services.Configure<JWTConfig>(Configuration.GetSection("JWTConfig"));

            //add AddDBContext
            services.AddDbContext<AppDBContext>(opt=>{
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            //...
            services.AddIdentity<AppUser,IdentityRole>(opt=>{}).AddEntityFrameworkStores<AppDBContext>();
            
            // Add Authentication JWT
            services.AddAuthentication(x=>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                    var key = Encoding.ASCII.GetBytes(Configuration["JWTConfig:Key"]);
                    var issuer = Configuration["JWTConfig:Issuer"];
                    var audience = Configuration["JWTConfig:Audience"];
                    // options.RequireHttpsMetadata = false;
                    // options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        RequireExpirationTime = true,
                        ValidAudience = issuer,
                        ValidIssuer = audience
                    };
            });

            services.AddCors(opt=>{
                opt.AddPolicy(_loginOrigin,builder=>{
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Train v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors(_loginOrigin);

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
