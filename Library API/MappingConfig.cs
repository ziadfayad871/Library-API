using AutoMapper;
using Library_API.Dtos;
using Library_API.Dtos.BookCopyDTOs;
using Library_API.Model;

namespace Library_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Book mapping
            // create mapping for BookCreateDTO to Book and vice versa
            CreateMap<BookCreateDTO, Book>();
            CreateMap<Book, BookCreateDTO>();
            // create mapping for BookUpdateDTO to Book and vice versa
            CreateMap<BookUpdateDTO, Book>();
            CreateMap<Book, BookUpdateDTO>();
            // create mapping for BookDTO to Book and vice versa
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();
            // BookCopy mapping
            // create mapping for BookCopyCreateDTO to BookCopy and vice versa
            CreateMap<BookCopyCreateDTO, BookCopy>();
            CreateMap<BookCopy, BookCopyCreateDTO>();
            // create mapping for BookCopyUpdateDTO to BookCopy and vice versa
            CreateMap<BookCopyUpdateDTO, BookCopy>();
            CreateMap<BookCopy, BookCopyUpdateDTO>();
            // create mapping for BookCopyDTO to BookCopy and vice versa
            CreateMap<BookCopy, BookCopyDTO>();
            CreateMap<BookCopyDTO, BookCopy>();

            

        }
    }
}
