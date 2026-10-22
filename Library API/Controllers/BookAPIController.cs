using Library_API.Dtos;
using Library_API.Dtos.APIResponse;
using Library_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/BookAPI")]
    [ApiController]
    [Authorize]
    public class BookAPIController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BookAPIController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(APIResponse), 200)]
        public async Task<IActionResult> GetBooks(string? author = null)
        {
            var response = await _bookRepository.GetAllBooks(author);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}", Name = "GetBook")]
        [ProducesResponseType(typeof(APIResponse), 200)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> GetBook(int id)
        {
            var response = await _bookRepository.GetBookById(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse), 201)]
        [ProducesResponseType(typeof(APIResponse), 400)]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDTO bookCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _bookRepository.CreateBook(bookCreateDTO);
            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response);
            }
            var createdBook = (BookDTO)response.Result;
            return CreatedAtRoute("GetBook", new { id = createdBook.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse), 200)]
        [ProducesResponseType(typeof(APIResponse), 400)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDTO bookUpdateDTO)
        {
            if (bookUpdateDTO.Id != id)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "ID in the URL does not match ID in the request body." }
                });
            }
            var response = await _bookRepository.UpdateBook(bookUpdateDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(APIResponse), 400)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> PatchBook(int id, [FromBody] JsonPatchDocument<BookUpdateDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var response = await _bookRepository.GetBookById(id);
            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response);
            }
            var bookDto = (BookDTO)response.Result;
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
            var updateResponse = await _bookRepository.UpdateBook(bookToPatch);
            if (!updateResponse.IsSuccess)
            {
                return StatusCode(updateResponse.StatusCode, updateResponse);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse), 200)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var response = await _bookRepository.DeleteBook(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
