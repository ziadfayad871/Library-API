using AutoMapper;
using rakoltodev.Models;
using rakoltodev.Models.CopyDTO;

namespace rakoltodev
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Book mapping
           CreateMap<BookDTO, BookCreateDTO>();
CreateMap<BookCreateDTO, BookDTO>();
            CreateMap<BookDTO, BookUpdateDTO>();
            CreateMap<BookUpdateDTO, BookDTO>();


        }
    }
}
