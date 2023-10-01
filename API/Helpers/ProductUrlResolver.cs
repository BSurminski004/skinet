using API.DTOs;
using AutoMapper;
using AutoMapper.Execution;
using Core.Entities;

namespace API.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductToReturnDto, string>
    {
        private readonly IConfiguration _config;
        public ProductUrlResolver(IConfiguration config)
        {
            _config = config;
        }
        public string Resolve(Product source, ProductToReturnDto destination,
            string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                //Wykorzystanie danych z appSettings za pomocą obiektu IConfiguration
                return _config["ApiUrl"] + source.PictureUrl;
            }
            return null;
        }
    }
}
