using APIHealthCheck.Model;
using APIHealthCheck.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APIHealthCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibraryController : Controller
    {
        ILibraryRepository repository;

        public LibraryController(ILibraryRepository libraryRepository)
        {
            repository = libraryRepository;
        }

        [HttpGet("books")]
        public IActionResult GetBooks()
        {            
            return Ok(repository.GetBooks());
        }

        [HttpGet("authors")]
        public IActionResult GetAuthors()
        {
            return Ok(repository.GetAuthors());
        }

        [HttpGet("booksbyauthor")]
        public IActionResult GetBooksByAuthor(string firstName, string lastName)
        {
            List<Book> result = repository.GetBooksByAuthor(firstName, lastName);

            if (result.Count>0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound($"No books were found for the author {firstName} {lastName}");
            }
        }
    }
}
