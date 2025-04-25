using System.Globalization;
using DataAccess;

namespace CodeGenerator;

class Program
{
    const string ServerUrl = "https://localhost:5000/feedback/";

    public static void Main(string[] args)
    {
        try
        {
            var factory = new DataAccess.ApplicationDataContextFactory();
            var context = factory.CreateDbContext([]);
            var usedCourseCodes = context.Courses.Select(c => c.Code).ToList();

            if (args.Length < 3)
            {
                Console.WriteLine("Usage: <courseCode> <courseName> <deadline> [participants]");
                return;
            }

            var courseCode = args[0];
            var courseName = args[1];
            var deadlineInput = args[2];
            var participants = args.Length >= 4 ? int.Parse(args[3]) : 30;

            if (courseCode.Length > 20 || courseName.Length > 200)
            {
                Console.WriteLine("Error: courseCode must be <= 20 characters and courseName <= 200 characters.");
                return;
            }

            if (!DateTime.TryParseExact(deadlineInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deadline))
            {
                Console.WriteLine("Error: deadline must be in format yyyy-MM-dd.");
                return;
            }

            if (participants < 1 || participants >= 100)
            {
                Console.WriteLine("Error: participants must be between 1 and 99.");
                return;
            }

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
                if (!feedbackCodes.Contains(code))
                    feedbackCodes.Add(code);
            }

            int i = 1;
            foreach (var code in feedbackCodes)
            {
                Console.WriteLine($"Participant {i++}: {ServerUrl}{code}");
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

    static string GenerateUniqueCode()
    {
        var rnd = new Random(Guid.NewGuid().GetHashCode());
        const string letters = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";

        var chars = new string(Enumerable.Range(0, 6)
            .Select(_ => letters[rnd.Next(letters.Length)])
            .ToArray());
        var nums = new string(Enumerable.Range(0, 2)
            .Select(_ => digits[rnd.Next(digits.Length)])
            .ToArray());
        return chars + nums;
    }
}