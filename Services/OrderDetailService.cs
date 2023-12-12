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
    public interface IOrderDetailService
    {
        Task<OrderDetail> Get(int id);
        IEnumerable<OrderDetail> GetAll();
        Task<bool> Add(OrderDetail orderdetail);
        Task<bool> Update(OrderDetail orderdetail);
        Task<bool> Delete(int id);
    }
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository) 
        {
            this.orderDetailRepository = orderDetailRepository;
        }

        public Task<bool> Add(OrderDetail orderdetail)
        {
            try
            {
                //if (productRepository.GetAll(x => x.ProductName == product.ProductName).Any())
                //{
                //    throw new Exception("Productname exist");
                //}
                //member.IngredientId = AutoGenId.AutoGenerateId();
                //ingredient.IsDelete = true;
                return orderDetailRepository.Add(orderdetail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> Delete(int id)
        {
            try
            {
                var orderdetail = orderDetailRepository.Get(id).Result;
                if (orderdetail != null)
                {
                    //ingredient.IsDelete = false;
                    return orderDetailRepository.Delete(orderdetail.OrderId);
                }
                else
                {
                    throw new Exception("Not Found Order");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<OrderDetail> Get(int id)
        {
            try
            {
                return orderDetailRepository.Get(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<OrderDetail> GetAll()
        {
            try
            {
                return orderDetailRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> Update(OrderDetail item)
        {
            try
            {
                var orderdetail = orderDetailRepository.Get(item.OrderId).Result;
                //if (productRepository.GetAll(x => x.ProductId != item.ProductId && x.ProductName == item.ProductName).Any())
                //{
                //    throw new Exception("ProductName exist");
                //}
                orderdetail.UnitPrice = item.UnitPrice;
                orderdetail.Quantity = item.Quantity;
                orderdetail.Discount = item.Discount;
                

                return orderDetailRepository.Update(orderdetail.OrderId, orderdetail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
