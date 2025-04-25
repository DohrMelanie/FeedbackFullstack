using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class FeedbackApi
{
    public static async Task<IResult> GetCourseByFeedbackCode(string feedbackCode, ApplicationDataContext context)
    {
        var courseEntity = await context.Courses
            .FirstOrDefaultAsync(c => c.FeedbackCodes.Contains(feedbackCode));

        if (courseEntity == null)
            return Results.NotFound();

        var course = new Course(
            Name: courseEntity.Name,
            Code: courseEntity.Code
        );

        return Results.Ok(course);
    }

    public record Course(
        string Name,
        string Code
    );
}