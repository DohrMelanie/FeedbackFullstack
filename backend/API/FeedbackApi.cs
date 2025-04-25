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

        var course = new CourseByFeedback(
            Name: courseEntity.Name,
            Code: courseEntity.Code
        );

        return Results.Ok(course);
    }

    public static async Task<IResult> PostFeedback(PostFeedbackDto req, ApplicationDataContext context)
    {
        var courseEntity = await context.Courses.FirstOrDefaultAsync(c => c.Code == req.CourseCode);
        if (courseEntity == null)
            return Results.NotFound();
        courseEntity.Feedbacks.Add(new Feedback
        {
            Course = courseEntity,
            Helpful = req.Helpful,
            Satisfied = req.Satisfied,
            Knowledgeable = req.Knowledgeable,
            LikedMost = req.LikedMost,
            LikedLeast = req.LikedLeast,
        });
        await context.SaveChangesAsync();
        return Results.Created();
    }

    public record CourseByFeedback(
        string Name,
        string Code
    );

    public record PostFeedbackDto(
        string CourseCode,
        int Helpful,
        int Satisfied,
        int Knowledgeable,
        string LikedMost,
        string LikedLeast
    );
}