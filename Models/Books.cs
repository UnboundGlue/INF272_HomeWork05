namespace Super_Duper_Library_System.Models
{
    public class Books
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public int PageCount { get; set; }
        public int Point { get; set; }
        public int AuthorId { get; set; }

        public int TypeId { get; set; }
    }
}
