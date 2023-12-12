using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.Interface
{
    public interface IAllUnit : IDisposable
    {
        IGenericRepository<Member> MemberRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        void SaveChange();
    }
}
