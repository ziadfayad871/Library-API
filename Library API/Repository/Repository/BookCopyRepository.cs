using AutoMapper;
using Library_API.Data;
using Library_API.Dtos.APIResponse;
using Library_API.Dtos.BookCopyDTOs;
using Library_API.Model;
using Library_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repository.Repository
{
    public class BookCopyRepository : IBookCopyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BookCopyRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<APIResponse> AddBookCopyAsync(BookCopyCreateDTO bookCopy)
        {
            try
            {
                var bookCopyEntity = _mapper.Map<BookCopy>(bookCopy);
                await _context.BookCopies.AddAsync(bookCopyEntity);
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    StatusCode = 201,
                    IsSuccess = true,
                    Result = _mapper.Map<BookCopyDTO>(bookCopyEntity)
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

        public async Task<APIResponse> DeleteBookCopyAsync(int id)
        {
            try
            {
                var bookCopy = await _context.BookCopies.FirstOrDefaultAsync(bc => bc.CopyNo == id);
                if (bookCopy == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Book copy not found" }
                    };
                }
                if (!bookCopy.IsAvailable)
                {
                    return new APIResponse
                    {
                        StatusCode = 400,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Cannot delete a book copy that is currently checked out." }
                    };
                }
                var deletedBookCopy = _mapper.Map<BookCopyDTO>(bookCopy);
                _context.BookCopies.Remove(bookCopy);
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Result = deletedBookCopy
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

        public async Task<APIResponse> GetAllBookCopiesAsync()
        {
            var bookCopies = await _context.BookCopies.AsNoTracking().ToListAsync();
            return new APIResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Result = _mapper.Map<IEnumerable<BookCopyDTO>>(bookCopies)
            };
        }

        public async Task<APIResponse> GetBookCopyByIdAsync(int id)
        {
            var bookCopy = await _context.BookCopies.AsNoTracking().FirstOrDefaultAsync(bc => bc.CopyNo == id);
            if (bookCopy == null)
            {
                return new APIResponse
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Book copy not found" }
                };
            }
            return new APIResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Result = _mapper.Map<BookCopyDTO>(bookCopy)
            };
        }

        public async Task<APIResponse> UpdateBookCopyAsync(BookCopyUpdateDTO bookCopy)
        {
            try
            {
                var existingBookCopy = await _context.BookCopies.FirstOrDefaultAsync(bc => bc.CopyNo == bookCopy.CopyNo);
                if (existingBookCopy == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Book copy not found" }
                    };
                }
                _mapper.Map(bookCopy, existingBookCopy);
                await _context.SaveChangesAsync();
                return new APIResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Result = _mapper.Map<BookCopyDTO>(existingBookCopy)
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
