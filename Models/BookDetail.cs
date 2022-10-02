namespace Super_Duper_Library_System.Models
{
    public class BookDetail
    {
        public int borrowId { get; set; }   
        public DateTime takenDate { get; set; }
        public  DateTime? BroughtDate { get; set; } 
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
    }
}
