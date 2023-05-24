namespace JobApplicationTrackerForStudents_Server.Dtos
{
    public class SignUpResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? Token { get; set; }
    }
}
