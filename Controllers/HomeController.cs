using Microsoft.AspNetCore.Mvc;
using Super_Duper_Library_System.Models;
using System.Diagnostics;
using Super_Duper_Library_System.Database;
using System.Data.SqlClient;

namespace Super_Duper_Library_System.Controllers
{
    public class HomeController : Controller
    {
        public class MyViewModel
        {
            public List<BookGeneralInfo> BookGeneralInfo { get; set; }
            public List<Types> Type { get; set; }

            public List<Author> Authors { get; set; }
        }

        public class MyBookViewModel
        {
            public List<BookDetail> BookDetail { get; set; }
            public String BookName { get; set; }
            public Boolean Status { get; set; }

            public int BookId { get; set; }
        }

        public class MyStudentViewModel
        {
            public List<String> Class { get; set; }
            public List<Student> Students { get; set; }

            public Boolean Status { get; set; }
            public int BookId { get; set; }
            public string BookName { get; set; }
            public int? BStudentId { get; set; }

        }


        private readonly ILogger<HomeController> _logger;
        private Service dataBase;
        private MyViewModel BookGeneralModel = new MyViewModel();
        private static MyStudentViewModel myStudentView = new MyStudentViewModel();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            dataBase = new Service();
            BookGeneralModel.BookGeneralInfo = dataBase.getBookGeneral();
            BookGeneralModel.Type = dataBase.getType();
            BookGeneralModel.Authors = dataBase.getAuthors();

            myStudentView.Students = dataBase.getStudents();
            myStudentView.Class = dataBase.getClass();


        }

        public async Task<IActionResult> Index(string SearchString, string Type, string Author)
        {
            /*
            foreach(BookGeneralInfo bgi in BookGeneralModel.BookGeneralInfo)
            {
                bgi.Status = dataBase.getStatus(bgi.Book.BookId);
            }
            */
            return View(BookGeneralModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Search(string? searchText, string? type, string? author)
        {
            MyViewModel temp = BookGeneralModel;


            if (!String.IsNullOrEmpty(searchText))
            {
                temp.BookGeneralInfo = temp.BookGeneralInfo.Where(x => x.Book.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            if (!String.IsNullOrEmpty(type))
            {
                temp.BookGeneralInfo = temp.BookGeneralInfo.Where(x => x.Type == type).ToList();
            }

            if (!String.IsNullOrEmpty(author))
            {
                int authorId = Convert.ToInt32(author);
                temp.BookGeneralInfo = temp.BookGeneralInfo.Where(x => x.Book.AuthorId == authorId).ToList();
            }

            return View("Index", BookGeneralModel);
        }

        public ActionResult BookDetails(int bookId)
        {
            MyBookViewModel myBookView = new MyBookViewModel();
            myBookView.BookDetail = dataBase.getBorrowDetails(bookId);
            myBookView.BookName = dataBase.getBook(bookId).Name;
            myBookView.BookId = bookId;
            myBookView.Status = dataBase.getStatus(bookId);

            myBookView.BookDetail = myBookView.BookDetail.OrderByDescending(x => x.borrowId).ToList();

            return View(myBookView);
        }

        public ActionResult Student(int bookId, string bookName)
        {
            myStudentView.Status = dataBase.getStatus(bookId);
            myStudentView.BookId = bookId;
            myStudentView.BookName = bookName;

            if (myStudentView.Status == false)
            {
                myStudentView.BStudentId = dataBase.getBStudentId(bookId);

                int index = 0;
                foreach(Student student in myStudentView.Students)
                {
                    if(student.studentId == myStudentView.BStudentId)
                    {
                        index = myStudentView.Students.IndexOf(student);
                        break;
                    }
                }
                Student top = myStudentView.Students.ElementAt(index);
                myStudentView.Students.Remove(top);
                myStudentView.Students.Insert(0, top);
            }
                
            else
                myStudentView.BStudentId = null;


            return View(myStudentView);

        }

        public ActionResult SearchStudent(string? searchText, string? classNumber)
        {
            MyStudentViewModel temp = myStudentView;

            if (!String.IsNullOrEmpty(searchText))
            {
                temp.Students = temp.Students.Where(x => x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }

            if (!String.IsNullOrEmpty(classNumber))
            {
                temp.Students = temp.Students.Where(x => x.Class == classNumber).ToList();
            }






            return View("Student", temp);
        }

        public ActionResult BorrowBook(int bookId, int studentId)
        {

            dataBase.BorrowBook(bookId, studentId);

            return RedirectToAction("Student", new {bookId = bookId, bookName = myStudentView.BookName});
        }

        public ActionResult ReturnBook(int bookId, int studentId)
        {
            dataBase.ReturnBook(bookId, studentId);
            return RedirectToAction("Student", new { bookId = bookId, bookName = myStudentView.BookName });
        }
    }
}