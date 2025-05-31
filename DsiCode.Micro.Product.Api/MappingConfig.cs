using AutoMapper;
using DsiCode.Micro.Product.Api.Models.Dto;

namespace DsiCode.Micro.Product.Api
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, DsiCode.Micro.Product.Api.Models.Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
 