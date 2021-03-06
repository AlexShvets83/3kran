using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using CommonVending.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace ThirdVendingWebApi
{
  public class Startup
  {
    public Startup(IConfiguration configuration) { Configuration = configuration; }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors();
      
      //services.AddControllers().AddNewtonsoftJson(options =>
      //{
      //  options.SerializerSettings.ContractResolver = new DefaultContractResolver
      //  {
      //    //NamingStrategy = new DefaultNamingStrategy { OverrideSpecifiedNames = true }
      //    NamingStrategy = new CamelCaseNamingStrategy() { OverrideSpecifiedNames = true },
      //    IgnoreIsSpecifiedMembers = false
      //  };
      //  options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      //}).AddXmlSerializerFormatters();
      services.AddRazorPages();
      services.AddControllers()
        .AddJsonOptions(options =>
        {
          JsonConvert.DefaultSettings = () => new JsonSerializerSettings();
          options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

          //options.JsonSerializerOptions.DictionaryKeyPolicy = null;
          options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

          //options.JsonSerializerOptions.IgnoreNullValues = true;
        })
        .AddXmlSerializerFormatters();

      var connStr = MainSettings.Settings.ConnectionStrings.DefaultConnection;
      services.AddDbContext<IdentityDbContext<ApplicationUser>>(options => options.UseSnakeCaseNamingConvention().UseNpgsql(connStr));

      services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<IdentityDbContext<ApplicationUser>>(); //.AddDefaultTokenProviders();

      // Add application services.
      services.AddTransient<IEmailSender, AuthMessageSender>();
      services.AddTransient<ISmsSender, AuthMessageSender>();

      services.Configure<IdentityOptions>(options =>
      {
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        // User settings
        options.User.RequireUniqueEmail = true;
      });

      //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      services.AddAuthentication(x =>
        {
          x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
          x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          // false - SSL ??? ???????? ?????? ?? ????????????
          options.RequireHttpsMetadata = false;

          options.TokenValidationParameters = new TokenValidationParameters
          {
            // ?????????, ????? ?? ?????????????? ???????? ??? ????????? ??????
            ValidateIssuer = true,

            // ??????, ?????????????? ????????
            ValidIssuer = AuthOptions.JwtIssuer,

            // ????? ?? ?????????????? ??????????? ??????
            ValidateAudience = true,

            // ????????? ??????????? ??????
            ValidAudience = AuthOptions.JwtAudience,

            // ????? ?? ?????????????? ????? ?????????????
            ValidateLifetime = true,

            // ????????? ????? ????????????
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

            // ????????? ????? ????????????
            ValidateIssuerSigningKey = true, LifetimeValidator = CustomLifetimeValidator
          };
        });

      services.ConfigureApplicationCookie(options =>
      {
        // Cookie settings
        options.Cookie.HttpOnly = true;

        //options.Cookie.Expiration = TimeSpan.FromDays(150);
        options.ExpireTimeSpan = TimeSpan.FromMinutes(600);

        // If the LoginPath isn't set, ASP.NET Core defaults 
        // the path to /Account/Login.
        options.LoginPath = "/Account/Login";

        // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
        // the path to /Account/AccessDenied.
        options.AccessDeniedPath = "/accessdenied";
        options.SlidingExpiration = true;
      });

      //services.ConfigureApplicationCookie(options =>
      //{
      //  options.AccessDeniedPath = "/Identity/Account/AccessDenied";
      //  options.Cookie.Name = "YourAppCookieName";
      //  options.Cookie.HttpOnly = true;
      //  options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
      //  options.LoginPath = "/Identity/Account/Login";
      //  // ReturnUrlParameter requires 
      //  //using Microsoft.AspNetCore.Authentication.Cookies;
      //  options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
      //  options.SlidingExpiration = true;
      //});

      services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);

      services.AddResponseCompression(options =>
      {
        options.MimeTypes = new[]
        {
          // Default
          "text/plain", "text/css", "application/javascript", "text/html",
          "application/xml", "text/xml", "application/json", "text/json",

          // Custom
          "image/svg+xml", "application/x-msgpack", "application/msgpack"
        };
        options.EnableForHttps = true;
      });

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo {Title = "ThirdVendingWebApi", Version = "v1"});

        c.AddSecurityDefinition("Bearer",
                                new OpenApiSecurityScheme
                                {
                                  Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"", Name = "Authorization",
                                  In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey, Scheme = "Bearer", BearerFormat = "JWT"
                                });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {new OpenApiSecurityScheme {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}}, new string[] { }}
        });

        //Locate the XML file being generated by ASP.NET...
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
      });
      
      services.Configure<FormOptions>(o =>
      {
        o.ValueLengthLimit = int.MaxValue;
        o.MultipartBodyLengthLimit = long.MaxValue;
        o.MemoryBufferThreshold = int.MaxValue;
      });
    }

    private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
      if (expires != null) { return DateTime.UtcNow < expires; }

      return false;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      //app.UseHttpsRedirection();

      app.UseRouting();
      app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
      app.UseForwardedHeaders(new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
      app.UseResponseCompression();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseDefaultFiles();

      //app.UseStaticFiles();

      const int cachePeriod = 24 * 60 * 60; // seconds
      app.UseStaticFiles(new StaticFileOptions
      {
        OnPrepareResponse = ctx =>
        {
          //ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
          ctx.Context.Response.Headers.Append(new KeyValuePair<String, StringValues>("Cache-Control", $"public, max-age={cachePeriod}"));
        }
      });

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ThirdVendingWebApi v1");
        c.DocExpansion(DocExpansion.List);
        c.RoutePrefix = "api/doc";
      });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapRazorPages();
      });
    }
  }
}
