namespace Super_Duper_Library_System.Models
{
    public class Borrows
    {
        public int BorrowId { get; set; }
        public int StudentId { get; set; }
        public DateTime TakeDate { get; set; }
        public DateTime BroughtDate { get; set; }
    }
}
