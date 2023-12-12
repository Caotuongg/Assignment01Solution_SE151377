using AutoMapper;
using eStoreAPI.ViewEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Repositories.Repositories.Interface;
using Services;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService orderDetailService;
        private readonly IMapper mapper;

        public OrderDetailController(IOrderDetailService orderDetailService, IMapper mapper)
        {
            this.orderDetailService = orderDetailService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(new
                {
                    Status = "Get List Success",
                    Data = orderDetailService.GetAll()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(new
                {
                    Status = "Get ID success",
                    Data = orderDetailService.Get(id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

    }
}

