using AutoMapper;
using MinimalCupons.DTO;
using MinimalCupons.Models;

namespace MinimalCupons
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon,CouponCreateDTO>().ReverseMap();
        }
    }
}
