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
        public async Task<IActionResult> GetBooks()
        {
            var books = await _bookRepository.GetAllBooks();

            return Ok(books);
        }
        // id and Name of book
        [HttpGet("{id}", Name = "GetBook")]
        [ProducesResponseType(typeof(BookDTO), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);
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
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDTO bookCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var book = await _bookRepository.CreateBook(bookCreateDTO);
            
            if (book == null)
            {
                return BadRequest();
            }
            
            return CreatedAtRoute("GetBook", new { id = book.Id }, book);
        }
        // PUT   api/BookAPI -> تعديل كتاب - 20
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookDTO), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> UpdateBook(BookUpdateDTO bookUpdateDTO , int id)
        {
            var bookFromDb = await _bookRepository.GetBookById(id);
            // لو id اللي في الـ URL مختلف عن bookUpdateDTO.Id → 400 : 
            if (bookFromDb == null )
            {
                return NotFound();
            }
            if (bookUpdateDTO.Id != id)
            {
                return BadRequest("ID in the URL does not match ID in the request body.");
            }

            var book = await _bookRepository.UpdateBook(bookUpdateDTO);
            if (book == null)
            {
                // 404 : 
                return NotFound();
            }

            return Ok(book);
        }
        // PATCH  api/BookAPI/{id} -> تعديل جزئي بـ JsonPatchDocument - 204 / 400 / 404
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> PatchBook(int id, JsonPatchDocument<BookUpdateDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var bookDto = await _bookRepository.GetBookById(id);
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
            var updatedBook = await _bookRepository.UpdateBook(bookToPatch);
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
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            var result = await _bookRepository.DeleteBook(id);
            if (!result)
            {
                return BadRequest();
            }
            var message = new { Message = $"Book with ID {id} has been deleted successfully." };
            return Ok(message);
        }
    }
}
