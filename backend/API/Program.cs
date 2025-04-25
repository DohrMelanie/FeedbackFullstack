using API;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:4200", "http://localhost:4300", "http://localhost:51566", "http://localhost:5005");
            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseCors("AllowOrigins");

app.UseHttpsRedirection();

var apiApp = app.MapGroup("/api");
apiApp.MapGet("/course/feedbackcode/{feedbackCode}", FeedbackApi.GetCourseByFeedbackCode)
    .Produces<FeedbackApi.CourseByFeedback>()
    .Produces(StatusCodes.Status404NotFound);

apiApp.MapPost("/feedback", FeedbackApi.PostFeedback)
    .AddEndpointFilter(ValidationHelpers.GetEndpointFilter<FeedbackApi.PostFeedbackDto>(ValidationHelpers.ValidateAddSoftwareDto))
    .Accepts<FeedbackApi.PostFeedbackDto>("application/json")
    .Produces(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status404NotFound);

app.Run();
