using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
    
    public static async Task<IResult> GetFeedbackFromCourse(string courseCode, string secretCourseCode, ApplicationDataContext context)
    {
        if (!context.Courses.Any(c => c.Code == courseCode && c.SecretCode == secretCourseCode))
        {
            return Results.NotFound();
        }
        
        var courses = await context.Courses
            .Include(c => c.Feedbacks)
            .ToListAsync();

        var result = courses.Select(course => 
        {
            var hasFeedback = course.Feedbacks.Any();
            return new FeedbackOverviewDto(
                CourseCode: course.Code,
                CourseName: course.Name,
                FeedbackStatus: DateTime.UtcNow <= course.Deadline ? "Open" : "Stopped",
                NumberOfParticipants: course.Participants,
                NumberOfFeedbacksSubmitted: course.Feedbacks.Count,
                AverageRatings: hasFeedback ? new AverageRatingDto(
                    Helpful: course.Feedbacks.Average(f => f.Helpful),
                    Satisfied: course.Feedbacks.Average(f => f.Satisfied),
                    Knowledgeable: course.Feedbacks.Average(f => f.Knowledgeable)
                ) : null,
                FeedbackComments: hasFeedback ? new FeedbackCommentsDto(
                    LikedMostComments: course.Feedbacks
                        .Select(f => f.LikedMost)
                        .Where(comment => comment != null)
                        .Select(comment => comment!)
                        .ToList(),
                    LikedLeastComments: course.Feedbacks
                        .Select(f => f.LikedLeast)
                        .Where(comment => comment != null)
                        .Select(comment => comment!)
                        .ToList()
                ) : null
            );
        }).ToList();

        return Results.Ok(result);
    }

    public static async Task<IResult> PostFeedback(PostFeedbackDto req, ApplicationDataContext context)
    {
        try
        {
            var courseEntity = await context.Courses
                .Include(c => c.Feedbacks)
                .FirstOrDefaultAsync(c => c.FeedbackCodes.Contains(req.FeedbackCode));

            if (courseEntity == null)
                return Results.BadRequest(new { error = "Invalid feedback request" });

            var alreadyUsed = courseEntity.Feedbacks.Any(f => f.FeedbackCode == req.FeedbackCode);
            if (alreadyUsed || DateTime.UtcNow > courseEntity.Deadline)
                return Results.BadRequest(new { error = "Invalid feedback request" });

            courseEntity.Feedbacks.Add(new Feedback
            {
                Course = courseEntity,
                CourseId = courseEntity.Id,
                Helpful = req.Helpful,
                Satisfied = req.Satisfied,
                Knowledgeable = req.Knowledgeable,
                LikedMost = req.LikedMost.Length == 0 ? null : req.LikedMost,
                LikedLeast = req.LikedLeast.Length == 0 ? null : req.LikedLeast,
                FeedbackCode = req.FeedbackCode 
            });

            await context.SaveChangesAsync();
            return Results.Created();
        }
        catch (Exception)
        {
            return Results.BadRequest(new { error = "Invalid feedback request" });
        }
    }

    public record CourseByFeedback(
        string Name,
        string Code
    );

    public record PostFeedbackDto(
        string FeedbackCode,
        int Helpful,
        int Satisfied,
        int Knowledgeable,
        string LikedMost,
        string LikedLeast
    );

    public record FeedbackOverviewDto(
        string CourseCode,
        string CourseName,
        string FeedbackStatus,
        int NumberOfParticipants,
        int NumberOfFeedbacksSubmitted,
        AverageRatingDto? AverageRatings,
        FeedbackCommentsDto? FeedbackComments
    );

    public record AverageRatingDto(
        double Helpful,
        double Satisfied,
        double Knowledgeable
    );

    public record FeedbackCommentsDto(
        List<string> LikedMostComments,
        List<string> LikedLeastComments
    );
}