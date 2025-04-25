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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var apiApp = app.MapGroup("/api");
apiApp.MapGet("/course/feebackcode/{feedbackCode}", FeedbackApi.GetCourseByFeedbackCode)
    .Produces<FeedbackApi.Course>()
    .Produces(StatusCodes.Status404NotFound);

app.Run();
