using AutoMapper;
using Library_API.Dtos;
using Library_API.Model;

namespace Library_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<BookCreateDTO, Book>();
            CreateMap<Book, BookCreateDTO>();
            CreateMap<BookUpdateDTO, Book>();
            CreateMap<Book, BookUpdateDTO>();
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();
            
        }
    }
}
