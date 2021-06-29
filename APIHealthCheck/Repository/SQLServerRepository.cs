using APIHealthCheck.Model;
using APIHealthCheck.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace APIHealthCheck.Repository
{
    public class SQLServerRepository : ILibraryRepository
    {
        private string connectionString;

        public SQLServerRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("LibraryDB");
        }

        public List<Author> GetAuthors()
        {
            string sqlText = "dbo.GetAuthors";
            DataTable dataTable;
            List<Author> authors = new List<Author>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = sqlText,
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (dataTable = new DataTable())
                {
                    dataTable.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                }
            }

            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    authors.Add(new Author()
                    {
                        AuthorId = int.Parse(dr["AuthorId"].ToString()),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        IsActive = bool.Parse(dr["IsActive"].ToString()),
                        RowId = dr["RowId"] as byte[]
                    });
                }
            }

            return authors;
        }

        public List<Book> GetBooks()
        {
            string sqlText = "dbo.GetBooks";
            DataTable dataTable;
            List<Book> books = new List<Book>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = sqlText,
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (dataTable = new DataTable())
                {
                    dataTable.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                }
            }

            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    books.Add(new Book()
                    {
                        BookId = int.Parse(dr["BookId"].ToString()),
                        Title = dr["Title"].ToString(),
                        PageCount = int.Parse(dr["PageCount"].ToString()),
                        IsActive = bool.Parse(dr["IsActive"].ToString()),
                        RowId = dr["RowId"] as byte[]
                    });
                }
            }

            return books;
        }

        public List<Book> GetBooksByAuthor(string firstName, string lastName)
        {
            string sqlText = "dbo.GetBooksByAuthor";
            DataTable dataTable;
            List<Book> books = new List<Book>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("FirstName", firstName));
            parameters.Add(new SqlParameter("LastName", lastName));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = sqlText,
                    CommandType = CommandType.StoredProcedure                    
                };

                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                connection.Open();
                using (dataTable = new DataTable())
                {
                    dataTable.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                }
            }

            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    books.Add(new Book()
                    {
                        BookId = int.Parse(dr["BookId"].ToString()),
                        Title = dr["Title"].ToString(),
                        PageCount = int.Parse(dr["PageCount"].ToString()),
                        IsActive = bool.Parse(dr["IsActive"].ToString()),
                        RowId = dr["RowId"] as byte[]
                    });
                }
            }

            return books;
        }       
    }
}
