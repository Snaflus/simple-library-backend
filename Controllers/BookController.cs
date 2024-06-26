using LibraryApi.Models;
using LibraryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchGenre = null)
    {
        try
        {
            var books = await _bookService.GetBooks(page, pageSize, searchGenre);
            return Ok(books);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        try
        {
            var book = await _bookService.GetBook(id);
            return Ok(book);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {
                return NotFound($"Book with id {id} not found");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpGet("title/{bookTitle}")]
    public async Task<ActionResult<Book>> GetBook(string bookTitle)
    {
        try
        {
            var book = await _bookService.GetBook(bookTitle);
            return Ok(book);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {
                return NotFound($"Book with title {bookTitle} not found");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<Book>> AddBook(Book book)
    {
        try
        {
            var createdBook = await _bookService.AddBook(book);
            string host = HttpContext.Request.Host.Value;
            string uri = $"{Request.Path}/{createdBook.Id}";
            return Created(uri, createdBook);
        }
        catch (Exception e)
        {
            if (e.Message.Contains($"Book with title {book.Title} already exists"))
            {
                return Conflict(e.Message);
            }
            return BadRequest(e.Message);
        }
    }

    [HttpPut, Authorize(Roles = "Admin")]
    public async Task<ActionResult<Book>> UpdateBook([FromBody] Book book)
    {
        try
        {
            var updatedBook = await _bookService.UpdateBook(book);
            return Ok(updatedBook);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        try
        {
            await _bookService.DeleteBook(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
