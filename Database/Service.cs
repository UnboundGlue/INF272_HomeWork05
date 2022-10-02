using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Super_Duper_Library_System.Models;
using System.Data.SqlClient;

namespace Super_Duper_Library_System.Database
{
    public class Service
    {
        public SqlConnection openDBConnection()
        {
            SqlConnection con = null;

            //TODO: open connection and return it.
            con = new SqlConnection("Data Source=DESKTOP-7NMUUME;Initial Catalog=Library;Integrated Security=True");
            con.Open();
            return con;
        }

        public List<Types> getType()
        {
            List<Types> types = new List<Types>();

            using (SqlConnection con = openDBConnection())
            {
                using (SqlCommand cmd = new SqlCommand("select * from types", con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                types.Add(new Types
                                {
                                    Name = reader["name"].ToString(),
                                    TypeId = Convert.ToInt32(reader["typeId"])
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
            return types;
        }
    

        public List<BookGeneralInfo> getBookGeneral()
        {
            List<BookGeneralInfo> bookGeneralList = new List<BookGeneralInfo>();

            using (SqlConnection con = openDBConnection())
            {
                string q = @"select books.bookId AS bookId, books.name AS bookName, books.pagecount AS pagecount, books.point AS point, authors.name AS authorName, authors.authorId , types.name AS type
                             from books
                             JOIN authors ON books.authorId = authors.authorId
                             JOIN types ON books.typeId = types.typeId";

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    try
                    {
                       using (SqlDataReader reader = cmd.ExecuteReader())
                       {
                            while (reader.Read())
                            {
                                bookGeneralList.Add(new BookGeneralInfo
                                {
                                    Book = new Books
                                    {
                                        BookId = Convert.ToInt32(reader["bookId"]),
                                        AuthorId = Convert.ToInt32(reader["authorId"]),
                                        Name = reader["bookName"].ToString(),
                                        PageCount = Convert.ToInt32(reader["pagecount"]),
                                        Point = Convert.ToInt32(reader["point"])
                                    },
                                    Type = reader["type"].ToString(),
                                    AuthorName = reader["authorName"].ToString(),
                                    Status = getStatus(Convert.ToInt32(reader["bookId"]))
                                });
                            }
                       }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return bookGeneralList;

        }

        public List<Author> getAuthors()
        {
            List<Author> authors = new List<Author>();

            using (SqlConnection con = openDBConnection())
            {
                using (SqlCommand cmd = new SqlCommand("select * from authors", con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                authors.Add(new Author {
                                    Name = reader["name"].ToString(),
                                    Surname = reader["surname"].ToString(),
                                    Id = Convert.ToInt32(reader["authorId"])
                                });
                            }

                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }

            return authors;
        }

        public List<BookDetail> getBorrowDetails(int bookId)
        {
            List<BookDetail> bookDetails = new List<BookDetail>();

            using (SqlConnection con = openDBConnection())
            {
                string q = @"select borrows.borrowId, borrows.takenDate, borrows.broughtDate,students.name AS studentName, students.surname AS studentSurname
                            from borrows
                            INNER join students ON borrows.studentId = students.studentId
							where borrows.bookId = " + bookId.ToString();
                
                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(3))
                                {
                                    bookDetails.Add(new BookDetail
                                    {
                                        borrowId = Convert.ToInt32(reader["borrowId"]),
                                        takenDate = Convert.ToDateTime(reader["takenDate"]),
                                        BroughtDate = Convert.ToDateTime(reader["broughtDate"]),
                                        StudentName = reader["studentName"].ToString(),
                                        StudentSurname = reader["studentSurname"].ToString(),
                                    });
                                } else
                                {
                                    bookDetails.Add(new BookDetail
                                    {
                                        borrowId = Convert.ToInt32(reader["borrowId"]),
                                        takenDate = Convert.ToDateTime(reader["takenDate"]),
                                        BroughtDate = null,
                                        StudentName = reader["studentName"].ToString(),
                                        StudentSurname = reader["studentSurname"].ToString(),
                                    });
                                }

                                    
                            }

                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            return bookDetails;
        }

        public Books getBook(int bookId)
        {
            Books book = new Books();

            using (SqlConnection con = openDBConnection())
            {
                string q = @"select * from books where books.bookId = " + bookId.ToString();

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            book = new Books
                            {
                                AuthorId = Convert.ToInt32(reader["authorId"]),
                                BookId = Convert.ToInt32(reader["bookId"]),
                                Name = reader["name"].ToString(),
                                PageCount = Convert.ToInt32(reader["pagecount"]),
                                Point = Convert.ToInt32(reader["point"]),
                                TypeId = Convert.ToInt32(reader["typeId"])
                            };
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            return book;
        }

        public Boolean getStatus(int bookId)
        {
            Boolean status = true;

            using (SqlConnection con = openDBConnection())
            {
                string q = @"select borrows.broughtDate
                            from borrows
                            where borrows.takenDate=(
                                select MAX(borrows.takenDate)
                                from borrows
                                where borrows.bookId = " + bookId.ToString() + ")";

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            if (reader.IsDBNull(0))
                                status = false;
                            else
                                status = true;
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            return status;
        }

        public List<String> getClass()
        {
            List<String> list = new List<String>();

            using (SqlConnection con = openDBConnection())
            {
                string q = @"select class from students";

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                if(!list.Contains(reader["class"].ToString()))
                                {
                                    list.Add(reader["class"].ToString());
                                }
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            list.Sort();
            return list;

        }

        public List<Student> getStudents()
        {
            List<Student> students = new List<Student>();

            using (SqlConnection con = openDBConnection())
            {
                string q = @"select * 
                            from students";

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                students.Add(new Student
                                {
                                    Name = reader["name"].ToString(),
                                    Surname = reader["surname"].ToString(),
                                    studentId = Convert.ToInt32(reader["studentId"]),
                                    Class = reader["class"].ToString(),
                                    Point = Convert.ToInt32(reader["point"])
                                });
                            }

                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }
            return students;

        }

        public int getBStudentId(int bookId)
        {
            int StudentId = 0;
            using (SqlConnection con = openDBConnection())
            {
                string q = @"select borrows.broughtDate, borrows.studentId
                            from borrows
                            where borrows.takenDate=(
                                select MAX(borrows.takenDate)
                                from borrows
                                where borrows.bookId = " + bookId.ToString() + ")";

                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            StudentId = Convert.ToInt32(reader["studentId"]);
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.Message); }
                }
            }

            return StudentId;
        }

        public void ReturnBook(int BookId, int StudentId)
        {
            using (SqlConnection con = openDBConnection())
            {
                string q = @"UPDATE borrows
                             SET broughtDate = '" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")
                             +"' WHERE bookId = " + BookId + " AND studentId = " + StudentId;

                try
                {
                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }

            }
        }

        public void BorrowBook(int BookId, int StudentId)
        {
            using (SqlConnection con = openDBConnection())
            {
                string q = @"INSERT INTO borrows (studentId,bookId,takenDate)
                             VALUES (" + StudentId + ","+ BookId + ", '" + DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") + "')";

                try
                {
                    SqlCommand cmd = new SqlCommand(q, con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }
                
            }
        }
    }

    

}
