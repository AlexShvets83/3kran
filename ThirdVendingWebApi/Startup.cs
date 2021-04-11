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
using System.IO;
using System.Reflection;
using System.Text.Json;
using ThirdVendingWebApi.Services;

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

      //services.AddIdentityCore<ApplicationUser>().AddEntityFrameworkStores<IdentityDbContext<ApplicationUser>>(); //.AddDefaultTokenProviders();
      //services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<IdentityDbContext<ApplicationUser>>(); //.AddDefaultTokenProviders();
      //services.AddScoped<IUserStore<ApplicationUser>>();

      //services.ConfigureApplicationCookie(options =>
      //{
      //  options.
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

          //x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          //x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          // false - SSL при отправке токена не используется
          options.RequireHttpsMetadata = false;

          options.TokenValidationParameters = new TokenValidationParameters
          {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,

            // строка, представляющая издателя
            ValidIssuer = AuthOptions.JwtIssuer,

            // будет ли валидироваться потребитель токена
            ValidateAudience = true,

            // установка потребителя токена
            ValidAudience = AuthOptions.JwtAudience,

            // будет ли валидироваться время существования
            ValidateLifetime = true,

            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

            // валидация ключа безопасности
            ValidateIssuerSigningKey = true, LifetimeValidator = CustomLifetimeValidator

            //ClockSkew =  
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
        options.LoginPath = "/#/";

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

      //services.Configure<ForwardedHeadersOptions>(options => { options.KnownProxies.Add(IPAddress.Parse("10.0.0.100")); });
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

        //app.UseSwagger();
        //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ThirdVendingWebApi v1"));
      }

      //app.UseHttpsRedirection();
      //app.UseRouting();
      //app.UseAuthorization();
      //app.UseHttpsRedirection();

      app.UseRouting();
      app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
      app.UseForwardedHeaders(new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
      app.UseResponseCompression();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ThirdVendingWebApi v1");
        c.DocExpansion(DocExpansion.List);
        c.RoutePrefix = "api/doc"; //string.Empty;
      });

      //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapRazorPages();
      });
    }
  }
}
