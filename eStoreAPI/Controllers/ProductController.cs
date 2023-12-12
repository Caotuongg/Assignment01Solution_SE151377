using AutoMapper;
using eStoreAPI.ViewEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Repositories.Entities;
using Repositories.Repositories.Interface;
using Services;
using System.Security.Claims;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = productService.GetAll();
                var product = new List<Product>();
                foreach (var item in list)
                {
                    product.Add(mapper.Map<Product>(item));
                }
                return Ok(product);
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
                        Data = await productService.Get(id)
                    });
                
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductVE productVE)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                //if (!string.IsNullOrEmpty(role))
                //{
                //    return StatusCode(401, new
                //    {
                //        Status = "Error",
                //        Message = "You are not login",
                //    });
                //}
                if (role == "Admin")
                {
                    var product = Validate(productVE);
                    var check = productService.Add(product);
                    return await check ? Ok(new
                    {
                        Status = 1,
                        Message = "Add Product Success"
                    }) : Ok(new
                    {
                        Status = 0,
                        Message = "Add Product Fail"
                    });
                }
                else
                {
                    return StatusCode(404, new
                    {
                        Status = "Error",
                        Message = "Not Found",
                    });
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductVE productVE)
        {
            
            try
            {
                var role = User.Claims.FirstOrDefault(r => r.Type  == ClaimTypes.Role)?.Value;
                //if (!string.IsNullOrEmpty(role))
                //{
                //    return StatusCode(401, new
                //    {
                //        Status = "Error",
                //        Message = "You are not login",
                //    });
                //}
                if(role == "Admin")
                {
                    var product = Validate(productVE);
                    var check = productService.Update(product);
                    return await check ? Ok(new
                    {
                        Status = true,
                        Message = "Update Product Success"
                    }) : Ok(new
                    {
                        Status = false,
                        Message = "Update Product Fail"
                    });
                }
                else
                {
                    return StatusCode(404, new
                    {
                        Status = "Error",
                        Message = "Not Found",
                    });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                });
            }
        }

        [HttpDelete]
       

        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(role))
                {
                    return StatusCode(401, new
                    {
                        Status = "Error",
                        Message = "You are not login",
                    });
                }
                if (role != "Admin")
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Role Denied",
                    });
                }
                var pro = await productService.Get(id);
                if (pro == null)
                {
                    return StatusCode(404, new
                    {
                        Status = "Error",
                        Message = "Not Found Product"
                    });
                }
                await productService.Delete(id);
                return StatusCode(200, new
                {
                    Status = "Success"
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
        public async Task<IActionResult> Search(string search)
        {
            try
            {
                return Ok(new
                {
                    Status = "Get Name or Unitprice succesfull",
                    Data = productService.Search(search)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private Product Validate(ProductVE productVE)
        {
            if (string.IsNullOrEmpty(productVE.ProductName.Trim()))
            {
                throw new Exception("ProductName cannot be empty!!!");
            }
            if (productVE.UnitPrice < 0)
            {
                throw new Exception("UnitPrice cannot be empty!!!");
            }
            if (productVE.UnitsInStock < 0)
            {
                throw new Exception("UnitsInStock cannot be empty!!!");
            }
            if (string.IsNullOrEmpty(productVE.Weight.Trim()))
            {
                throw new Exception("Weight cannot be empty!!!");
            }

            return mapper.Map<Product>(productVE);
        }

    }
}
