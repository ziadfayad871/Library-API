using AutoMapper;
using Library_API.Data;
using Library_API.Dtos;
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

        public async Task<BookDTO> CreateBook(BookCreateDTO bookCreateDTO)
        {
            var book = _mapper.Map<Book>(bookCreateDTO);
            book.CreatedAt = DateTime.UtcNow;
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return _mapper.Map<BookDTO>(book);
        }

        public async Task<bool> DeleteBook(int bookId)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                return false;
            }
            _context.Books.Remove(book);
            return await Save();
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooks()
        {
            var books = await _context.Books.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<BookDTO>>(books);
        }

        public async Task<BookDTO> GetBookById(int bookId)
        {
            var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                return null!;
            }
            return _mapper.Map<BookDTO>(book);
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<BookDTO> UpdateBook(BookUpdateDTO bookUpdateDTO)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookUpdateDTO.Id);
            if (book == null)
            {
                return null!;
            }
            _mapper.Map(bookUpdateDTO, book);
            await _context.SaveChangesAsync();
            return _mapper.Map<BookDTO>(book);
        }
    }
}
