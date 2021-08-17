using System;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CourseLibrary.API
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
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers(setupOptions =>
                {
                    setupOptions.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson(builder =>
                {
                    builder.UseCamelCasing(true);
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemFactory = context.HttpContext.RequestServices
                            .GetRequiredService<ProblemDetailsFactory>();
                        var problemDetails = problemFactory.CreateValidationProblemDetails(
                            context.HttpContext, context.ModelState);
                        problemDetails.Detail = "see error fields for details";
                        problemDetails.Instance = context.HttpContext.Request.Path;
                        var actionExecutingContext =
                            context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
                        if (context.ModelState.ErrorCount > 0 &&
                            (context is ControllerContext ||
                            actionExecutingContext?.ActionArguments.Count ==
                            context.ActionDescriptor.Parameters.Count))
                        {
                            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                            problemDetails.Title = "one or mode validation errors occurred";
                            problemDetails.Type = "http://courselibrary.com/modelvalidation";
                            return new UnprocessableEntityObjectResult(problemDetails)
                            {
                                ContentTypes = {"application/problem+json"}
                            };
                        }
                        problemDetails.Status = StatusCodes.Status400BadRequest;
                        problemDetails.Title = "one or mode  errors occurred";
                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });
             
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            }); 
            
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
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An error occurred. Please try later");
                    });
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
