namespace MovieLibrary.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public int MovieId { get; set; }
    }
}
