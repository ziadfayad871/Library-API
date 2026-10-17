using Library_API.Dtos;
using Library_API.Dtos.BookCopyDTOs;
using Library_API.Repository.IRepository;
using Library_API.Repository.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/BookCopyAPI")]
    // [Route("api/[controller]")]
    [ApiController]
    public class BookCopyAPIController : ControllerBase
    {
        private readonly IBookCopyRepository _bookCopyRepository;

        public BookCopyAPIController(IBookCopyRepository bookCopyRepository)
        {
            _bookCopyRepository = bookCopyRepository;
        }
        // code status: 200 OK
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookCopyDTO>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetBookCopies()
        {
            var bookCopies = await _bookCopyRepository.GetAllBookCopiesAsync() ;
           
           
            return Ok(bookCopies);
        }
        [HttpGet("{id}", Name = "GetBookCopy")]
        [ProducesResponseType(typeof(BookCopyDTO), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetBookCopy(int id)
        {
            var bookCopy = await _bookCopyRepository.GetBookCopyByIdAsync(id);
            if (bookCopy == null)
            {
                return NotFound();
            }
            
            return Ok(bookCopy);
        }
        [HttpPost]
        [ProducesResponseType(typeof(BookCopyDTO), 201)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> CreateBookCopy([FromBody] BookCopyCreateDTO bookCopy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdBookCopy = await _bookCopyRepository.AddBookCopyAsync(bookCopy);
            return CreatedAtRoute("GetBookCopy", new { id = createdBookCopy.CopyNo }, createdBookCopy);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookCopyDTO), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> UpdateBookCopy(int id, [FromBody] BookCopyUpdateDTO bookCopy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != bookCopy.CopyNo)
            {
                return BadRequest("ID mismatch");
            }
            
            var updatedBookCopy = await _bookCopyRepository.UpdateBookCopyAsync(bookCopy);
            if (updatedBookCopy == null)
            {
                return NotFound();
            }
            return Ok(updatedBookCopy);
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> DeleteBookCopy(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var bookCopy = await _bookCopyRepository.GetBookCopyByIdAsync(id);
            if (bookCopy == null)
            {
                return NotFound();
            }
            if (bookCopy.IsAvailable == false)
            {
                return BadRequest("Cannot delete a book copy that is currently checked out.");
            }
            var isDeleted = await _bookCopyRepository.DeleteBookCopyAsync(id);
            return Ok(isDeleted);
        }
        // PATCH  api/BookAPI/{id} -> تعديل جزئي بـ JsonPatchDocument - 204 / 400 / 404
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> PatchBook(int id, JsonPatchDocument<BookCopyUpdateDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var bookDto = await _bookCopyRepository.GetBookCopyByIdAsync(id);
            if (bookDto == null)
            {
                return NotFound();
            }
            var bookToPatch = new BookCopyUpdateDTO
            {
                CopyNo = bookDto.CopyNo,
                BookId = bookDto.BookId,
                ShelfLocation = bookDto.ShelfLocation,
                IsAvailable = bookDto.IsAvailable,
                Condition = bookDto.Condition
            };
            patchDoc.ApplyTo(bookToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedBook = await _bookCopyRepository.UpdateBookCopyAsync(bookToPatch);
            if (updatedBook == null)
            {
                return BadRequest();
            }
            return NoContent();

        }
    }
}
