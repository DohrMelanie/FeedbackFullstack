using System.Text.RegularExpressions;
using CodeGenerator;

namespace UnitTests
{
    public class CodeGenerationTests
    {
        [Fact]
        public void GenerateUniqueCode_ReturnsValidFormat()
        {
            var regex = new Regex("^[a-z]{6}[0-9]{2}$");
            
            var result = Program.GenerateUniqueCode();
            
            Assert.Matches(regex, result);
            Assert.Equal(8, result.Length);
        }
        
        [Fact]
        public void GenerateUniqueCode_ReturnsDifferentValues()
        {
            var generatedCodes = new HashSet<string>();
            for (var i = 0; i < 100; i++)
            {
                generatedCodes.Add(Program.GenerateUniqueCode());
            }
            Assert.Equal(100, generatedCodes.Count);
        }
        
        [Fact]
        public void CheckArguments_WithValidInput_ReturnsExpectedValues()
        {
            string[] args = ["PROG123", "Programming Course", "2023-12-31", "server", "25"];
            var expectedDate = new DateTime(2023, 12, 31);
            var result = Program.CheckArguments(args);
            
            Assert.Equal("PROG123", result.courseCode);
            Assert.Equal("Programming Course", result.courseName);
            Assert.Equal(expectedDate, result.deadline);
            Assert.Equal(25, result.participants);
        }
        
        [Fact]
        public void CheckArguments_WithDefaultParticipants_Returns30()
        {
            string[] args = ["PROG123", "Programming Course", "2023-12-31", "server"];
            var result = Program.CheckArguments(args);
            
            Assert.Equal(30, result.participants);
        }
        
        [Theory]
        [InlineData("NotEnoughArgs", "Not enough arguments")]
        [InlineData("TooLongCourseCode", "courseCode must be")]
        [InlineData("TooLongCourseName", "and courseName")]
        [InlineData("InvalidDateFormat", "deadline must be")]
        [InlineData("ZeroParticipants", "participants must be")]
        [InlineData("TooManyParticipants", "participants must be")]
        public void CheckArguments_WithInvalidInput_ThrowsArgumentException(string testCase, string expectedErrorMessage)
        {
            string[] args = testCase switch
            {
                "NotEnoughArgs" => ["PROG123"],
                "TooLongCourseCode" => [new string('A', 21), "Name", "2023-12-31", "server"],
                "TooLongCourseName" => ["PROG123", new string('A', 201), "2023-12-31", "server"],
                "InvalidDateFormat" => ["PROG123", "Name", "31.12.2023", "server"],
                "ZeroParticipants" => ["PROG123", "Name", "2023-12-31", "server", "0"],
                "TooManyParticipants" => ["PROG123", "Name", "2023-12-31", "server", "100"],
                _ => []
            };
            var exception = Assert.Throws<ArgumentException>(() => Program.CheckArguments(args));
            Assert.Contains(expectedErrorMessage, exception.Message);
        }
    }
}