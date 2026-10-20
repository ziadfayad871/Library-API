using AutoMapper;
using LibraryWeb.Models;
using LibraryWeb.Models.CopyDTO;
using LibraryWeb.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryWeb.Controllers
{
    public class BookCopyController : Controller
    {
        private readonly IBookCopyService _bookCopyService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookCopyController(IBookCopyService bookCopyService, IBookService bookService, IMapper mapper)
        {
            _bookCopyService = bookCopyService;
            _bookService = bookService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var booksResponse = await _bookService.GetAllBooksAsync();
            var bookTitles = new Dictionary<int, string>();

            if (booksResponse != null && booksResponse.IsSuccess && booksResponse.Result != null)
            {
                var books = (List<BookDTO>)booksResponse.Result;
                bookTitles = books.ToDictionary(b => b.Id, b => b.Name);
            }

            ViewData["BookTitles"] = bookTitles;

            var response = await _bookCopyService.GetAllBookCopiesAsync();

            if (response != null && response.IsSuccess && response.Result != null)
            {
                return View((List<BookCopyDTO>)response.Result);
            }

            if (response != null && !response.IsSuccess && response.ErrorMessages != null)
            {
                TempData["Error"] = string.Join(", ", response.ErrorMessages);
            }

            return View(new List<BookCopyDTO>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadBooksListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCopyCreateDTO bookCopyCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                await LoadBooksListAsync(bookCopyCreateDTO.BookId);
                return View(bookCopyCreateDTO);
            }

            var response = await _bookCopyService.CreateBookCopyAsync(bookCopyCreateDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book copy created successfully.";
                return RedirectToAction(nameof(Index));
            }

            if (response != null && response.ErrorMessages != null)
            {
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            await LoadBooksListAsync(bookCopyCreateDTO.BookId);
            return View(bookCopyCreateDTO);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var response = await _bookCopyService.GetBookCopyByIdAsync(id);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                var bookCopy = (BookCopyDTO)response.Result;
                var bookCopyUpdateDTO = _mapper.Map<BookCopyUpdateDTO>(bookCopy);
                await LoadBooksListAsync(bookCopy.BookId);
                return View(bookCopyUpdateDTO);
            }

            TempData["Error"] = "Book copy not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BookCopyUpdateDTO bookCopyUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                await LoadBooksListAsync(bookCopyUpdateDTO.BookId);
                return View(bookCopyUpdateDTO);
            }

            var response = await _bookCopyService.UpdateBookCopyAsync(bookCopyUpdateDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book copy updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            if (response != null && response.ErrorMessages != null)
            {
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            await LoadBooksListAsync(bookCopyUpdateDTO.BookId);
            return View(bookCopyUpdateDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _bookCopyService.DeleteBookCopyAsync(id);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book copy deleted successfully.";
            }
            else if (response != null && response.ErrorMessages != null)
            {
                TempData["Error"] = string.Join(", ", response.ErrorMessages);
            }
            else
            {
                TempData["Error"] = "An error occurred while deleting the book copy.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadBooksListAsync(int? selectedBookId = null)
        {
            var response = await _bookService.GetAllBooksAsync();

            var books = response != null && response.IsSuccess && response.Result != null
                ? (List<BookDTO>)response.Result
                : new List<BookDTO>();

            ViewBag.BooksList = new SelectList(books, "Id", "Name", selectedBookId);
        }
    }
}
