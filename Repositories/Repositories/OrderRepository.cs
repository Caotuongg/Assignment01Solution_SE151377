using Repositories.Entities;
using Repositories.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(FStoreDBContext context) : base(context) { }

        public Order GetLastOrder(int memId)
        {
            try
            {
                return dbSet.Where(x => x.MemberId == memId).OrderByDescending(x => x.OrderId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
