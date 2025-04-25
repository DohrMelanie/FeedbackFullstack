using DataAccess;

namespace API;

public class ValidationHelpers
{
    public static Dictionary<string, string[]> ValidateAddFeedbackDto(FeedbackApi.PostFeedbackDto feedback)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(feedback.FeedbackCode))
        {
            errors[nameof(feedback.FeedbackCode)] = ["Feedback Code is required"];
        }
        if (feedback.Helpful < 1 || feedback.Helpful > 10)
        {
            errors[nameof(feedback.Helpful)] = ["Helpful must be between 1 and 10"];
        }

        if (feedback.Satisfied < 1 || feedback.Satisfied > 10)
        {
            errors[nameof(feedback.Satisfied)] = ["Satisfied must be between 1 and 10"];
        }

        if (feedback.Knowledgeable < 1 || feedback.Knowledgeable > 10)
        {
            errors[nameof(feedback.Knowledgeable)] = ["Knowledgeable must be between 1 and 10"];
        }

        if (feedback.LikedMost.Length > 500)
        {
            errors[nameof(feedback.LikedMost)] = ["Liked Most must be less than 500 characters"];
        }

        if (feedback.LikedLeast.Length > 500)
        {
            errors[nameof(feedback.LikedLeast)] = ["Liked Least must be less than 500 characters"];
        }

        return errors;
    }
    
    public static Func<EndpointFilterInvocationContext, EndpointFilterDelegate, ValueTask<object?>> GetEndpointFilter<T>(
        Func<T, Dictionary<string, string[]>> validationResult)
    {
        return async (context, next) =>
        {
            var computer = context.GetArgument<T>(0);
            var errors = validationResult(computer);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            return await next(context);
        };
    }
}