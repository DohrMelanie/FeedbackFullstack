using System.Globalization;
using DataAccess;

namespace CodeGenerator;

public class Program
{

    public static void Main(string[] args)
    {
        try
        {
            var factory = new ApplicationDataContextFactory();
            var context = factory.CreateDbContext([]);
            var usedCourseCodes = context.Courses.Select(c => c.Code).ToList();

            var existingFeedbackCodes = context.Courses
                .ToList()
                .SelectMany(c => c.FeedbackCodes)
                .ToList();
            
            var (courseCode, courseName, deadline, serverName, participants) = CheckArguments(args);
            
            if (usedCourseCodes.Contains(courseCode))
            {
                Console.WriteLine($"Error: Feedback URLs for course '{courseCode}' already generated.");
                return;
            }

            var secretCourseCode = GenerateUniqueCode();

            Console.WriteLine($"Secret Course Code: {secretCourseCode}");
            Console.WriteLine($"Generating {participants} feedback URLs for course '{courseName}' (Code: {courseCode})...");

            var feedbackCodes = new List<string>();
            while (feedbackCodes.Count < participants)
            {
                var code = GenerateUniqueCode();
                if (!feedbackCodes.Contains(code) && !existingFeedbackCodes.Contains(code))
                    feedbackCodes.Add(code);
            }

            var i = 1;
            foreach (var code in feedbackCodes)
            {
                Console.WriteLine($"Participant {i++}: https://{serverName}/feedback/{code}");
            }
            var course = new Course
            {
                Code = courseCode,
                Name = courseName,    
                SecretCode = secretCourseCode,
                Deadline = deadline,
                Participants = participants,
                IsOpen = true,
                FeedbackCodes = feedbackCodes.ToList()
            };
            
            context.Courses.Add(course);
            context.SaveChanges();
            
            Console.WriteLine($"Course '{courseCode}' successfully saved to database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
    }
    
    public static (string courseCode, string courseName, DateTime deadline, string serverName, int participants) CheckArguments(string[] args)
    {
        if (args.Length < 4)
        {
            throw new ArgumentException("Not enough arguments. Usage: <courseCode> <courseName> <deadline> <servername> [participants]");
        }

        var courseCode = args[0];
        var courseName = args[1];
        var deadlineInput = args[2];
        var serverName = args[3];
        var participants = args.Length >= 5 ? int.Parse(args[4]) : 30;

        if (courseCode.Length > 20 || courseName.Length > 200)
        {
            throw new ArgumentException("courseCode must be <= 20 characters and courseName <= 200 characters.");
        }

        if (!DateTime.TryParseExact(deadlineInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var deadline))
        {
            throw new ArgumentException("deadline must be in format yyyy-MM-dd.");
        }

        if (participants < 1 || participants >= 100)
        {
            throw new ArgumentException("participants must be between 1 and 99.");
        }

        if (serverName.Length > 40)
        {
            throw new ArgumentException("serverName must be <= 40 characters.");
        }

        return (courseCode, courseName, deadline, serverName, participants);
    }

    public static string GenerateUniqueCode()
    {
        var rnd = new Random(Guid.NewGuid().GetHashCode());
        const string letters = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";

        var chars = new string(Enumerable.Range(0, 6).Select(_ => letters[rnd.Next(letters.Length)]).ToArray());
        var nums = new string(Enumerable.Range(0, 2).Select(_ => digits[rnd.Next(digits.Length)]).ToArray());
        return chars + nums;
    }
}