using AutoMapper;
using LibraryWeb.Models;
using LibraryWeb.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWeb.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string author)
        {
            ViewBag.Author = author;

            var response = await _bookService.GetAllBooksAsync(author);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                var books = (List<BookDTO>)response.Result;
                return View(books);
            }

            if (response != null && !response.IsSuccess && response.ErrorMessages != null)
            {
                TempData["Error"] = string.Join(", ", response.ErrorMessages);
            }

            return View(new List<BookDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateDTO bookCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(bookCreateDTO);
            }

            var response = await _bookService.CreateBookAsync(bookCreateDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book created successfully.";
                return RedirectToAction(nameof(Index));
            }

            if (response != null && response.ErrorMessages != null)
            {
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(bookCreateDTO);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var response = await _bookService.GetBookByIdAsync(id);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                var book = (BookDTO)response.Result;
                var bookUpdateDTO = _mapper.Map<BookUpdateDTO>(book);
                return View(bookUpdateDTO);
            }

            if (response != null && !response.IsSuccess && response.ErrorMessages != null)
            {
                TempData["Error"] = string.Join(", ", response.ErrorMessages);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BookUpdateDTO bookUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(bookUpdateDTO);
            }

            var response = await _bookService.UpdateBookAsync(bookUpdateDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            if (response != null && response.ErrorMessages != null)
            {
                foreach (var error in response.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(bookUpdateDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _bookService.DeleteBookAsync(id);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Book deleted successfully.";
            }
            else if (response != null && response.ErrorMessages != null)
            {
                TempData["Error"] = string.Join(", ", response.ErrorMessages);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
