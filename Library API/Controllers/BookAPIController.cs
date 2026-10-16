using Library_API.Data;
using Library_API.Dtos;
using Library_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/BookAPI")]
    [ApiController]
    public class BookAPIController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BookAPIController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        // code status: 200 OK
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookDTO>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetBooks()
        {
            var books = _bookRepository.GetAllBooks().Result;

            return Ok(books);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookDTO), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetBook(int id)
        {
            var book = _bookRepository.GetBookById(id).Result;
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        // POST   api/BookAPI -> كتاب جديد - 20
        [HttpPost]
        [ProducesResponseType(typeof(BookDTO), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult CreateBook([FromBody] BookCreateDTO bookCreateDTO)
        {
            var book = _bookRepository.CreateBook(bookCreateDTO).Result;
            if (book == null)
            {
                return BadRequest();
            }
            return Ok(book);
        }
        // PUT   api/BookAPI -> تعديل كتاب - 20
        [HttpPut]
        [ProducesResponseType(typeof(BookDTO), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public IActionResult UpdateBook(BookUpdateDTO bookUpdateDTO)
        {
            var book = _bookRepository.UpdateBook(bookUpdateDTO).Result;
            if (book == null)
            {
                return BadRequest();
            }
            return Ok(book);
        }
        // PATCH  api/BookAPI/{id} -> تعديل جزئي بـ JsonPatchDocument - 204 / 400 / 404
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult PatchBook(int id, JsonPatchDocument<BookUpdateDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var bookDto = _bookRepository.GetBookById(id).Result;
            if (bookDto == null)
            {
                return NotFound();
            }
            var bookToPatch = new BookUpdateDTO
            {
                Id = bookDto.Id,
                Name = bookDto.Name,
                Author = bookDto.Author,
                Description = bookDto.Description,
                Price = bookDto.Price,
                Pages = bookDto.Pages,
                PublishedYear = bookDto.PublishedYear
            };
            patchDoc.ApplyTo(bookToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedBook = _bookRepository.UpdateBook(bookToPatch).Result;
            if (updatedBook == null)
            {
                return BadRequest();
            }
            return NoContent();

        }
        //DELETE api/BookAPI/{id} -> مسح - 200 / 404
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult DeleteBook(int id)
        {
            var book = _bookRepository.GetBookById(id).Result;
            if (book == null)
            {
                return NotFound();
            }
            var result = _bookRepository.DeleteBook(id).Result;
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
