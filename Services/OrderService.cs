using Repositories.Entities;
using Repositories.Repositories;
using Repositories.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrderService
    {
        Task AddOrder(Order order, int memId);
        Task<IEnumerable<OrderDetail>> GetOrderDetail(int id);
        Task<IEnumerable<Order>> GetOrderOfMember(int id);
    }
    public class OrderService : IOrderService
    {
        private readonly IAllUnit allUnit;
        public OrderService(IAllUnit allUnit)
        {
            this.allUnit = allUnit;
        }

        public async Task AddOrder(Order order, int memId)
        {
            var orderId = 0;
            var bkProduct = new List<Product>();
            try
            {
                List<OrderDetail> list = new List<OrderDetail>();
                list = order.OrderDetails.ToList();
                order.OrderDetails.Clear();
                await allUnit.OrderRepository.Add(new Order
                {
                    MemberId = memId,
                    OrderDate = DateTime.Now,
                    RequiredDate = DateTime.Now,
                    ShippedDate = DateTime.Now,
                });
                allUnit.SaveChange();
                var lastOrder = allUnit.OrderRepository.GetLastOrder(memId);
                if (lastOrder != null)
                {
                    orderId = lastOrder.OrderId;
                    foreach (var orderDetail in list)
                    {
                        var product = await allUnit.ProductRepository.Get(orderDetail.ProductId);
                        if (orderDetail.Quantity > product.UnitsInStock)
                        {
                            throw new Exception($"Can not buy product {product.ProductName}. The quantity is larger than unit in stock");
                        }
                    }
                    foreach (var orderDetail in list)
                    {
                        orderDetail.OrderId = lastOrder.OrderId;
                        orderDetail.Product = null;
                        await allUnit.OrderDetailRepository.Add(orderDetail);
                        allUnit.SaveChange();
                        var product = await allUnit.ProductRepository.Get(orderDetail.ProductId);
                        bkProduct.Add(product);
                        product.UnitsInStock -= orderDetail.Quantity;
                        await allUnit.ProductRepository.Update(product.ProductId, product);
                        allUnit.SaveChange();
                    }
                }
            }
            catch (Exception ex)
            {
                //allUnit.ProductRepository.UpdateRange(bkProduct);
                //allUnit.OrderDetailRepository.RemoveRange(orderId);
                await allUnit.OrderRepository.Delete(orderId);
                allUnit.SaveChange();
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetail(int id)
        {
            try
            {
                var order = await allUnit.OrderRepository.Get(id);
                if (order == null)
                {
                    throw new Exception("Not Found");
                }
                var orderDetails = await allUnit.OrderDetailRepository.GetAllMember();
                var returnOrder = orderDetails.Where(x => x.OrderId == id).ToList();
                foreach (var item in returnOrder)
                {
                    item.Order = null;
                    item.Product = await allUnit.ProductRepository.Get(item.ProductId);
                }
                return returnOrder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Order>> GetOrderOfMember(int id)
        {
            try
            {
                var orders = await allUnit.OrderRepository.GetAllMember();
                return orders.Where(x => x.MemberId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
