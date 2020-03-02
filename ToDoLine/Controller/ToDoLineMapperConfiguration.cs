using AutoMapper;
using Bit.Model.Contracts;
using ToDoLine.Dto;
using ToDoLine.Model;

namespace ToDoLine.Controller
{
    public class ToDoLineMapperConfiguration : IMapperConfiguration
    {
        public void Configure(IMapperConfigurationExpression mapperConfigExpression)
        {
            mapperConfigExpression.CreateMap<ToDoItemStepDto, ToDoItemStep>().ReverseMap();
        }
    }
}
