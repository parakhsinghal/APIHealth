using APIHealthCheck.Model;
using System.Collections.Generic;

namespace APIHealthCheck.Repository.Interfaces
{
    public interface ILibraryRepository
    {
        List<Book> GetBooks();
        List<Author> GetAuthors();
        List<Book> GetBooksByAuthor(string firstName, string lastName);
    }
}
