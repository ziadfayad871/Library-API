using Asp.Versioning;
using Library_API.Dtos.APIResponse;
using Library_API.Dtos.BookCopyDTOs;
using Library_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/BookCopyAPI")]
    [ApiController]
    [Authorize]
    public class BookCopyAPIController : ControllerBase
    {
        private readonly IBookCopyRepository _bookCopyRepository;

        public BookCopyAPIController(IBookCopyRepository bookCopyRepository)
        {
            _bookCopyRepository = bookCopyRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(APIResponse), 200)]
        public async Task<IActionResult> GetBookCopies()
        {
            var response = await _bookCopyRepository.GetAllBookCopiesAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}", Name = "GetBookCopy")]
        [ProducesResponseType(typeof(APIResponse), 200)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> GetBookCopy(int id)
        {
            var response = await _bookCopyRepository.GetBookCopyByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse), 201)]
        [ProducesResponseType(typeof(APIResponse), 400)]
        public async Task<IActionResult> CreateBookCopy([FromBody] BookCopyCreateDTO bookCopy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _bookCopyRepository.AddBookCopyAsync(bookCopy);
            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response);
            }
            var createdBookCopy = (BookCopyDTO)response.Result;
            return CreatedAtRoute("GetBookCopy", new { id = createdBookCopy.CopyNo }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse), 200)]
        [ProducesResponseType(typeof(APIResponse), 400)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> UpdateBookCopy(int id, [FromBody] BookCopyUpdateDTO bookCopy)
        {
            if (id != bookCopy.CopyNo)
            {
                return BadRequest(new APIResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "ID in the URL does not match CopyNo in the request body." }
                });
            }
            var response = await _bookCopyRepository.UpdateBookCopyAsync(bookCopy);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse), 200)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> DeleteBookCopy(int id)
        {
            var response = await _bookCopyRepository.DeleteBookCopyAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(APIResponse), 400)]
        [ProducesResponseType(typeof(APIResponse), 404)]
        public async Task<IActionResult> PatchBookCopy(int id, [FromBody] JsonPatchDocument<BookCopyUpdateDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var response = await _bookCopyRepository.GetBookCopyByIdAsync(id);
            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response);
            }
            var bookCopyDto = (BookCopyDTO)response.Result;
            var bookCopyToPatch = new BookCopyUpdateDTO
            {
                CopyNo = bookCopyDto.CopyNo,
                BookId = bookCopyDto.BookId,
                ShelfLocation = bookCopyDto.ShelfLocation,
                IsAvailable = bookCopyDto.IsAvailable,
                Condition = bookCopyDto.Condition
            };
            patchDoc.ApplyTo(bookCopyToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updateResponse = await _bookCopyRepository.UpdateBookCopyAsync(bookCopyToPatch);
            if (!updateResponse.IsSuccess)
            {
                return StatusCode(updateResponse.StatusCode, updateResponse);
            }
            return NoContent();
        }
    }
}
