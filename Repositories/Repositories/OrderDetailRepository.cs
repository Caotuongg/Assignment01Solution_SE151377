using Repositories.Entities;
using Repositories.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class OrderDetailRepository: GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(FStoreDBContext context) : base(context) { }

        public IEnumerable<OrderDetail> GetAll(int id)
        {
            try
            {
                return dbSet.Where(x => x.OrderId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
