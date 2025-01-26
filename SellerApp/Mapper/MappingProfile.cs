using AutoMapper;
using OrderCloud.SDK;
using SellerApp.Options;

namespace SellerApp.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Catalog, CatalogItemCsvModel>().ReverseMap();
            CreateMap<PartialCatalog, CatalogItemCsvModel>().ReverseMap();
            CreateMap<PartialCatalog, Catalog>().ReverseMap();

            CreateMap<PriceSchedule, PriceScheduleItemCsvModel>().ReverseMap();
            CreateMap<PartialPriceSchedule, PriceScheduleItemCsvModel>().ReverseMap();
            CreateMap<PartialPriceSchedule, PriceSchedule>().ReverseMap();
        }
    }
}
