using AutoMapper;
using eStoreAPI.ViewEntities;
using Repositories.Entities;

namespace eStoreAPI
{
    public class Mapper : Profile
    {
        public Mapper() 
        {
            CreateMap<Product, ProductVE>();
            CreateMap<ProductVE, Product>();

            CreateMap<Member, MemberVE>();
            CreateMap<MemberVE, Member>();

            CreateMap<Order, OrderVE>();
            CreateMap<OrderVE, Order>();

            CreateMap<OrderDetail, OrderDetailVE>();
            CreateMap<OrderDetailVE, OrderDetail>();

            CreateMap<Category, CategoryVE>();
            CreateMap<CategoryVE, Category>();
        }

    }
}
