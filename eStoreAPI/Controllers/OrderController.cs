using AutoMapper;
using eStoreAPI.ViewEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Repositories.Repositories.Interface;
using Services;
using System.Security.Claims;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }

        //[HttpPost]
        //public async Task<IActionResult> BuyCart(List<OrderDetail> orderDetails)
        //{
        //    try
        //    {
        //        var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        //        if (role == null)
        //        {
        //            return StatusCode(401, new
        //            {
        //                Status = "Error",
        //                Message = "You are not login"
        //            });
        //        }
        //        if (role == "Admin")
        //        {
        //            return StatusCode(404, new
        //            {
        //                Status = "Error",
        //                Message = "Not Found"
        //            });
        //        }
        //        var memId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        //        await orderService.AddOrder(new Order
        //        {
                    
        //            OrderDate = DateTime.Now,
        //            RequiredDate = DateTime.Now,
        //            ShippedDate = DateTime.Now,
        //            OrderDetails = orderDetails,
        //            MemberId = int.Parse(memId)
        //        }, int.Parse(memId));
        //        return StatusCode(200, new
        //        {
        //            Status = "Success",
        //            Message = "Buy Success"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Status = "Error",
        //            Message = ex.Message
        //        });
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                if (role == null)
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }
                if (role != "Admin")
                {
                    var memId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    var order = await orderService.GetOrderOfMember(int.Parse(memId));
                    if (!order.Where(x => x.OrderId == id).Any())
                    {
                        return StatusCode(404, new
                        {
                            Status = "Error",
                            Message = "Not Found"
                        });
                    }
                }
                var detailOrder = await orderService.GetOrderDetail(id);
                foreach (var item in detailOrder)
                {
                    item.Product.OrderDetails = null;
                }
                return StatusCode(200, new
                {
                    Status = "Success",
                    Data = detailOrder
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrder()
        {
            try
            {
                var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                if (role == null)
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login"
                    });
                }
                if (role == "Admin")
                {
                    return StatusCode(404, new
                    {
                        Status = "Error",
                        Message = "Not Found"
                    });
                }
                var memId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var order = await orderService.GetOrderOfMember(int.Parse(memId));
                return StatusCode(200, new
                {
                    Status = "Success",
                    Data = order.ToList(),
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }


    }
}

