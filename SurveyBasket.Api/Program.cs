using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using Serilog;
using SurveyBasket.Api.Services;
namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDependencies(builder.Configuration);

            //builder.Services.AddOutputCache(options =>
            //{
            //    options.AddPolicy("Polls", x =>
            //        x.Cache()
            //        .Expire(TimeSpan.FromSeconds(120))
            //        .Tag("availableQuestions")  
            //    );
            //});

            builder.Services.AddDistributedMemoryCache();

            //builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Host.UseSerilog((context, configuration) =>
            { 
                //configuration.MinimumLevel.Information().WriteTo.Console();

                //read from appsettings.json
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "SurveyBasket.v1"));    
            }
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = [
                    new  HangfireCustomBasicAuthenticationFilter{
                        User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
                        Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
                    }
                ],
                DashboardTitle = "Survey Basket Dashboard",
                //IsReadOnlyFunc = (DashboardContext context) => true
            });

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotifications(null), Cron.Daily);


            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            //app.UseMiddleware<Middleware.ExceptionHandlingMiddleware>();
            app.UseExceptionHandler();

            app.Run();
        }
    }
}
