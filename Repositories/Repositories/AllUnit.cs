using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class AllUnit : IAllUnit
    {
        private IGenericRepository<Member> _memberRepository;

        private IGenericRepository<Category> _categoryRepository;

        private IProductRepository _productRepository;

        private IOrderRepository _orderRepository;

        private IOrderDetailRepository _orderDetailRepository;

        private readonly FStoreDBContext context;
        public AllUnit(FStoreDBContext context)
        {
            if (this.context == null)
            {
                this.context = context;
            }
            _memberRepository = new GenericRepository<Member>(context);
            _categoryRepository = new GenericRepository<Category>(context);
            _orderRepository = new OrderRepository(context);
            _orderDetailRepository = new OrderDetailRepository(context);
            _productRepository = new ProductRepository(context);
        }
        public IGenericRepository<Member> MemberRepository
        {
            get
            {
                return _memberRepository;
            }
        }

        public IGenericRepository<Category> CategoryRepository
        {
            get
            {
                return _categoryRepository;
            }
        }

        public IOrderDetailRepository OrderDetailRepository
        {
            get
            {
                return _orderDetailRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                return _productRepository;
            }
        }
        public IOrderRepository OrderRepository
        {
            get
            {
                return _orderRepository;
            }
        }

        public void SaveChange()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}

