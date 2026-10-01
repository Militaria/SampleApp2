namespace Sampleapp.Models
{
    public class ValidationError
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public Guid CorrelationId { get; set; }
    }

    public class ValidationSummary
    {
        public Guid CorrelationId { get; set; }
        public int Total { get; set; }
        public int Invalid { get; set; }
    }

    public class UserValidationException : Exception
    {
        public UserValidationException(string message) : base(message)
        {
        }
    }
}
