using AutoMapper;
using Library_API.Data;
using Library_API.Dtos;
using Library_API.Dtos.APIResponse;
using Library_API.Model;
using Library_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repository.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BookRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<APIResponse> CreateBook(BookCreateDTO bookCreateDTO)
        {
            try
            {
                var book = _mapper.Map<Book>(bookCreateDTO);
                book.CreatedAt = DateTime.UtcNow;
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    StatusCode = 201,
                    IsSuccess = true,
                    Result = _mapper.Map<BookDTO>(book)
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse> UpdateBook(BookUpdateDTO bookUpdateDTO)
        {
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookUpdateDTO.Id);
                if (book == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Book not found" }
                    };
                }
                _mapper.Map(bookUpdateDTO, book);
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Result = _mapper.Map<BookDTO>(book)
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }

        public async Task<APIResponse> GetBookById(int bookId)
        {
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                return new APIResponse
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Book not found" }
                };
            }
            return new APIResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Result = _mapper.Map<BookDTO>(book)
            };
        }

        public async Task<APIResponse> GetAllBooks(string? author = null, string search = null, decimal? minPrice = null, decimal? maxPrice = null, int pageNumber = 1, int pageSize = 5)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return new APIResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Minimum price cannot be greater than maximum price." }
                };
            }

            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new APIResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Page number and page size must be greater than zero." }
                };
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            var query = _context.Books.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(author))
                query = query.Where(b => b.Author.Contains(author));
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b => b.Name.Contains(search) || b.Description.Contains(search));
            if (minPrice.HasValue)
                query = query.Where(b => b.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(b => b.Price <= maxPrice.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var books = await query.OrderBy(b => b.Id)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new APIResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Result = _mapper.Map<IEnumerable<BookDTO>>(books),
                Pagination = new Pagination
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPrevious = pageNumber > 1,
                    HasNext = pageNumber < totalPages
                }
            };
        }

        public async Task<APIResponse> DeleteBook(int bookId)
        {
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
                if (book == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Book not found" }
                    };
                }
                var deletedBook = _mapper.Map<BookDTO>(book);
                var BookCopies = await _context.BookCopies.Where(bc => bc.BookId == bookId).ToListAsync();
                if (BookCopies.Any())
                {
                    return new APIResponse
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Cannot delete a book that has book copies." }
                    };
                }
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Result = deletedBook
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
