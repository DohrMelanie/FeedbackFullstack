namespace DataAccess;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string SecretCode { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int Participants { get; set; }
    public bool IsOpen { get; set; }
    public List<string> FeedbackCodes { get; set; } = [];
    public List<Feedback> Feedbacks { get; set; } = [];
}

public class Feedback
{
    public int Id { get; set; }
    public int? CourseId { get; set; } = null;
    public Course Course { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
}