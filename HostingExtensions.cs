using Attendr.IdentityServer.DbContexts;
using Attendr.IdentityServer.Entities;
using Attendr.IdentityServer.Models.Email;
using Attendr.IdentityServer.Services;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Attendr.IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        //IHttpContextAccessor register
        builder.Services.AddHttpContextAccessor();
        // Password Hasher
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddControllers();
        //builder.Services.AddLocalApiAuthentication();

        builder.Services.AddDbContext<AttendrDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseSqlServer(builder.Configuration["ConnectionStrings:AttendrIDPUsersDB"]);
        });

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddAutoMapper(typeof(Program));

        var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        builder.Services.AddSingleton(emailConfig);
        builder.Services.AddScoped<IEmailSender, EmailSender>();

        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
             .AddProfileService<ProfileService>();
        //.AddTestUsers(TestUsers.Users);

        builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
        builder.Services.AddTransient<IProfileService, ProfileService>();

        //builder.Services.AddCors(options =>
        //{
        //    options.AddPolicy("CorsPolicy", builder =>
        //        builder.AllowAnyOrigin()
        //       .AllowAnyMethod()
        //       .AllowAnyHeader());
        //});

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseIdentityServer();

        app.UseExceptionHandler(c => c.Run(async context =>
        {
            var exception = context.Features
                .Get<IExceptionHandlerPathFeature>()
                .Error;
            var response = new { error = exception.Message };
            await context.Response.WriteAsJsonAsync(response);
        }));

        //app.UseCors();

        app.MapControllers();

        return app;
    }
}
