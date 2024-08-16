namespace MovieLibrary.DTOs
{
    public class CommentDTO
    {
        public int id {  get; set; }
        public string Body { get; set; } = null!;
        public int MovieId { get; set; }

    }
}
