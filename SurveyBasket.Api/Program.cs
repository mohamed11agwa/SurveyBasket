using Serilog;
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

            app.UseCors();
            app.UseAuthorization();

            //app.MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            //app.UseMiddleware<Middleware.ExceptionHandlingMiddleware>();
            app.UseExceptionHandler();

            app.Run();
        }
    }
}
