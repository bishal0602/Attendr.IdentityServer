using Attendr.IdentityServer.DbContexts;
using Attendr.IdentityServer.Entities;
using Attendr.IdentityServer.Models.Email;
using Attendr.IdentityServer.Services;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
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

                //options.Discovery.CustomEntries.Add("registration_endpoint", "~/account/register");
                //options.Discovery.CustomEntries.Add("verification_endpoint", "~/account/verify");
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
             .AddProfileService<ProfileService>();
        //.AddTestUsers(TestUsers.Users);

        builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
        builder.Services.AddTransient<IProfileService, ProfileService>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        //app.UseStaticFiles();
        //app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        //app.UseAuthorization();
        //app.MapRazorPages().RequireAuthorization();
        app.MapControllers();

        return app;
    }
}
