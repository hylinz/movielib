namespace MovieLibrary.Entities
{
    public class Error
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? StackTrace { get; set; }
        public string Message { get; set; } = null!;
    }
}
