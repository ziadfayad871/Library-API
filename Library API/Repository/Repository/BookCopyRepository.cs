using AutoMapper;
using Library_API.Data;
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
        public async Task<BookCopyDTO> AddBookCopyAsync(BookCopyCreateDTO bookCopy)
        {
            var bookCopyEntity = _mapper.Map<BookCopyCreateDTO, BookCopy>(bookCopy);
            // Add the new book copy to the context and save changes :
            var createdBookCopy = await _context.BookCopies.AddAsync(bookCopyEntity);
            var saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return _mapper.Map<BookCopyDTO>(createdBookCopy.Entity);
            }
            else
            {
                throw new Exception("Failed to save the book copy.");
            }
            

        }

        public async Task<bool> DeleteBookCopyAsync(int id)
        {
            var bookCopy = await  _context.BookCopies.FirstOrDefaultAsync(bc => bc.CopyNo == id);
            if (bookCopy == null)
            {
                return false;
            }

            _context.BookCopies.Remove(bookCopy);
            var saveResult = await  _context.SaveChangesAsync();
            return saveResult > 0;
        }

        public async Task<IEnumerable<BookCopyDTO>> GetAllBookCopiesAsync()
        {
            var bookCopies = await _context.BookCopies.AsNoTracking().ToListAsync();
            return bookCopies.Select(bc => _mapper.Map<BookCopyDTO>(bc));
        }

        public async Task<BookCopyDTO> GetBookCopyByIdAsync(int id)
        {
            var bookCopy = await  _context.BookCopies.FirstOrDefaultAsync(bc => bc.CopyNo == id);
            if (bookCopy == null)
            {
                return null!;
            }
            return _mapper.Map<BookCopyDTO>(bookCopy);
        }

        public async Task<bool> Save()
        {
            var saveResult = await  _context.SaveChangesAsync();
            return saveResult > 0;
        }

        public async Task<BookCopyDTO> UpdateBookCopyAsync(BookCopyUpdateDTO bookCopy)
        {
            var existingBookCopy = await _context.BookCopies.FirstOrDefaultAsync(bc => bc.CopyNo == bookCopy.CopyNo);
            if (existingBookCopy == null)
            {
                return null!;
            }
           var updatedBookCopy = _mapper.Map(bookCopy, existingBookCopy);
            _context.BookCopies.Update(updatedBookCopy);
            var saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return _mapper.Map<BookCopyDTO>(updatedBookCopy);
            }
            else
            {
                throw new Exception("Failed to update the book copy.");
            }
        }
    }
}
